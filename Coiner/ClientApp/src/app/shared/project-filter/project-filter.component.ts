import { Component, OnInit } from '@angular/core';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
import { ProjectService } from '../../services/projectService';
import { ProjectFilterDto } from '../../models/projectFilterDto';
import { TranslateService } from '@ngx-translate/core';
import { Project } from '../../models/project';
import { CommonService } from '../../services/commonService';

declare var $: any;
@Component({
    selector: 'project-filter',
    templateUrl: './project-filter.component.html',
    styleUrls: ['./project-filter.component.css']
})
export class ProjectFilterComponent implements OnInit {

    constructor(private _projectService: ProjectService,
        private _translateService: TranslateService,
        private _commonService: CommonService
    ) {
        this._translateService.onLangChange.subscribe(() => {
            this.initProjectTypesTranslation();
            this.initActivityTypesTranslation();
        })
        this.initProjectTypesTranslation();
        this.initActivityTypesTranslation();
        this.projectFilterDto = new ProjectFilterDto();
        this._projectService.filteredProjects.subscribe(
            (filtereProjects) => {
                this.projects = filtereProjects;
            }
        )
        this._projectService.filteredProjectsCount.subscribe(
            (filteredProjectsCount) => {
                this.progectsTotalCount = filteredProjectsCount;
            }
        )
    }
    itemsPerPage = 6;
    page = 1;
    projectFilterDto: ProjectFilterDto;
    projects: any[] = [];
    mode: string;
    public activityTypes: IMultiSelectOption[];
    public projectTypes: IMultiSelectOption[];
    progectsTotalCount: number;


    activityTypesTexts: IMultiSelectTexts;
    projectTypesTexts: IMultiSelectTexts;

    mySettings: IMultiSelectSettings = {
        checkedStyle: 'fontawesome',
        buttonClasses: 'btn btn-default btn-block',
        dynamicTitleMaxItems: 3,
        displayAllSelectedText: true,
        showCheckAll: true,
        showUncheckAll: true
    };

    ngOnInit() {
        this.newFilter();
    }

    getFilteredProjects() {
        this.projectFilterDto.pageIndex = (this.page - 1) * this.itemsPerPage;
        this.projectFilterDto.pageSize = this.itemsPerPage;
        this._projectService.getFiltredProjects(this.projectFilterDto).subscribe(
            (data: any) => {
                let newProjects = [];
                if (this.mode == 'getMore') {
                    let oldProjects = this._projectService.filteredProjects.value;
                    newProjects = oldProjects.concat(data.projects);
                } else {
                    newProjects = data.projects;
                }
                this._projectService.filteredProjects.next(newProjects);
                this._projectService.filteredProjectsCount.next(data.projectsCount);
                this._commonService.enableScroll();
                this._commonService.scrollDisabled = false;
                document.getElementById('loader').style.display = 'none';
            },
            (err) => {
                if (err.error instanceof Error) {
                    console.log("Client-side Error occured");
                } else {
                    console.log("Server-side Error occured");
                }
            }
        )
    }

    getNewFilteredProjects() {
        this.page = 1;
        this.mode = 'newFilter';
        this._projectService.filteredProjects.next([]);
        this.getFilteredProjects();
    }

    newFilter() {
        this.mode = 'newFilter';
        this._projectService.filteredProjects.next([]);
        this.itemsPerPage = 6;
        this.page = 1;
        this.projectFilterDto.projectActivityTypes = this.getAllActivityTypes();
        this.projectFilterDto.projectTypes = [0, 1, 2, 3];
        this.getFilteredProjects();
    }

    initProjectTypesTranslation() {
        this._translateService.get('ProjectFilterPanel.ProjectTypesFilter.ProjectTypesTexts')
            .subscribe((res) => {
                this.projectTypesTexts = res;
            });
        this._translateService.get('ProjectFilterPanel.ProjectTypesFilter.ProjectTypes')
            .subscribe((res: any[]) => {
                this.projectTypes = [];
                let id = 0;
                for (let key in res) {
                    this.projectTypes.push({
                        id: id,
                        name: res[key]
                    });
                    id++;
                }
            });
    }


    initActivityTypesTranslation() {
        this._translateService.get('ProjectFilterPanel.ActivityTypesFilter.ActivityTypesTexts')
            .subscribe((res) => {
                this.activityTypesTexts = res;
            });
        this.activityTypes = [];

        this._translateService.get('AllActivityTypesCreation')
            .subscribe((res: any[]) => {
                let id = 0;
                for (let key in res) {
                    this.activityTypes.push({
                        id: res[key].value,
                        name: res[key].name
                    });
                    id++;
                }
            });
    }

    getAllActivityTypes(){
        let allActivityTypes = [];
        this._translateService.get('AllActivityTypesCreation')
        .subscribe((res: any[]) => {
            let id = 0;
            for (let key in res) {
                allActivityTypes.push(
                    res[key].value,
                );
                id++;
            }
        });
        return allActivityTypes;
    }
}

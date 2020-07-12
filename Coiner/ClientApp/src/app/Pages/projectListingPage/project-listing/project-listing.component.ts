import { Component, OnInit, ViewChild, HostListener, OnDestroy, AfterViewInit, Renderer } from '@angular/core';
import { ProjectService } from '../../../services/projectService';
import { Project } from '../../../models/project';
import { Constants } from '../../../constants';
import { Router, ActivatedRoute } from '@angular/router';
import { ProjectFilterComponent } from '../../../shared/project-filter/project-filter.component';
import { CommonService } from '../../../services/commonService';

declare var $: any;

@Component({
  selector: 'project-listing',
  templateUrl: './project-listing.component.html',
  styleUrls: ['./project-listing.component.css']
})
export class ProjectListingComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild('projectFilter') projectFilter: ProjectFilterComponent;
  projects: Project[];
  projectsCount: number;
  filteredProjectsCount: number;
  displayScrollTo: boolean = false;
  scrollEvent: any;
  emptyProjects: boolean = false;
  seeMore: any;
  scrollHeight: number;
  elementHeight: any;
  $: any;
  constructor(private _projectService: ProjectService,
    private _router: Router,
    private route: ActivatedRoute,
    public _commonService: CommonService,
    private renderer: Renderer) {

    var projectsCount = this.route.snapshot.data.projects[1];
    this._projectService.projectCarouselsCount.next(projectsCount);
    if (projectsCount == 0) {
      this.emptyProjects=true;
    }
    this.scrollHeight = 0;
    this.elementHeight = 0;
  }
  
  ngOnInit() {   
    this._commonService.scroll();
    this._projectService.filteredProjects.subscribe(
      (filteredProjects) => {
        this.projects = filteredProjects;
        if (this.projects.length >= 6) { 
             this.displayScrollTo = true; 
        }
      }
    )
     this._projectService.filteredProjectsCount.subscribe(
      (filteredProjectsCount) => {
        this.filteredProjectsCount = filteredProjectsCount;  
        if(this.projects.length == this.filteredProjectsCount) clearInterval(this.seeMore);
        }
    )
  }

  ngAfterViewInit(){
    this.seeMore = setInterval(()=>{
          this.scrollHeight = document.getElementById('scrollable').scrollHeight;
          this.elementHeight = $('#scrollable').height();
          if(this.scrollHeight>this.elementHeight){
          clearInterval(this.seeMore); return;
         }
         this.getMoreProjects();
      },1000);   
    this.scrollEvent = this.renderer.listen(document.getElementById('scrollable'), 'scroll', () => {
      this.onWindowScroll();
    })
  }

  ngOnDestroy() {
    this.seeMore;
    this.scrollEvent();
    this._projectService.filteredProjects.next([]);
  }
  
  getMoreProjects() {
    this.projectFilter.page += 1;
    this.projectFilter.mode = 'getMore';
    this.projectFilter.getFilteredProjects();    
  }


  // @HostListener('window:scroll')
  onWindowScroll() {    
    if (this.displayScrollTo) {
      if (this._commonService.scrollDisabled || this.projects.length == this.filteredProjectsCount) {        
        return;
      }
      let offsetHeight = document.getElementById('scrollable').offsetHeight;
      let scrollHeight = document.getElementById('scrollable').scrollHeight;
      let scrollTop = document.getElementById('scrollable').scrollTop;
      let heightDifference = scrollHeight-offsetHeight;
          
      if ( scrollTop > (heightDifference-20) ) {
        this._commonService.disableScroll();
        this._commonService.scrollDisabled = true;
        document.getElementById('loader').style.display = 'block';
        this.getMoreProjects();
      }
    } 
  } 
}

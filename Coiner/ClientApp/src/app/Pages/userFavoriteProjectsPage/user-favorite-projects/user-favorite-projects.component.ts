import { Component, OnInit} from '@angular/core';
import { ProjectService } from '../../../services/projectService';
import { Constants } from '../../../constants';
import { Router } from '@angular/router';
import { CommonService } from '../../../services/commonService';
import { LocalStorageService } from '../../../services/local-storage.service';

declare var $: any;

@Component({
  selector: 'app-user-favorite-projects',
  templateUrl: './user-favorite-projects.component.html',
  styleUrls: ['./user-favorite-projects.component.css']
})
export class UserFavoriteProjectsComponent implements OnInit {

  favoriteProjects: any;
  success: boolean = false;
  emptyFavorites: boolean = false;
  isRunning : boolean = false;
  constructor(private _projectService: ProjectService,
    private _router: Router,
    private _commonService: CommonService,
    private _localStorageService: LocalStorageService) {
  }

  ngOnInit() {
    this.getfavoriteProjects();
  }

  getfavoriteProjects() {
    this._commonService.scrollTop();
     this.isRunning = true;

    if (this.favoriteProjects == null || this.favoriteProjects.length == 0) {
      let ids = [].concat(this._localStorageService.getData('favoriteChocolateIds', []));

      if (ids.length == 0) {
        this.isRunning = false;
        this.emptyFavorites = true;
        return;
      }

      this._projectService.getfavoriteProjects(ids).subscribe(
        res => {
          // this.updateFavoriteChocolateIds(res);
          this.favoriteProjects = res;
          this.emptyFavorites = false;
          this.success = true;
           this.isRunning = false;
        },
        err => {
          console.error(err);
          this.emptyFavorites = false;
          this.success = false;
           this.isRunning = false;
        }
      );
    } else {
       this.isRunning = false;
    }
  }
}

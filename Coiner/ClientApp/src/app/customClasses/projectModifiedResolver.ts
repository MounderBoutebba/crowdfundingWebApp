import { Injectable } from '@angular/core';

import { Resolve, Router } from '@angular/router';

import { Observable } from 'rxjs';


import { ProjectService } from '../services/projectService';

@Injectable()
export class ProjectModifiedResolver implements Resolve<any> {
  constructor(private _projectService: ProjectService,
              private _router: Router) {}

  resolve() {
    if (this._projectService.projectModified == null) {
        this._router.navigateByUrl('dashboard?page=myProjects');
    }
  }
}
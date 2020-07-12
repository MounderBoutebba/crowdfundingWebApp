import { Injectable } from '@angular/core';

import { Resolve, Router } from '@angular/router';

import { Observable } from 'rxjs';


import { ProjectService } from '../services/projectService';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { Project } from '../models/project';
//import { ActivatedRoute, Router } from '@angular/router';

@Injectable()
export class ProjectDetailsFromUrlResolver implements Resolve<any> {
  projectUrlId: number;
  project: Project;
  constructor(private _projectService: ProjectService,
    private _route: ActivatedRoute,
    private _router: Router) { }

  resolve(r: ActivatedRouteSnapshot) {
    this.projectUrlId = r.params['id'];
    if (isNaN(this.projectUrlId)) {
      this._router.navigateByUrl('/');
    } else {
      return this._projectService.getProjectId(this.projectUrlId);
    }
  }
}

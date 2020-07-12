import { Component, OnInit } from '@angular/core';
import { User } from '../../../models/user';
import { Router } from '@angular/router';
import { ProjectService } from '../../../services/projectService';
import { UserService } from '../../../services/userService';


declare var $: any;

@Component({
  selector: 'project-edit-wizard',
  templateUrl: './project-edit-wizard.component.html',
  styleUrls: ['./project-edit-wizard.component.css']
})
export class ProjectEditWizard implements OnInit {

  user: User;

  constructor(private projectService: ProjectService,
    private _router: Router,
    private _userService: UserService) {
    this._userService.currentUser.subscribe((currenUser) => {
      this.user = currenUser;
    })
  }

  ngOnInit() {
  }
}


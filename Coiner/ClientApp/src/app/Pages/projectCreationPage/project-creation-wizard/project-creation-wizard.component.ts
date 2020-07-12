import { NgForm } from '@angular/forms';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Project } from '../../../models/project';
import { ProjectService } from '../../../services/projectService';
import { HttpErrorResponse } from '@angular/common/http/src/response';
import { ImageCropperComponent, CropperSettings } from 'ng2-img-cropper';
import { Picture } from '../../../interfaces/picture';
import { UploadedFile } from '../../../interfaces/uploadedFile';
import { Router, ActivatedRoute } from '@angular/router';
import { forEach } from '@angular/router/src/utils/collection';
import { UploadedDocument } from '../../../models/uploadedDocument';
import { ProjectImage } from '../../../models/projectImage';
import { Constants } from '../../../constants';
import { CommonService } from '../../../services/commonService';
import { UserService } from '../../../services/userService';
import { User } from '../../../models/user';
import { LocalStorageService } from '../../../services/local-storage.service';
import { UserTypeEnum } from '../../../models/enums/userType';

declare var $: any;

@Component({
  selector: 'project-creation-wizard',
  templateUrl: './project-creation-wizard.component.html',
  styleUrls: ['./project-creation-wizard.component.css']
})
export class ProjectCreationWizardComponent implements OnInit {
  user: User;
  constructor(private _route: ActivatedRoute,
    public _commonService: CommonService,
    public _userService: UserService,
    private _localStorageServicve: LocalStorageService,
    private _router: Router) {
    this.user = _route.snapshot.data.currentUser.currentUser;
  }
  ngOnInit() {
    debugger;
    if (this.user == null || this.user.userType != UserTypeEnum.Entreprise) {
      this._router.navigateByUrl('/');
    }
  }
}

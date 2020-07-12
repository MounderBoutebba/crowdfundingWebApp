import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Project } from '../../models/project';
import { Constants } from '../../constants';
import { NouiFormatter } from "ng2-nouislider";
import { CustomTooltip } from '../../customClasses/customTooltip';
import { ProjectService } from '../../services/projectService';
import { Router } from '@angular/router';
import { CommonService } from '../../services/commonService';
import { UserService } from '../../services/userService';
import { User } from '../../models/user';
import { Slider } from '../../models/slider';
import { TranslateService } from '@ngx-translate/core';
import { NgForm } from '@angular/forms';
import { ContactUsDto } from '../../models/contactUsDto';
import { ProjectStatusEnum } from '../../models/enums/projectStatusEnum';


declare var $: any;
@Component({
  selector: 'project-listing-item',
  templateUrl: './project-listing-item.component.html',
  styleUrls: ['./project-listing-item.component.css']
})
export class ProjectListingItemComponent implements OnInit {

  newsFormInputsInvalide: boolean;
  newsFormInvalid: boolean;
  projectFormInvalid: boolean;
  projectFormInputsInvalide: boolean;
  @Input() project: Project;
  @Input() slider: string;
  @Output() validateProject: EventEmitter<boolean> = new EventEmitter();
  @Output() loaderIsRunning: EventEmitter<boolean> = new EventEmitter();

  currentUser: User;
  readonly constants = Constants;
  isRunning: boolean = false;
  adminLogin: string;
  someKeyboardConfig: any = {
    behaviour: 'drag',
    connect: [true, false],
    tooltips: new CustomTooltip,
    start: 0,
    keyboard: true, // same as [keyboard]="true"
    step: 1,
    pageSteps: 10, // number of page steps, defaults to 10
    // range: {
    //   min: 0,
    //   max: 5  
    // }
  }
  formInfos: ContactUsDto = new ContactUsDto();
  DefaultNewsMessage: boolean = true;
  DefaultProjectMessage: boolean = true;
  newsAdded: boolean = false;
  projectInfosAdded: boolean = false;
  newsText: string;
  projectEditText: string;
  constructor(public _projectService: ProjectService,
    private _router: Router,
    public _commonService: CommonService,
    private _userService: UserService,
    private _translateService: TranslateService) {
  }

  ngOnInit() {
    $('[data-toggle="tooltip"]').tooltip();
    if (this.project.beginEstimatedDate != null) {
      this.project.beginEstimatedDate = new Date(this.project.beginEstimatedDate);
    }
    this._userService.adminLogin.subscribe((adminLogin) => {
      this.adminLogin = adminLogin;
    });
    this._userService.currentUser.subscribe(
      (currentUser) => {
        this.project.slider = new Slider();
        this.currentUser = currentUser;
        if (this.currentUser != null) {
          (this.currentUser.userCoinsNumber == 0)
            ? this.project.slider.sliderMax = 1
            : this.project.slider.sliderMax = this.currentUser.userCoinsNumber;
          if (this.currentUser.userCoinsNumber == 0) {
            this.project.slider.disabledSlider = true;
          } else {
            this.project.slider.disabledSlider = false;
          }
        }
      }
    )
  }

  confirmProject() {
    this.isRunning = true;
    this._projectService.confirmProject(this._projectService.projectConfirmed.id).subscribe(
      (projects) => {
        this.isRunning = false;
        this.validateProject.emit(true);
      }, (err) => {
        this.isRunning = false;
        this.validateProject.emit(false);
      });
  }

  getDetails() {
    if (this.slider == 'off') {
      return;
    }
    this._projectService.projectDetails = this.project;
    this._projectService.smallDetailsIsClicked = true;
    this._router.navigateByUrl("details-projet/" + this.project.id);
    this._commonService.scrollTop();
  }

  editProject() {
    this._projectService.projectModified = this.project;
    if (this.currentUser.login == this.adminLogin) {
      this._router.navigateByUrl('modifier-projet');
      this._commonService.scrollTop();
    }
    else {
      $('#editProjectConfirm').modal('show');
    }
  }

  editProjectConfirmation(projectForm: NgForm) {
    this.projectFormInputsInvalide = projectForm.invalid;
    this.projectFormInvalid = this.projectFormInputsInvalide;
    if (!this.projectFormInvalid) {
      if (this.projectEditText != null) {
        this.isRunning = true;
        this.formInfos.email = this.currentUser.email;
        this.formInfos.message = this.projectEditText;
        this.formInfos.name = this.currentUser.firstName;
        this._userService.SendEditProjectRequest(this.formInfos, this.project.projectName).subscribe(
          () => {
            this.isRunning = false;
            this.DefaultProjectMessage = false;
            this.projectInfosAdded = true;
          },
          err => {
            this.DefaultProjectMessage = false;
            this.projectInfosAdded = false;
            this.isRunning = false;
          }
        );
        this.hideProjectConfirmModal();
      }
    }
  }

  onhideEditProjectConfirmModal() {
    $('#editProjectConfirm').on('hidden.bs.modal', () => {
      this.isRunning = false;
      this.projectInfosAdded = false;
      this.DefaultProjectMessage = true;
      this.projectFormInvalid = false;
      this.projectFormInputsInvalide = false;
      this.projectEditText = "";
    })
  }

  hideProjectConfirmModal() {
    $('#editProjectConfirm').modal('hide');
    this.onhideEditProjectConfirmModal();
  }

  ShowProjectNewsModel() {
    this._projectService.projectNews = this.project;
    if (this.currentUser.login == this.adminLogin) {
      $('#projectNewsModel').modal('show');
    }
    else {
      $('#askAddNewsModal').modal('show');
    }
  }

  askToAddNews(newsForm: NgForm) {
    this.newsFormInputsInvalide = newsForm.invalid;
    this.newsFormInvalid = this.newsFormInputsInvalide;
    if (!this.newsFormInvalid) {
      if (this.newsText != null) {
        this.isRunning = true;
        this.formInfos.email = this.currentUser.email;
        this.formInfos.message = this.newsText;
        this.formInfos.name = this.currentUser.firstName;
        this._userService.SendAddNewsRequest(this.formInfos, this.project.projectName).subscribe(
          () => {
            this.isRunning = false;
            this.newsAdded = true;
            this.DefaultNewsMessage = false;
          },
          err => {
            this.newsAdded = false;
            this.DefaultNewsMessage = false;
            this.isRunning = false;
          }
        );
        this.hideAskAddNewsModal();
      }
    }
  }

  onhideAskAddNewsModal() {
    $('#askAddNewsModal').on('hidden.bs.modal', () => {
      this.isRunning = false;
      this.newsAdded = false;
      this.DefaultNewsMessage = true;
      this.newsFormInvalid = false;
      this.newsFormInputsInvalide = false;
      this.newsText = "";
    })
  }

  hideAskAddNewsModal() {
    $('#askAddNewsModal').modal('hide');
    this.onhideAskAddNewsModal();
  }

  ShowConfirmProjectModel() {
    this._projectService.projectConfirmed = this.project;
    if ((this.project.productName != null && this.project.productName != "")
      && this.project.product_TVA > 0) {
      $('#confirmProject').modal('show');
    } else {
      $('#confirmProjectError').modal('show');
    }
  }

  ShowConfirmEndSurveyModal() {
    this._projectService.projectEndSurvey = this.project;
    $('#confirmEndSurvey').modal('show');
  }

  endSurvey() {
    this.isRunning = true;
    this.loaderIsRunning.emit(true);
    this._projectService.endSurvey(this._projectService.projectEndSurvey.id).subscribe(
      (data) => {
        this.project.projectStatus = this._projectService.projectEndSurvey.projectStatus = ProjectStatusEnum.EndSurvey;
        this.isRunning = false;
        this.loaderIsRunning.emit(false);
      },
      (err) => {
        this.isRunning = false;
        this.loaderIsRunning.emit(false);
      }
    );
    this.isRunning = false;
    $('#confirmEndSurvey').modal('hide');
  }

  refundUsers(projectId: number) {
    this.isRunning = true;
    this.loaderIsRunning.emit(true);
    this._projectService.refundUsers(projectId).subscribe(
      (data) => {
        this.project.projectStatus = data.projectStatus;
        this.isRunning = false;
        this.loaderIsRunning.emit(false);
      },
      (err) => {
        this.isRunning = false;
        this.loaderIsRunning.emit(false);
      }
    );
  }
}

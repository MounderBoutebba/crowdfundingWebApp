import { Component, OnInit, ViewChild } from '@angular/core';
import { AfterViewInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { ProjectService } from '../../../services/projectService';
import { Project } from '../../../models/project';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { Constants } from '../../../constants';
import { UserService } from '../../../services/userService';
import { ContactUsDto } from '../../../models/contactUsDto';
import { RecaptchaComponent } from 'ng-recaptcha';
import { CommonService } from '../../../services/commonService';

declare var jQuery: any;
declare var $: any;
@Component({
  selector: 'app-home-content',
  templateUrl: './home-content.component.html',
  styleUrls: ['./home-content.component.css']
})
export class HomeContentComponent implements OnInit {

  @ViewChild('recaptchaRef') recaptchaRef: RecaptchaComponent;

  constructor(private _projectServicve: ProjectService,
    private userservice: UserService,
    private _router: Router,
  
    private _commonService: CommonService) {
    this.isRunningProjects = true;
    this.isRunningProducts = true;
    this._projectServicve.getLatestProjectsList()
      .subscribe(
      (data) => {
        this.isRunningProjects = false;
        console.log(data);
        this.latestprojects = data[0].slice(0, 3);
        this.projects = data[0];
      },
      (err) => {
        this.isRunningProjects = false;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
      }
      );
      this.userservice.currentUser.subscribe(
        (currentUser) => {
          this.currentUser = currentUser;
        }
      );
    this._projectServicve.GetLatestProducts()
      .subscribe(
      (data) => {
        this.isRunningProducts = false;
        this.products = data[0];
        this._projectServicve.productCarouselsCount.next(data[1]);
      }, (err) => {
        this.isRunningProducts = false;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
      })
  }
  products: any;
  projects: any[];
  latestprojects: any[];
  carouselBanner: any;
  searchInput: string = "";
  projectsSearchResult: Project[] = [];
  applySearch: boolean = false;
  isRunningProjects:boolean = false;
  isRunningProducts:boolean = false;
  success: boolean = false;
  constants = Constants;
  currentUser;
  formInvalid: boolean = false;
  captchaResponse: boolean = false;
  sendSuccess: boolean = false;
  fail: boolean = false;
  sendForm: boolean = false;
  LoaderContactIsRunning: boolean = false;
  contacUstDto: ContactUsDto;

  ngOnInit() {
    this._commonService.scrollTop();
    this.contacUstDto = new ContactUsDto();
    this.carouselBanner = {
      grid: { xs: 1, sm: 1, md: 1, lg: 1, all: 0 },
      slide: 1,
      speed: 400,
      interval: 4000,
      point: {
        visible: false
      },
      load: 2,
      loop: true,
      touch: true
    }
  }

  searchForProjects() {
    if (this.searchInput.length == 0) {
      return;
    }
    this._router.navigateByUrl("rechercher-projet/" + this.searchInput);
  }

  clearSearchInput() {
    this.searchInput = '';
    this.projectsSearchResult = [];
    this.success = false;
    this.applySearch = false;
  }

  sendEmail(form: NgForm) {
    this.formInvalid = form.invalid || !this.captchaResponse;
    if (!this.formInvalid) {
      this.sendForm = true;
      this.LoaderContactIsRunning = true;
      this.userservice.SendMessageFromContactUs(this.contacUstDto).subscribe(
        (response) => {
          this.LoaderContactIsRunning = false;
          this.sendSuccess = true;
          this.contacUstDto = new ContactUsDto();
          this.recaptchaRef.reset();
        },
        (err) => {
          this.LoaderContactIsRunning = false;
          this.recaptchaRef.reset();
          this.fail = false;
        }
      )
    }
  }

  resolved(captchaResponseToken: string) {
    if (captchaResponseToken == null) {
      return;
    }
    this.LoaderContactIsRunning = true;
    this.userservice.getRecaptchaResponse(captchaResponseToken).subscribe(
      (captchaResponse) => {
        this.LoaderContactIsRunning = false;
        this.captchaResponse = captchaResponse;
      }
    )
  }
  checkConnectedUser() {
    if (this.currentUser != null) {
      this._router.navigateByUrl('creer-projet');
    } else {
      $('#loginModal').modal('show');
    }
  }
}

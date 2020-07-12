import { AuthService, GoogleLoginProvider } from 'angularx-social-login';
import { SocialUser } from 'angularx-social-login';
import { NgForm } from '@angular/forms';
import { UserService } from './../../../services/userService';
import { HttpErrorResponse } from '@angular/common/http/src/response';
import { User } from './../../../models/user';
import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';

declare var $: any;

@Component({
  selector: 'user-login',
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.css']
})
export class UserLoginComponent implements OnInit {

  private googleData: SocialUser;
  private loggedIn: boolean;

  isRunning: boolean = false;
  success: boolean = false;
  sendForm: boolean = false;
  iWillSend: boolean = false;
  login: string;
  password: string;

  formInvalid: boolean = false;
  formInputsInvalide = false;
  userActive: boolean = false;
  currentUser: any;

  constructor(private authService: AuthService, public _userService: UserService, private _router: Router) {
  }
  ngOnInit() {
    this.authService.authState.subscribe((user) => {
      this.googleData = user;
    });

    $('#DemandeKycModal').on('hidden.bs.modal', () => {
      this.iWillSend = false;
    })
  }

  loginUser(form: NgForm) {
    this.formInputsInvalide = form.invalid;
    this.formInvalid = this.formInputsInvalide;
    if (!this.formInvalid) {
      this.isRunning = true;
      this._userService.loginUser(this.login, this.password)
        .subscribe(
          (data) => {
            this.sendForm = true;
            this.success = true;
            this.isRunning = false;
            this.currentUser = data[0];
            if (this.currentUser != null) {
              this.userActive = data[3];
              if (this.userActive) {
                this.hideLoginModal();
                let token = data[1].token;
                let adminLogin = data[2];
                this._userService.setLocalUser(this.currentUser.id, this.currentUser.login, this.currentUser.firstName, this.currentUser.lastName, token);
                this._userService.currentUser.next(this.currentUser);
                this._userService.adminLogin.next(adminLogin);
                if (this.currentUser.kyc == true && this.currentUser.kycNotificationSent == false) {
                  $('#DemandeKycModal').modal('show');
                }
              } else {
                console.log(data);
              }
            }
          },
          (err: HttpErrorResponse) => {
            this.isRunning = false;
            this.sendForm = true;
            this.success = false;
            if (err.error instanceof Error) {
              console.log("Client-side Error occured");
            } else {
              console.log("Server-side Error occured");
            }
          });
    }
  }

  openLoginModal() {
    $('#loginModal').modal('show');
    $('#loginModal').on('hidden.bs.modal', () => {
      this.sendForm = false;
      this.success = false;
      this.formInvalid = false;
    })
  }

  openUserModal() {
    this.hideLoginModal();
    $('#userModal').modal('show');
  }

  hideLoginModal() {
    $('#loginModal').modal('hide');
  }

  OpenDemandNewPasswordModel() {
    this.hideLoginModal();
    $('#DemandNewPasswordModel').modal('show');
  }

  loginUserWithGogle() {

    this._userService.loginUserWithGogle(this.googleData.email)
      .subscribe(
        (data) => {
          this.isRunning = false;
          this.sendForm = true;
          this.success = true;
          this.currentUser = data[0];
          if (this.currentUser != null) {
            this.userActive = data[3];
            if (this.userActive) {
              this.hideLoginModal();
              let token = data[1].token;
              let adminLogin = data[2];
              this._userService.setLocalUser(this.currentUser.id, this.currentUser.login, this.currentUser.firstName, this.currentUser.lastName, token);
              this._userService.currentUser.next(this.currentUser);
              this._userService.adminLogin.next(adminLogin);
            } else {
              console.log(data);
            }
          }
        },
        (err: HttpErrorResponse) => {
          this.isRunning = false;
          this.sendForm = true;
          this.success = false;
          if (err.error instanceof Error) {
            console.log("Client-side Error occured");
          } else {
            console.log("Server-side Error occured");
          }
        });
  }

  signInWithGoogle(): void {
    this.isRunning = true;
    this.authService.signIn(GoogleLoginProvider.PROVIDER_ID).then(
      (googledata) => {
        this.loginUserWithGogle();
      }, (reason: any) => { this.isRunning = false; }
    )

  }

  signOut(): void {
    this.authService.signOut();
  }


  sendByEmail() {
    this.iWillSend = true;
  }


  updateKyc() {
    this._userService.updateKycNotification(this.currentUser.id)
      .subscribe(
        (data) => {
          this.currentUser.KycNotificationSent = true;
        },
        (err: HttpErrorResponse) => {
          this.currentUser.KycNotificationSent = false;
        });
  }



}

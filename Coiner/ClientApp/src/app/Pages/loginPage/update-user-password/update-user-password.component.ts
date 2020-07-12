import { HttpErrorResponse } from '@angular/common/http';
import { NgForm } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Constants } from '../../../constants';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../../services/userService';
import { Route } from '@angular/router/src/config';


declare var $: any;

@Component({
  selector: 'update-user-password',
  templateUrl: './update-user-password.component.html',
  styleUrls: ['./update-user-password.component.css']
})

export class UpdateUserPasswordComponent implements OnInit {

  readonly Constants = Constants;

  isRunning: boolean = false;
  formInvalid: boolean = false;
  passwordMisMatch = false;
  formInputsInvalide = false;
  sub: any;
  password: string;
  confirmPassword: string;
  id: string;
  passwordUpdatted: boolean = false;
  formSent: boolean = true;
  idExiste: boolean = false;

  constructor(private _route: ActivatedRoute, private _userService: UserService, private _router: Router) {
  }

  ngOnInit() {

  }

  updatePassword(form: NgForm) {
    this.formInputsInvalide = form.invalid;
    this.passwordMisMatch = form.controls.Password.value !== form.controls.confirmerPW.value;
    this.formInvalid = this.formInputsInvalide || this.passwordMisMatch;
    if (!this.formInvalid) {
      this.isRunning = true;
      this.sub = this._route.params.subscribe(params => {
        this.id = params['id'];
      });
      this._userService.updateUserPassword(this.id, this.password).subscribe((res) => {
        this.passwordUpdatted = true;
        this.formSent = true;
        this.isRunning = false;
        $('#passChangeModal').modal('show');
        $('#passChangeModal').on('hidden.bs.modal', () => {
          this.hidePassChangeModal();
        });
      }, (err: HttpErrorResponse) => {
        if (err.status == 404) {
          this.passwordUpdatted = false;
          this.formSent = false;
          this.idExiste = false;
          this.isRunning = false;
        } else {
          this.formSent = false;
          this.idExiste = true;
          this.isRunning = false;
        }
        $('#passChangeModal').modal('show');
        $('#passChangeModal').on('hidden.bs.modal', () => {
          this.hidePassChangeModal();
        });
      })

    }
  }

  hidePassChangeModal() {
    this.passwordUpdatted = false;
    this.formSent = false;
    this.idExiste = false;
    this.isRunning = false;
    this.formInvalid = false;
    this._router.navigateByUrl("/");
  }
}

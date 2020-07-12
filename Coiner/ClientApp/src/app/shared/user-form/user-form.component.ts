import { ProviderEnum } from './../../models/enums/providerEnum';
import { UserTypeEnum } from './../../models/enums/userType';
import { CommonService } from './../../services/commonService';
import { HttpClient } from '@angular/common/http';
import { Address } from './../../models/address';
import { UserImage } from './../../models/userImage';
import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http/src/response';
import { User } from './../../models/user';
import { UserService } from '../../services/userService';
import { ImageCropperComponent, CropperSettings } from 'ng2-img-cropper';
import { Picture } from './../../interfaces/picture';
import { Router } from '@angular/router';
import { Constants } from './../../constants';
import { Form } from '@angular/forms/src/directives/form_interface';
import { NgForm } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { RecaptchaComponent } from 'ng-recaptcha';
import { debug } from 'util';
import { AuthService, SocialUser } from "angularx-social-login";
import { GoogleLoginProvider } from "angularx-social-login";

declare var $: any;

@Component({
  selector: 'user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit {

  @ViewChild('cropModal') cropModal: ElementRef;
  userTypeEnum = UserTypeEnum; // local field for accessing Enum in the dom
  usertype: UserTypeEnum = UserTypeEnum.Particulier;
  private googleData: SocialUser;
  private loggedIn: boolean;
  // Frm validation variables
  formInvalid: boolean = false;
  isRunning: boolean = false;
  passwordMisMatch = false;
  formInputsInvalide = false;
  loginExsiste = false;
  fileUploadMaxSize = Constants.FileUploadMaxSize;
  isMaxSize: boolean = false;
  isNotImage: boolean = false;
  //Cropper 2 data
  readonly Constants = Constants;
  data: any;
  cropperSettings: CropperSettings;
  @ViewChild('cropper', undefined) cropper: ImageCropperComponent;
  @ViewChild('recaptchaRef') recaptchaRef: RecaptchaComponent;

  countries: any;
  newPicture: Picture;
  success: boolean = false;
  minBirthday: any;
  maxBirthday: any;
  @Input() editMode: string = "new";
  @Input() user: User;

  genders = [{ name: "Homme", value: 0 },
  { name: "Femme", value: 1 }
  ];
  userReset: User;
  checkRecaptchaClicked: boolean = false;
  submitForm: boolean = false;
  captchaResponse: boolean = false;
  bsConfig: Partial<BsDatepickerConfig>;
  isSubmitClicked: boolean = false;
  constructor(private authService: AuthService, private userservice: UserService, private _http: HttpClient, private _router: Router, public _commonService: CommonService, private _sanitizer: DomSanitizer) {

    // this.getCountries();
    //Cropper settings
    this._commonService.scroll();
    this.cropperSettings = new CropperSettings();

    this.cropperSettings.width = 700;
    this.cropperSettings.height = 600;
    this.cropperSettings.minWidth = 200;
    this.cropperSettings.minHeight = 200;
    this.cropperSettings.croppedWidth = 700;
    this.cropperSettings.croppedHeight = 600;

    this.cropperSettings.cropperDrawSettings.strokeColor = 'rgba(255,255,255,1)';
    this.cropperSettings.cropperDrawSettings.strokeWidth = 2;
    this.cropperSettings.noFileInput = true;

    let windowWidth = $(window).width();
    if (windowWidth >= 768) {
      this.cropperSettings.canvasWidth = 568;
      this.cropperSettings.canvasHeight = 300;
    } else {
      this.cropperSettings.canvasWidth = windowWidth - 52;
      this.cropperSettings.canvasHeight = 200;
    }

    this.data = {};

  }

  createUser(form: NgForm) {

    this.submitForm = true;
    this.formInputsInvalide = form.invalid;
    this.passwordMisMatch = form.controls.Password.value !== form.controls.confirmerPW.value;
    this.formInvalid = this.formInputsInvalide || this.passwordMisMatch || !this.captchaResponse;
    if (!this.formInvalid) {
      this.isSubmitClicked = true;
      if (this.editMode == "new") {
        this.saveUserImage();
        this.user.login = this.user.email;
        this.user.provider = ProviderEnum.Coiner;
        this.user.userType = this.usertype;
        this.isRunning = true;
        this.userservice.createUser(this.user)
          .subscribe(
            res => {
              this.isRunning = false;
              this.success = true;
              console.log(res);
              $('#userCompteModal').modal('show');
              // $('#userModal').modal('hide');
              $('#userCompteModal').on('hidden.bs.modal', () => {
                this._router.navigateByUrl("/");
              });
              this.focusOnVisibleModal();
            },
            (err: HttpErrorResponse) => {
              this.isRunning = false;
              if (err.status == 403) {
                this.loginExsiste = true;
              }
              else {
                this.loginExsiste = false;
              }
              if (err.error instanceof Error) {
                console.log("Client-side Error occured");
              } else {
                console.log("Server-side Error occured");
              }
              $('#userCompteModal').modal('show');
            });
      } else {
        this.saveUserImage();
        this.isRunning = true;
        this.userservice.updateUser(this.user)
          .subscribe(
            modifiedUser => {
              this.isRunning = false;
              this.success = true;
              console.log(modifiedUser);
              modifiedUser.confirmPassword = modifiedUser.password;
              modifiedUser.birthDay = new Date(modifiedUser.birthDay);
              this.userservice.currentUser.next(modifiedUser);
              $('#userCompteModal').modal('show');
              // $('#userModal').modal('hide');
              $('#userCompteModal').on('hidden.bs.modal', () => {
                //this._router.navigateByUrl("/");
              });
              this.focusOnVisibleModal();
            }, (err: HttpErrorResponse) => {
              this.isRunning = false;
              if (err.status == 403) {
                this.loginExsiste = true;
              }
              else {
                this.loginExsiste = false;
              }
              if (err.error instanceof Error) {
                console.log("Client-side Error occured");
              } else {
                console.log("Server-side Error occured");
              }
              $('#userCompteModal').modal('show');
            });
      }
    } else {
      this._commonService.scrollTopModal("userModal");
    }

  }
  focusOnVisibleModal() {
    $('body').on('hidden.bs.modal', function () {
      if ($('.modal.in').length > 0) {
        $('body').addClass('modal-open');
      }
    });
  }
  ngOnInit() {
    this.authService.authState.subscribe((googleData) => {
      this.googleData = googleData;
    });
    this.isSubmitClicked = false;
    this.minBirthday = new Date(1900, 1, 1);
    this.maxBirthday = new Date(2003, 0, 1);
    let colorTheme = 'theme-dark-blue';
    this.bsConfig = Object.assign({}, { containerClass: colorTheme });
    var image: any = new Image();
    var oldImage: any = new Image();
    this.newPicture = {
      img: image,
      oldImage: oldImage
    };
    if (this.editMode == 'new') {
      this.user = new User();
      //this.user.address = new Address();
      this.user.gender = this.genders[0].value;
    } else {
      this.userReset = Object.assign({}, this.user);
      this.userservice.fetchImageContent(this.user.id).subscribe(data => {
        if (data != null) {
          let src = "data:image/jpg;base64," + data;
          image.src = src;
          this.newPicture.img.image = this._sanitizer.bypassSecurityTrustResourceUrl(src);
          this.newPicture.oldImage.src = src;
        }
      })
      this.user.confirmPassword = this.user.password;
      this.user.birthDay = new Date(this.user.birthDay);

    }

  }

  openUserModal(editMode: string, user?: User) {
    this.editMode = editMode;
    if (user != null) {
      this.user = user;
      this.ngOnInit();
    }
    $('#userModal').modal('show');
    $('#userModal').on('hidden.bs.modal', () => {
      // this.ngOnInit();
      this.submitForm = false;
      this.success = false;
      this.formInvalid = false;
      if (this.editMode == "new") {
        this.ngOnInit();
      } else if (!this.isSubmitClicked) {
        this.resetUser();
      }
      this.submitForm = false;
      this.success = false;
      this.formInvalid = false;
      this.checkRecaptchaClicked = false;
      this.captchaResponse = false;
      this.recaptchaRef.reset();
    })
  }

  clear() {
    this.userservice.currentUser.next(null);
    this.editMode = "new";
    this.ngOnInit();
  }
  hideUserModal() {
    $('#userModal').modal('hide');
    this.focusOnVisibleModal();
  }


  resetCropper() {
    this.cropper.reset();
  }

  closeCropModal() {
    $(this.cropModal.nativeElement).modal('hide');
    $('#CropImage').on('hidden.bs.modal', () => {
      this.isMaxSize = false;
      this.isNotImage = false;
      this.focusOnVisibleModal();
    })
  }

  formatSizeUnits(bytes) {
    if (bytes >= 1073741824) { bytes = (bytes / 1073741824).toFixed(2) + ' GB'; }
    else if (bytes >= 1048576) { bytes = (bytes / 1048576).toFixed(2) + ' MB'; }
    else if (bytes >= 1024) { bytes = (bytes / 1024).toFixed(2) + ' KB'; }
    else if (bytes > 1) { bytes = bytes + ' bytes'; }
    else if (bytes == 1) { bytes = bytes + ' byte'; }
    else { bytes = '0 byte'; }
    return bytes;
  }

  checkValidImage(file: File) {
    return !(file.type.indexOf('image') === -1);
  }

  fileChangeListener($event) {
    this.isMaxSize = false;
    this.isNotImage = false;
    var file: File = $event.target.files[0];
    if (!this.checkValidImage(file)) {
      this.isNotImage = true;
      $event.target.value = "";
      return;
    }
    if (file.size >= Constants.FileUploadMaxSize) {
      this.isMaxSize = true;
      this.isNotImage = !this.checkValidImage(file);
      $event.target.value = "";
      return;
    }
    var myReader: FileReader = new FileReader();
    var that = this;
    myReader.onloadend = function (loadEvent: any) {
      that.newPicture.img.src = loadEvent.target.result;
      that.cropper.setImage(that.newPicture.img);

    };
    $event.target.value = "";
    myReader.readAsDataURL(file);
  }

  savePicture(pict: any) {
    this.newPicture.img.image = pict.image;
    this.newPicture.oldImage.src = this.newPicture.img.image
    this.newPicture.isDefault = true;

    this.data = {};
    this.closeCropModal();
  }

  deleteImage() {
    var image: any = new Image();
    this.newPicture = {
      img: image,
      oldImage: image
    };
  }

  getModified() {
    this.cropper.setImage(this.newPicture.oldImage);
  }

  saveUserImage() {
    if (this.editMode == "new") {
      this.user.userImage = new UserImage();
      if (this.newPicture.img.image != null) {
        let image = this.newPicture.img.image.toString().split(',')[1].split(' ')[0];
        this.user.userImage.content = image;
        this.user.userImage.isDefault = false;
      } else {
        this.user.userImage = null;
      }
    } else {
      if (this.user.provider == 0) {
        this.user.userImage = new UserImage();
        if (this.newPicture.img.image != null) {
          let image = this.newPicture.img.image.toString().split(',')[1].split(' ')[0];
          this.user.userImage.content = image;
          this.user.userImage.isDefault = false;
        } else {
          this.user.userImage = null;
        }
      }
    }
  }

  // getCountries() {
  //   this._http.get('../assets/i18n/countries.json').subscribe(data => {
  //     this.countries = data;
  //     if (this.editMode == 'new') {
  //       this.user.address.country = this.countries[0].name;
  //     } else {
  //       this.user.address.country = this.user.address.country;
  //     }
  //   });
  // }

  resetUser() {
    this.user.firstName = this.userReset.firstName;
    this.user.lastName = this.userReset.lastName;
    this.user.email = this.userReset.email;
    this.user.password = this.userReset.password;
    this.user.confirmPassword = this.userReset.confirmPassword;
    this.userservice.currentUser.next(this.userReset);
    //this._commonService.redirectToHomePage();
  }

  resolved(captchaResponseToken: string) {
    this.checkRecaptchaClicked = true;
    this.userservice.getRecaptchaResponse(captchaResponseToken).subscribe(
      (captchaResponse) => {
        this.captchaResponse = captchaResponse;
      }
    )
  }

  signInWithGoogle(): void {
    this.authService.signIn(GoogleLoginProvider.PROVIDER_ID).then(
      (googledata) => {
        if (this.googleData != undefined || this.googleData != null) {
          this.user.email = this.googleData.email;
          this.user.firstName = this.googleData.firstName;
          this.user.lastName = this.googleData.lastName;
          this.user.provider = ProviderEnum.Google;
          this.user.login = this.googleData.email;
          this.user.userImage = new UserImage();
          this.user.userImage.path = this.googleData.photoUrl;
          this.createUserGmail();
        }
      })
  }

  signOut(): void {
    this.authService.signOut();
  }

  createUserWithGoogle() {
    this.signInWithGoogle();

  }

  createUserGmail() {
    this.isRunning = true;
    this.userservice.createUserWithGoogle(this.user).subscribe(
      (res) => {
        this.isRunning = false;
        this.success = true;
        this.googleData = null;
        $('#userCompteModal').modal('show');
        $('#userCompteModal').on('hidden.bs.modal', () => {
          this._router.navigateByUrl("/");
        });
        this.focusOnVisibleModal();
      }, (err: HttpErrorResponse) => {
        this.isRunning = false;
        if (err.status == 403) {
          this.loginExsiste = true;
        }
        else {
          this.loginExsiste = false;
        }
        this.googleData = null;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
        $('#userCompteModal').modal('show');
      });
  }

  UserTypeChange(type: UserTypeEnum) {
    //   this.ngOnInit();
    this.usertype = type;
  }










}


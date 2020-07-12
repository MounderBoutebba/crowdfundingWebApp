import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Constants } from './../../../constants';
import { User } from './../../../models/user';
import { UserService } from './../../../services/userService';
import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { LocalStorageService } from '../../../services/local-storage.service';
import { UserLoginComponent } from '../../loginPage/user-login/user-login.component';
import { ProjectService } from '../../../services/projectService';
import { UserCheckTokenDto } from '../../../models/userCheckTokenDto';
import { UserFormComponent } from '../../../shared/user-form/user-form.component';
import { Bill } from '../../../models/bill';
import { DomSanitizer } from '@angular/platform-browser';

declare var $;

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent implements OnInit {

  emailSent: boolean = false;
  mailInvalid: boolean = false;
  sendForm: boolean = false;
  email: string;
  success: boolean = false;
  serverError: boolean = false;
  constants = Constants;
  WebsiteName = 'Coiner';
  currentUser: User;
  displayAllProjectsRoute: boolean;
  displayAllProductsRoute: boolean;
  formInvalid: boolean = false;
  formInputsInvalide = false;
  isRunning: boolean = false;
  bills: Bill[];
  pdfSrc: any;
  billsPath = this.constants.baseUrlServer + this.constants.BillsSharedPath;

  @ViewChild('loginModal') loginModal: UserLoginComponent;
  @ViewChild('userModal') userModal: UserFormComponent;

  constructor(private _userService: UserService, private _router: Router,
    public translate: TranslateService,
    private _localStorage: LocalStorageService,
    public _projectService: ProjectService,
    private sanitizer: DomSanitizer) {
    let user = _localStorage.getData('user');
    if (user != null) {
      var userCheckDto = new UserCheckTokenDto();
      userCheckDto.userId = user.id;
      userCheckDto.token = user.token;
      this._userService.getUser(userCheckDto).subscribe(
        (user: any) => {
          if (user == null) {
            this.logoutUser();
          } else {
            this._userService.adminLogin.next(user.adminLogin);
            this._userService.currentUser.next(user.currentUser);
          }
        },
        (err) => {
          this.logoutUser();
        }
      );
    }
    this._userService.currentUser.subscribe(
      (currentUser) => {
        this.currentUser = currentUser;
      }
    );
    this._projectService.projectCarouselsCount.subscribe(
      (projectsCount) => {
        this.displayAllProjectsRoute = (projectsCount >= Constants.MaxCarouselProjectsNumber)
      }
    )

    this._projectService.productCarouselsCount.subscribe(
      (productsCount) => {
        this.displayAllProductsRoute = (productsCount >= Constants.MaxCarouselProjectsNumber)
      }
    )
  }

  ngOnInit() {
  }

  checkConnectedUser() {
    if (this.currentUser != null) {
      this._router.navigateByUrl('creer-projet');
    } else {
      this.loginModal.openLoginModal();
    }
    this.collapseMenu();
  }

  openUserModal(editMode: string, user?: User) {
    this.userModal.openUserModal(editMode, user);
  }

  openLoginModal() {
    this.loginModal.openLoginModal();
  }

  logoutUser() {
    // this.userModal.clear();
    this.collapseMenu();
    this._userService.currentUser.next(null);
    this._userService.adminLogin.next(null);
    this._userService.clearLocalUser()
    this._router.navigateByUrl("/");
  }

  demandNewPassword(form: NgForm) {
    this.isRunning = true;
    this.sendForm = true;
    this.formInputsInvalide = form.invalid;
    this.formInvalid = this.formInputsInvalide;
    if (!this.formInvalid) {
      this._userService.sendEmailToRenewPassword(this.email).subscribe((res) => {
        this.emailSent = true;
        this.success = true;
        this.isRunning = false;
        $('#DemandNewPasswordModel').on('hidden.bs.modal', () => {
          this._router.navigateByUrl("/");
        })
      }, (err) => {
        this.isRunning = false;
        this.success = false;
        if (err.status == 403) {
          this.mailInvalid = true;
          this.serverError = false;
        } else {
          this.mailInvalid = false;
          this.serverError = true;
        }
      });
    } else {
      this.isRunning = false;
    }
  }

  collapseMenu() {
    $(".navbar-collapse").collapse('hide');
  }

  onCloseModal() {
    $('#DemandNewPasswordModel').on('data-dismiss', () => { })
    this.sendForm = false;
    this.emailSent = false;
    this.mailInvalid = false;
    this.sendForm = false;
    this.success = false;
    this.serverError = false;
    this.formInvalid = false;
  }

  openBillsModal() {
    this.isRunning = true;
    this._projectService.GetUserBills(this.currentUser.id).subscribe(
      (bills) => {
        this.isRunning = false;
        $('#MyBills').modal('show');
        this.bills = bills;
        bills.forEach(bill => {
          var blob = this.base64toBlob(bill.content, 'application/pdf');
          var pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(window.URL.createObjectURL(blob));
          bill.content = pdfUrl;
        });
      },
      (err) => {
        this.isRunning = false;
      }
    )
  }

  base64toBlob(base64Data, contentType) {
    contentType = contentType || '';
    var sliceSize = 1024;
    var byteCharacters = atob(base64Data);
    var bytesLength = byteCharacters.length;
    var slicesCount = Math.ceil(bytesLength / sliceSize);
    var byteArrays = new Array(slicesCount);

    for (var sliceIndex = 0; sliceIndex < slicesCount; ++sliceIndex) {
      var begin = sliceIndex * sliceSize;
      var end = Math.min(begin + sliceSize, bytesLength);

      var bytes = new Array(end - begin);
      for (var offset = begin, i = 0; offset < end; ++i, ++offset) {
        bytes[i] = byteCharacters[offset].charCodeAt(0);
      }
      byteArrays[sliceIndex] = new Uint8Array(bytes);
    }
    return new Blob(byteArrays, { type: contentType });
  }

  displayPdfInViewer(pdfPath: string) {
    this.pdfSrc = this.billsPath + pdfPath;
    $('#MyBills').modal('hide');
    $('#BillViewer').modal('show');
    $('#BillViewer').on('hidden.bs.modal', () => {
      $('#MyBills').modal('show');
    })
  }

}

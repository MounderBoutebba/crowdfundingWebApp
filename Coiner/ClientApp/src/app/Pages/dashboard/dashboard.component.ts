import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from '../../services/commonService';
import { LocalStorageService } from '../../services/local-storage.service';
import { User } from '../../models/user';
import { UserService } from '../../services/userService';
import { UserNotificationsComponent } from '../user-notifications/user-notifications.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  @ViewChild('UserNotificationsComponent') userNotificationsComponent: UserNotificationsComponent;

  currentUser: User;
  token: string;
  notificationNb: number;
  isDashboardClicked: boolean = true;
  isWalletClicked: boolean = false;
  isProjectClicked: boolean = false;
  isStatClicked: boolean = false;
  isProfileClicked: boolean = false;
  isNotificationClicked: boolean = false;
  isFavClicked: boolean = false;

  sub: any;
  page: string;

  constructor(private _route: ActivatedRoute,
    public _commonService: CommonService,
    public _userService: UserService,
    private _localStorageServicve: LocalStorageService,
    private _router: Router) {
    let user = _route.snapshot.data.currentUser;

    if (user == null) {
      this._router.navigateByUrl('/');
    } else {
      this.currentUser = user.currentUser;
      this.token = this._localStorageServicve.getData('user').token;
    }
  }

  ngOnInit() {
    this._commonService.scroll();
    this.notificationNb = this.userNotificationsComponent.notificationNb;
    this.checkPage();
  }

  isClicked() {
    this.isDashboardClicked = false;
    this.isWalletClicked = false;
    this.isProjectClicked = false;
    this.isStatClicked = false;
    this.isProfileClicked = false;
    this.isNotificationClicked = false;
    this.isFavClicked = false;
  }

  DashboardClicked() {
    this.isClicked();
    this.isDashboardClicked = true;
  }

  FavClicked() {
    this.isClicked();
    this.isFavClicked = true;
  }

  WalletClicked() {
    this.isClicked();
    this.isWalletClicked = true;
  }

  ProjectClicked() {
    this.isClicked();
    this.isProjectClicked = true;
  }

  StatClicked() {
    this.isClicked();
    this.isStatClicked = true;
  }

  ProfileClicked() {
    this.isClicked();
    this.isProfileClicked = true;
  }

  NotificationClicked() {
    this.isClicked();
    this.isNotificationClicked = true;
  }

  ngOnDestroy() {
    if(this.currentUser != null)
    this.sub.unsubscribe();
  }

  changePage(page: string) {
    this._router.navigate(['dashboard'], { queryParams: { page: page } });
  }

  checkPage() {
    this.sub = this._route
      .queryParams
      .subscribe(params => {
        this.page = params['page'] || 'dashboard';
        switch (this.page) {
          case 'dashboard':
            this.DashboardClicked();
            break;
          case 'profile':
            this.ProfileClicked();
            break;
          case 'wallet':
            this.WalletClicked();
            break;
          case 'favorites':
            this.FavClicked();
            break;
          case 'myProjects':
            this.ProjectClicked();
            break;
          case 'stats':
            this.StatClicked();
            break;
          case 'notifications':
            this.NotificationClicked();
            break;
        }
      });
  }
}

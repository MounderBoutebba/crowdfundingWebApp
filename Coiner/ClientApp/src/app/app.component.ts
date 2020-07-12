import { SocketService } from './services/socket-service';
import { ProjectService } from './services/projectService';
import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from './services/commonService';
import { UserService } from './services/userService';
import { Router } from '@angular/router';

declare var $: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  currentUser: any;
  Year: number;

  constructor(public translate: TranslateService, private _commonservice: CommonService,
    private _userService: UserService,
    private _sockerService: SocketService,
    private _projectServicve: ProjectService,
    private _router: Router) {
    var browserLang = translate.getBrowserLang();
    let defaultLang = (browserLang != undefined) ?
      browserLang :
      'fr';
    translate.addLangs(['en', 'fr']);
    translate.setDefaultLang(defaultLang);
    translate.use(defaultLang);
    this._commonservice.checkBrowserSupported();
    this._userService.currentUser.subscribe(
      (currentUser) => {
        this.currentUser = currentUser;
      }
    )
    this._projectServicve.getLatestProjectsList()
      .subscribe(
        (data) => {
          this._projectServicve.projectCarouselsCount.next(data[1]);
        });
    this._sockerService.hubconnection.start()
      .then(() => { "Connection started" })
      .catch(err => { console.error(err) });
    this.Year = (new Date()).getFullYear();
  }

  checkConnectedUser() {
    if (this.currentUser != null) {
      this._router.navigateByUrl('creer-projet');
    } else {
      $('#loginModal').modal('show');
    }
  }
}
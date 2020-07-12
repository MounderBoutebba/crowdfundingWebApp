import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { User } from '../../../models/user';
import { Constants } from './../../../constants';
import { NavBarComponent } from '../../homePage/nav-bar/nav-bar.component';
import { UserFormComponent } from '../../../shared/user-form/user-form.component';
import { Router, ActivatedRoute } from '@angular/router';
import { LocalStorageService } from '../../../services/local-storage.service';


@Component({
  selector: 'user-profile-details',
  templateUrl: './user-profile-details.component.html',
  styleUrls: ['./user-profile-details.component.css']
})
export class UserProfileDetailsComponent implements OnInit {

  @Input() user: User;
  constants = Constants;

  @ViewChild('userModal') userModal: UserFormComponent;

  constructor(private _router: Router,
    private _route: ActivatedRoute,
    private _localStorage: LocalStorageService, ) {
    let user = _route.snapshot.data.currentUser;
    if (user == null) {
      this._router.navigateByUrl('/');
    } else {
      this.user = user.currentUser;
    }
  }

  ngOnInit() {
  }

  openUserModal() {
    this.userModal.openUserModal('edit', this.user);
  }
}

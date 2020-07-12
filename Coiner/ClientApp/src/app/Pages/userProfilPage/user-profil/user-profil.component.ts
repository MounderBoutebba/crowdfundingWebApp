import { User } from './../../../models/user';
import { UserService } from './../../../services/userService';
import { Component, OnInit, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'user-profil',
  templateUrl: './user-profil.component.html',
  styleUrls: ['./user-profil.component.css']
})
export class UserProfilComponent implements OnInit {
  user: User;
  constructor(private _userService: UserService,
    private _route: ActivatedRoute,
    private _router: Router) {
    this.user = _route.snapshot.data.currentUser;
      if (this.user == null) {
        this._userService.currentUser.next(null);
        this._userService.clearLocalUser()
        this._router.navigateByUrl('/');
      }
  }
  ngOnInit() {
  }

}

import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/userService';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/user';

@Component({
  selector: 'user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.css']
})
export class UserPageComponent implements OnInit {
  user:any;
  constructor(private _userService: UserService,
    private _route: ActivatedRoute,
    private _router: Router) {
      this.user = _route.snapshot.data.currentUser;
      if (this.user != null) {
        this._router.navigateByUrl('/');
      }
  }

  ngOnInit() {
  }
}

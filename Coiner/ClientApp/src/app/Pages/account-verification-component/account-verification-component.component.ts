import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-account-verification-component',
  templateUrl: './account-verification-component.component.html',
  styleUrls: ['./account-verification-component.component.css']
})
export class AccountVerificationComponent implements OnInit {
state :string;
  constructor(private _route: ActivatedRoute) { }

  ngOnInit() {
    this._route.params.subscribe(params => {
      this.state =params['state'];
    });

  }

}

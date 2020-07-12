import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { UserService } from '../services/userService';
import { Router } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router/src/router_state';
import { Observable } from 'rxjs';
import { LocalStorageService } from '../services/local-storage.service';

@Injectable()
export class OnlyLoggedInUsersGuard implements CanActivate {

    public loggedInUser: any = false;

    constructor(private _userService: UserService,
        private _router: Router,
        private _localStorageServicve: LocalStorageService) {
        console.log("OnlyLoggedInUsers");
        let user = this._localStorageServicve.getData('user');
        if (user != null) {
            this.loggedInUser = true;
        } else {
            this._router.navigateByUrl('/');
            this.loggedInUser = false;
        }
    };

    canActivate(rout: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Observable<boolean> {
        return this.loggedInUser;
    }
}
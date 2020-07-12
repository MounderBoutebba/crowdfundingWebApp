import { Injectable } from '@angular/core';

import { Resolve, Router } from '@angular/router';

import { Observable } from 'rxjs';


import { ProjectService } from '../services/projectService';
import { User } from '../models/user';
import { UserService } from '../services/userService';
import { LocalStorageService } from '../services/local-storage.service';
import { UserCheckTokenDto } from '../models/userCheckTokenDto';

@Injectable()
export class UserProfilResolver implements Resolve<any> {
    currentUser: User;
    constructor(private _userService: UserService,
        private _router: Router,
        private _localStorage: LocalStorageService) { }

    resolve() {
        let user = this._localStorage.getData('user');
        if (user != null) {
            var userCheckDto = new UserCheckTokenDto();
            userCheckDto.userId = user.id;
            userCheckDto.token = user.token;
            return this._userService.getUser(userCheckDto);
        }
    }
}
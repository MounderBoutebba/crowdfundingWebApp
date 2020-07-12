import { BehaviorSubject } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user'
import { Constants } from '../constants';
import { LocalStorageService } from './local-storage.service';
import { UserCheckTokenDto } from '../models/userCheckTokenDto';
import { ContactUsDto } from '../models/contactUsDto';
import { CreditCurrencyDto } from '../models/creditCurrencyDto';

@Injectable()
export class UserService {

    constants = Constants;
    readonly apiBasePath = "api/Users/";
    public currentUser = new BehaviorSubject<(User | any)>(null);
    public adminLogin = new BehaviorSubject<string>(null);


    constructor(private _http: HttpClient,
        private _localStorage: LocalStorageService) {
    }

    createUser(user: User) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "CreateUser";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(user);
        return this._http.post(url, data, { headers: headers })
    }

    updateUser(user: User) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "UpdateUser";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(user);
        return this._http.post<User>(url, data, { headers: headers })
    }

    getUser(userCheckDto: UserCheckTokenDto) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "GetUser";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(userCheckDto);
        return this._http.post<any>(url, data, { headers: headers })
    }

    loginUser(userlogin: string, userpassword: string) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "LoginUser/" + userlogin + "/" + userpassword;
        return this._http.get<(User | any)[]>(url)
    }

    // local storage methods
    setLocalUser(userId: number, login: string,
        firstName: string, lastName: string,
        token: string) {
        if (login == null) {
            return;
        }
        var user = new Object({
            id: userId, login: login, firstName: firstName,
            lastName: lastName, token: token
        });
        this._localStorage.setData("user", user);
    }

    clearLocalUser() {
        this._localStorage.removeData("user");
    }

    fetchImageContent(userId: number) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "FetchImageContent/" + userId;
        return this._http.get(url)
    }

    sendEmailToRenewPassword(email: string) {
        let emailEncoded = btoa(email);
        var url = Constants.baseUrlServer + this.apiBasePath + "SendEmailToUpdatePassword/" + emailEncoded;
        return this._http.get(url)
    }

    updateUserPassword(id: string, pw: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "UpdateUserPassword/" + id + "/" + pw;
        return this._http.get(url)
    }

    getRecaptchaResponse(captchaResponseToken: string) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "GetRecaptchaResponse";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(captchaResponseToken);
        return this._http.post<any>(url, data, { headers: headers })
    }

    SendEditProjectRequest(contactUsDto: ContactUsDto, projectName: String) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "SendEditProjectRequest/" + projectName;
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(contactUsDto);
        return this._http.post<any>(url, data, { headers: headers })
    }
    SendAddNewsRequest(contactUsDto: ContactUsDto, projectName: String) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "SendAddNewsRequest/" + projectName;
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(contactUsDto);
        return this._http.post<any>(url, data, { headers: headers })
    }
    SendMessageFromContactUs(contactUsDto: ContactUsDto) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "SendMessageFromContactUs";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(contactUsDto);
        return this._http.post<any>(url, data, { headers: headers })
    }
    createUserWithGoogle(user: User) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "CreateUserWithGoogle";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(user);
        return this._http.post(url, data, { headers: headers })
    }

    loginUserWithGogle(email: string) {
        var url = this.constants.baseUrlServer + this.apiBasePath + "LoginUserWithGogle/" + email;
        return this._http.get<(User | any)[]>(url)
    }


    getUserNotifications(userId: number, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserNotifications/" + userId;
        let headers = new HttpHeaders({ 'Authorization': 'bearer ' + token });
        return this._http.get(url, { headers: headers })
    }

    UpdateNotificationStatus(notifId: number, userId: number, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "UpdateNotificationStatus/" + notifId + "/" + userId;
        let headers = new HttpHeaders({ 'Authorization': 'bearer ' + token });
        return this._http.get(url, { headers: headers })
    }
    updateKycNotification(id: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "updateKycNotification/" + id;
        return this._http.get(url)
    }
    AddCoinsToUser(creditCurrencyDto: CreditCurrencyDto, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "AddCoinsToUser";
        let headers: HttpHeaders = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');
        headers = headers.append('Authorization', 'bearer ' + token);
        var data = JSON.stringify(creditCurrencyDto);
        return this._http.post(url, data, { headers: headers })
    }



}

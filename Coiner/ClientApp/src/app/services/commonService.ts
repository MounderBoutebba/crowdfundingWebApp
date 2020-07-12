import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Injectable } from "@angular/core";
import { ProjectStatusEnum } from "../models/enums/projectStatusEnum";
import { ProjectTypeEnum } from "../models/enums/projectTypeEnum";
import { ActivityTypeEnum } from "../models/enums/activityTypeEnum";
declare var $;

@Injectable()
export class CommonService {
    keys = { 37: 1, 38: 1, 39: 1, 40: 1 };
    scrollDisabled: boolean = false;
    activityTypes: any;

    constructor(private _translateService: TranslateService,
        private _router: Router) {
        this._translateService.onLangChange.subscribe(() => {
            this.getActivityTypesTranslation();
        })
    }

    getDays(beginEstimatedDate) {
        if (beginEstimatedDate != null) {
            if (beginEstimatedDate.getTime() - new Date().getTime() < 0) {
                return 0;
            }
            var timeDiff = Math.abs(beginEstimatedDate.getTime() - new Date().getTime());
            var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        } else {
            return 0;
        }
        return diffDays;
    }

    getProjectTypeName(value: number) {
        let projectType: string;
        switch (value) {
            case 0:
                projectType = 'Projet'
                break;
            case 1:
                projectType = 'Société'
                break;
            case 2:
                projectType = 'Parcours'
                break;
            case 3:
                projectType = 'Un Bien'
                break;
        }
        return projectType;
    }

    getProjectActitivityField(value: number) {
        let activityType: string;
        switch (value) {
            case 0:
                activityType = 'Environnement'
                break;
            case 1:
                activityType = 'Industrie & Service'
                break;
            case 2:
                activityType = 'Santé'
                break;
            case 3:
                activityType = 'Numérique'
                break;
            case 4:
                activityType = 'Promotion immobilière'
                break;
            case 5:
                activityType = 'Commerce'
                break;
        }
        return activityType;
    }

    getProjectValidationStatusString(projectStatusEnum: ProjectStatusEnum) {
        switch (projectStatusEnum) {
            case ProjectStatusEnum.Initialized:
                return 'Initialized';
            case ProjectStatusEnum.Accepted:
                return 'Accepted';
            case ProjectStatusEnum.EndSurvey:
                return 'EndSurvey';
            case ProjectStatusEnum.Refused:
                return 'Refused';
            case ProjectStatusEnum.Refused:
                return 'Validated';
        }
    }

    getProjectTypeNameString(projectType: ProjectTypeEnum) {
        switch (projectType) {
            case ProjectTypeEnum.Project:
                return 'Project';
            case ProjectTypeEnum.Society:
                return 'Society';
            case ProjectTypeEnum.Career:
                return 'Career';
            case ProjectTypeEnum.Product:
                return 'Product';
        }
    }

    getProjectActitivityTypeString(activityType: ActivityTypeEnum) {
        switch (activityType) {
            case ActivityTypeEnum.Environment:
                return 'Environment';
            case ActivityTypeEnum.IndustryAndService:
                return 'IndustryAndService';
            case ActivityTypeEnum.Health:
                return 'Health';
            case ActivityTypeEnum.Digital:
                return 'Product';
            case ActivityTypeEnum.RealEstateDevelopment:
                return 'RealEstateDevelopment';
            case ActivityTypeEnum.Trade:
                return 'Trade';
        }
    }

    getActivityTypeString(activityType: string) {
        for (let act of this.activityTypes) {
            if (act.value == activityType) {
                return act.name;
            }
        }
    }

    scrollTop() {
        // let body = $("html, body");
        let body = $(".scrollable");
        body.stop().animate({ scrollTop: 0 }, 500, 'swing');
    }

    scroll() {
        let body = $(".scrollable");
        body.stop().animate({ scrollTop: 0 }, 0, 'swing');
    }

    scrollTopModal(modalId: string) {
        $('#' + modalId).stop().animate({ scrollTop: 0 }, 500, 'swing');
    }

    preventDefault(e) {
        e = e || window.event;
        if (e.preventDefault)
            e.preventDefault();
        e.returnValue = false;
    }
    preventDefaultForScrollKeys(e) {
        if (this.keys[e.keyCode]) {
            this.preventDefault(e);
            return false;
        }
    }
    disableScroll() {
        if (window.addEventListener) // older FF
            window.addEventListener('DOMMouseScroll', this.preventDefault, false);
        window.onwheel = this.preventDefault; // modern standard
        window.onmousewheel = window.onmousewheel = this.preventDefault; // older browsers, IE
        window.ontouchmove = this.preventDefault; // mobile
        document.onkeydown = this.preventDefaultForScrollKeys;
    }

    enableScroll() {
        if (window.removeEventListener)
            window.removeEventListener('DOMMouseScroll', this.preventDefault, false);
        window.onmousewheel = window.onmousewheel = null;
        window.onwheel = null;
        window.ontouchmove = null;
        document.onkeydown = null;
    }

    detectIE() {
        let ua = window.navigator.userAgent;
        let msie = ua.indexOf('MSIE ');
        if (msie > 0) {
            // IE 10 or older => return version number
            return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
        }
        let trident = ua.indexOf('Trident/');
        if (trident > 0) {
            // IE 11 => return version number
            let rv = ua.indexOf('rv:');
            return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
        }
        let edge = ua.indexOf('Edge/');
        if (edge > 0) {
            // IE 12 (aka Edge) => return version number
            return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
        }
        // other browser
        return false;
    };

    detectEdge() {
        let ua = window.navigator.userAgent;
        let edge = ua.indexOf('Edge/');
        if (edge > 0) {
            // IE 12 (aka Edge) => return version number
            return true;
        }
    }

    showAlert(msg: string) {
        window.alert(msg);
    }

    checkBrowserSupported() {
        let ieVersion = this.detectIE();
        if (ieVersion && ieVersion < 10) {
            this._translateService.get('BrowserSupport.IEWarning').subscribe(
                (msg) => {
                    this.showAlert(msg);
                }
            );
        }
    }

    redirectToHomePage() {
        this._router.navigateByUrl('/');
        this.scrollTop();
    }

    orderBy(property: any, desc: string) {
        if (desc == "desc") {
            if (property != null && property.length > 0) {
                property.forEach((prop) => {
                    property.sort((propA: any, propB: any) => {
                        if (propA.creationDate > propB.creationDate) return -1;
                        if (propA.creationDate < propB.creationDate) return 1;
                        return 0;
                    });
                });
            }
        } else {
            if (property != null && property.length > 0) {
                property.forEach((prop) => {
                    property.sort((propA: any, propB: any) => {
                        if (propA.creationDate < propB.creationDate) return -1;
                        if (propA.creationDate > propB.creationDate) return 1;
                        return 0;
                    });
                });
            }
        }
    }

    getActivityTypesTranslation() {
        this._translateService.get('AllActivityTypesCreation').subscribe(
            (data) => {
                this.activityTypes = data;
            }
        );
    }

    getProgressionPercentage(value1: number, value2: number) {
        var percentage = (value1 * 100 / value2 < 100) ? value1 * 100 / value2 : 100;
        return (Math.round(percentage))
    }
}

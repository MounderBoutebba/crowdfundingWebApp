import { Component, OnInit, Renderer, Input } from '@angular/core';
import { NotificationProduced } from '../../models/NotificationProduced';
import { UserService } from '../../services/userService';
import { Router, ActivatedRoute } from '../../../../node_modules/@angular/router';
import { LocalStorageService } from '../../services/local-storage.service';
import { User } from '../../models/user';
import {animate, query, stagger, style, transition, trigger, animateChild} from '@angular/animations';
import { CommonService } from '../../services/commonService';
@Component({
  selector: 'user-notifications',
  templateUrl: './user-notifications.component.html',
  animations: [
    trigger('listStagger', [
      transition('* <=> *', [
        query(
          ':enter',
          [
            style({ opacity: 0, transform: 'translateY(-15px)' }),
            stagger(
              '50ms',
              animate(
                '550ms ease-out',
                style({ opacity: 1, transform: 'translateY(0px)' })
              )
            )
          ],
          { optional: true }
        ),
        query(':leave', animate('50ms', style({ opacity: 0 })), {
          optional: true
        })
      ])
    ])
  ]
  
  
  
  ,
  styleUrls: ['./user-notifications.component.css']
})
export class UserNotificationsComponent implements OnInit {
  @Input() isNotificationClicked:boolean;
  notificationProduceds: NotificationProduced[] = [];
  currentUser: User;
  token: string;
  isRunning:boolean=false;
  notificationNb: number;
  
  constructor(
    private _userService: UserService,
    private _localStorageService: LocalStorageService,
    private _route: ActivatedRoute,
    public _commonService: CommonService,
    private _router: Router, private renderer: Renderer) {

    let user = _route.snapshot.data.currentUser;

    if (user == null) {
      this._router.navigateByUrl('/');
    } else {
      this.currentUser = user.currentUser;
      this.token = this._localStorageService.getData('user').token;
    }
    this._commonService.scroll();
  }

  ngOnInit() {
    this.getUserNotifications();
    this.token =this._localStorageService.getData('user').token;
  }

  getUserNotifications(){

    this._userService.getUserNotifications(this.currentUser.id,this.token).subscribe(
      (data:any) => {
        this.notificationNb = data.notificationsCount;
        var notifications = data.notifications;
        for(let i=0; i<notifications.length; i++){
            var notification = new NotificationProduced;
            notification.Id=notifications[i].id;
            notification.Content=notifications[i].content;
            notification.CreateDate=notifications[i].createDate;
            notification.ProjectId=notifications[i].projectId;
            notification.Title=notifications[i].title;
            notification.UserId=notifications[i].userId;
            notification.ReadStatus=notifications[i].appReadStatus;
            this.notificationProduceds.push(notification);     
              }}
      
    ) 
  }
  updateNotificationStatus(notificationId){
    this._userService.UpdateNotificationStatus(notificationId,this.currentUser.id,this.token).subscribe(
      (data:any) => {
        this.notificationNb = data.notificationsCount;
        
        for(var i in this.notificationProduceds){
          if (this.notificationProduceds[i].Id == notificationId) {
            this.notificationProduceds[i].ReadStatus=true;
            break; 
         }
         // change the readStatus without calling the server again (for smoothe animations)
         // this.getUserNotifications();
        }
      } 
    ) 
  }
  





}

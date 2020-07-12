import { Component, OnInit,Input, Output, EventEmitter, } from '@angular/core';
import { NotificationProduced } from '../../models/NotificationProduced';



@Component({
  selector: 'user-notification-listing-item',
  templateUrl: './user-notification-listing-item.component.html',
  styleUrls: ['./user-notification-listing-item.component.css']
  
})
export class UserNotificationListingItemComponent implements OnInit {
  @Input() notificationItem: NotificationProduced;
  @Input() isNotificationClicked:boolean;
  @Input() read: boolean=false;
  @Output() notificationIsRead: EventEmitter<any> = new EventEmitter();
  showContent:boolean=false;
  constructor() { }

  ngOnInit() {
  } 

  
  updateNotificationStatus(){
    this.showContent=!this.showContent;
   if (this.read==false) this.notificationIsRead.emit(this.notificationItem.Id);
    this.read = true;
  }
}

import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ProductOffer } from '../../models/productOffer';
import { User } from '../../models/user';

declare var $;

@Component({
  selector: 'buy-sell-book',
  templateUrl: './buy-sell-book.component.html',
  styleUrls: ['./buy-sell-book.component.css']
})
export class BuySellBookComponent implements OnInit {
  @Input() isBetterPrice: boolean;
  @Input() productOffersBuy: ProductOffer[];
  @Input() productOffersSell: ProductOffer[];
  @Input() currentUser: User;
  @Input() displayType: string; //all,sell,buy
  @Output() acceptOffer: EventEmitter<any> = new EventEmitter();
  constructor() { }

  ngOnInit() {
  }

  showAcceptOfferModal(productOffer: ProductOffer) {
    if (this.currentUser != null) {
      $("#acceptOfferModal").modal('show');
      this.acceptOffer.emit(productOffer);
    } else {
      $("#loginModal").modal('show');
    }
  }

  showCancelOfferModal(productOffer: ProductOffer) {
    if (this.currentUser != null) {
      $("#cancelOfferModal").modal('show');
      this.acceptOffer.emit(productOffer);
    } else {
      $("#loginModal").modal('show');
    }
  }
}

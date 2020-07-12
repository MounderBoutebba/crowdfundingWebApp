import { Component, OnInit, ViewChild } from '@angular/core';
import { ProjectService } from '../../services/projectService';
import { User } from '../../models/user';
import { LocalStorageService } from '../../services/local-storage.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonService } from '../../services/commonService';
import { ProductOffer } from '../../models/productOffer';
import { OfferDto } from '../../models/acceptOfferDto';
import { TranslateService } from '@ngx-translate/core';
import { SharedModalComponent } from '../../shared/shared-modal/shared-modal.component';
import { NgForm } from '@angular/forms';
import { CreditCurrencyDto } from '../../models/creditCurrencyDto';
import { PayOutEncaseDto } from '../../models/PayOutEncaseDto';
import { UserService } from '../../services/userService';
import { PayInCardWebDTO } from '../../models/PayInCardWebDTO';
import { RedirectPageEnum } from '../../models/enums/redirectPageEnum';


declare var $;

@Component({
  selector: 'user-wallet-page',
  templateUrl: './user-wallet-page.component.html',
  styleUrls: ['./user-wallet-page.component.css']
})

export class UserWalletPageComponent implements OnInit {

  @ViewChild('sharedModal1') sharedModal: SharedModalComponent;

  SellListEmpty: boolean = false;
  BuyListEmpty: boolean = false;
  currentUser: User;
  token: string;
  sellList: ProductOffer[];
  buyList: ProductOffer[];
  buyHistory: ProductOffer[];
  sellHistory: ProductOffer[];
  isRunning: boolean = false;
  offerDto: OfferDto = new OfferDto();

  formInvalidCancel: boolean = false;
  formInvalidBuyCryptoCurrency: boolean = false;
  formInvalidBuyCoins: boolean = false;
  formInvalidPayOut: boolean = false;

  creditCurrencyDto: CreditCurrencyDto;
  balance: number;
  coinsBalance: number;
  canceledOffer: ProductOffer = new ProductOffer();
  canceledOfferProductName: string;
  success: boolean = false;
  debitedFund: number;
  privateKey: string;
  payOutEncaseDto: PayOutEncaseDto;

  constructor(private _projectService: ProjectService,
    private _localStorageService: LocalStorageService,
    private _router: Router, private _route: ActivatedRoute,
    private _commonService: CommonService, private translate: TranslateService,
    public _userService: UserService) {
    this.debitedFund = 0;
    this.privateKey = '';

    let user = _route.snapshot.data.currentUser;
    if (user == null) {
      this._router.navigateByUrl('/');
    } else {
      this.currentUser = user.currentUser;
      this.coinsBalance = this.currentUser.userCoinsNumber;
      this.token = this._localStorageService.getData('user').token;
      var transactionId = this._router.url.split("&transactionId=")[1];
      if (transactionId != undefined) {
        this.creditCryptoCurrency(this.currentUser.id, transactionId);
        this.isRunning = false;
      } else {
        this.isRunning = true;
        this._projectService.GetUserEuroBalance(this.currentUser.id).subscribe(
          (data) => {
            this.balance = data;
            this.isRunning = false;
          },
          (err) => {
            this.sharedModal.modalType = 'fail';
            this.sharedModal.modalMessage = "something went wrong";
            this.sharedModal.modalTitle = "Failed";
            this.sharedModal.openModal();
            this.isRunning = false;
          })
      }
    }
  }

  ngOnInit() {
    this.isRunning = true;
    this.creditCurrencyDto = new CreditCurrencyDto();
    this.payOutEncaseDto = new PayOutEncaseDto();
    this._commonService.scroll();
    this._projectService.getUserProductsBuyList(this.currentUser.id).subscribe(
      (data) => {
        this.getbalance();
        this.buyList = data.buyList;
        this.sellList = data.sellList;
        this.buyHistory = data.buyHistoryList;
        this.sellHistory = data.sellHistoryList;

      },
      (err) => {
        console.log("error getuser products");

      }
    )
  }

  openBuypopup() {
    $('#buyCryptomodal1').modal('show');
  }
  openBuypopup2() {
    $('#buyCoinsModal').modal('show');
  }

  creditCurrency(form: NgForm) {
    this.formInvalidBuyCryptoCurrency = form.invalid;
    if (!this.formInvalidBuyCryptoCurrency) {
      this.isRunning = true;
      $('#buyCryptomodal1').modal('hide');
      this.creditCurrencyDto.userId = this.currentUser.id;
      this.creditCurrencyDto.walletId = this.currentUser.walletId;
      this.creditCurrencyDto.redirectPage = RedirectPageEnum.Wallet;
      this._projectService.creditCurrency(this.creditCurrencyDto).subscribe(
        (data: PayInCardWebDTO) => {
          window.open(data.redirectURL, "_self");
        },
        (err) => {
          this.isRunning = false;
        }
      )
    }
  }


  creditCurrencyCoins(form: NgForm) {
    this.formInvalidBuyCoins = form.invalid;
    if (!this.formInvalidBuyCoins) {
      this.isRunning = true;
      $('#buyCoinsModal').modal('hide');
      this.creditCurrencyDto.userId = this.currentUser.id;
      this._userService.AddCoinsToUser(this.creditCurrencyDto, this.token).subscribe(
        (user: any) => {
          this.isRunning = false;
          if (user != null) {
            this.currentUser = user;
            this._userService.currentUser.next(user);
            this.coinsBalance = this.currentUser.userCoinsNumber;
          }
        },
        (err) => {
          this.isRunning = false;

        }
      );
      this.creditCurrencyDto = new CreditCurrencyDto;
    }
  }



  getbalance() {
    this._projectService.GetUserEuroBalance(this.currentUser.id).subscribe(
      (data) => {
        this.balance = data;
      },
      (err) => {
      })
  }

  setSharedModalMessage(title: string, message: string) {
    this.translate.get(title).subscribe((title) => {
      this.sharedModal.modalTitle = title;
    });
    this.translate.get(message).subscribe((message) => {
      this.sharedModal.modalMessage = message;
    });
  }

  openCancelPopUp(productOffer: ProductOffer, productName: string) {
    $("#cancelOfferWallet").modal('show');
    this.canceledOffer = productOffer;
    this.canceledOfferProductName = productName;
  }

  sendCancelOffer() {
    this.offerDto.userId = this.currentUser.id;
    this.offerDto.productOffer = this.canceledOffer;
    this.offerDto.productName = this.canceledOfferProductName;
    this.isRunning = true;
    $('#cancelOfferWallet').modal('hide');
    this._projectService.cancelOffer(this.offerDto).subscribe(
      () => {
        //update the list of all this user products 
        this._projectService.getUserProductsBuyList(this.currentUser.id).subscribe(
          (userProductList) => {
            this.buyList = userProductList.buyList;
            this.sellList = userProductList.sellList;
          }
        );
        this.isRunning = false;
        this.success = true;
        this.sharedModal.modalType = 'success';
        this.setSharedModalMessage('BlockChainMessages.Success', 'BlockChainMessages.SuccessCancelOffer');
        this.sharedModal.openModal();
        this.offerDto = new OfferDto();
      },
      (err) => {
        this.isRunning = false;
        this.success = false;
        this.sharedModal.modalType = 'fail';
        this.setSharedModalMessage('BlockChainMessages.ErrorPrivateKey', 'BlockChainMessages.ErrorCancelOffer');
        this.sharedModal.openModal();
      }
    )
  }

  gotoProduct(projectId: string) {
    this._projectService.smallDetailsIsClicked = false;
    this._router.navigateByUrl("details-produit/" + projectId);
    this._commonService.scrollTop();
  }

  creditCryptoCurrency(userId: number, transactionId: string) {
    this._projectService.creditCryptoCurrency(userId, transactionId).subscribe(
      (currentUser) => {
        this._userService.currentUser.next(currentUser);
        this.getbalance();
        this.isRunning = false;
      },
      (err) => {
        this.getbalance();
        this.isRunning = false;
      }
    )
  }
  payOut(form: NgForm) {
    this.formInvalidPayOut = form.invalid;
    if (!this.formInvalidPayOut) {
      this.isRunning = true;
      $('#payOutmodal').modal('hide');
      if (this.debitedFund <= this.balance) {
        this.payOutEncaseDto.debitedAmount = this.debitedFund;
        this.payOutEncaseDto.privateKey = this.privateKey;
        this.payOutEncaseDto.userId = this.currentUser.id;
        this._projectService.payOut(this.payOutEncaseDto).subscribe(
          (result) => {
            this.sharedModal.modalType = 'success';
            this.setSharedModalMessage('WalletPage.PayOutSuccessTitle', 'WalletPage.PayOutSuccessMessage');
            this.sharedModal.openModal();
            this.isRunning = true;
            this._projectService.GetUserEuroBalance(this.currentUser.id).subscribe(
              (data) => {
                this.balance = data;
                this.isRunning = false;
              },
              (err) => {
                this.isRunning = false;
              })
          },
          (err) => {
            this.sharedModal.modalType = 'fail';
            this.sharedModal.modalMessage = "something went wrong";
            this.sharedModal.modalTitle = "Failed";
            this.sharedModal.openModal();
            this.isRunning = false;
          }
        );
        this.ressetpayOut();
      } else {
        $('#payOutmodal').modal('hide');
        this.sharedModal.modalType = 'fail';
        this.setSharedModalMessage('WalletPage.PayOutFailedTitle', 'WalletPage.PayOutNotEnoughCryptoMessage');
        this.sharedModal.openModal();
        this.isRunning = false;
        this.ressetpayOut();
      }
    }
  }
  openPayOutPopup() {
    $('#payOutmodal').modal('show');
  }
  ressetpayOut() {
    this.debitedFund = 0;
    this.privateKey = '';
  }
}

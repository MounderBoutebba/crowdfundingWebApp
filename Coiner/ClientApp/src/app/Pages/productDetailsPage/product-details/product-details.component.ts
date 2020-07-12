import { SocketService } from './../../../services/socket-service';
import { ProductStatsComponent } from './../../../shared/product-stats/product-stats.component';
import { HubConnection } from '@aspnet/signalr';
import { TranslateService } from '@ngx-translate/core';
import { ProjectUpdate } from './../../../models/projectupdate';
import { UploadedDocument } from './../../../models/uploadedDocument';
import { NgxCarousel, NgxCarouselStore } from 'ngx-carousel';
import { CustomTooltip } from '../../../customClasses/customTooltip';
import { Coin } from '../../../models/coin';
import { Slider } from '../../../models/slider';
import { NouisliderComponent } from 'ng2-nouislider';
import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { Product } from '../../../models/product';
import { ProjectService } from '../../../services/projectService';
import { UserService } from '../../../services/userService';
import { BlockChainOfferDto } from '../../../models/blockChainOfferDto';
import { OfferTypeEnum } from '../../../models/enums/offerType';
import { NgForm } from '@angular/forms';
import { Project } from '../../../models/project';
import { Discussion } from '../../../models/discussion';
import { LocalStorageService } from '../../../services/local-storage.service';
import { CommonService } from '../../../services/commonService';
import { HttpErrorResponse } from '@angular/common/http';
import { User } from '../../../models/user';
import { Router, ActivatedRoute } from '@angular/router';
import { Constants } from '../../../constants';
import { ProductOffer } from '../../../models/productOffer';
import { OfferDto } from '../../../models/acceptOfferDto';
import { CreditCurrencyDto } from '../../../models/creditCurrencyDto';
import { SharedModalComponent } from '../../../shared/shared-modal/shared-modal.component';

import { Observable } from 'rxjs';
import { Location } from '@angular/common';
import { PayInCardWebDTO } from '../../../models/PayInCardWebDTO';
import { RedirectPageEnum } from '../../../models/enums/redirectPageEnum';

declare var $;

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit, OnDestroy {

  @ViewChild('sharedModal') sharedModal: SharedModalComponent;

  @ViewChild('productStats') productStats: ProductStatsComponent;


  product: Product;
  currentUser: User;
  blockChainOfferDto: BlockChainOfferDto;
  offerType: OfferTypeEnum;
  isRunning: boolean = false;
  answerContent: string;
  success: boolean = false;
  sendForm: boolean = false;
  formInvalid: boolean = false;
  betterSellValue: boolean = false;
  betterBuyValue: boolean = false;
  buyCryptoEuroDone: boolean = false;
  project: Project;
  discussion: Discussion;
  constants = Constants;
  productOffersSell: ProductOffer[];
  productOffersBuy: ProductOffer[];
  privateKey: string;
  transactionId: string;
  accetpedProductOffer: ProductOffer;
  offerDto: OfferDto;
  creditCurrencyDto: CreditCurrencyDto;
  sub: any;
  errorModalMessage: string;
  showModalButtons: boolean = false;
  userCryptoCurrency: number;
  totalPBuy: number = 0;
  totalPSale: number = 0;
  ToasterVisible: boolean;
  userAssetBalance: any;

  constructor(public _projectService: ProjectService,
    public _localStorageServicve: LocalStorageService,
    private _router: Router,
    private _route: ActivatedRoute,
    public _commonService: CommonService,
    private translate: TranslateService,
    public _sockerService: SocketService,
    public _userService: UserService,
    private _location: Location) {
    if (this._projectService.smallDetailsIsClicked) {
      this.product = this._projectService.productDetails;
      if (this.product == null) {
        this._router.navigateByUrl('/');
      } else {
        this.project = this.product.project;
      }
    } else {
      this.product = _route.snapshot.data.productDetails;
      if (this.product == null) {
        this._router.navigateByUrl('/');
        return;
      } else {
        this.project = this.product.project;
        this._projectService.productDetails = this.product;
        this._projectService.projectDetails = this.project;
      }
    }

    this._userService.currentUser.subscribe(
      (currentUser) => {
        this.currentUser = currentUser;
      })

    this.transactionId = this._router.url.split("?transactionId=")[1];
    if (this.transactionId != undefined) {
      this.creditCryptoCurrency(this.currentUser.id, this.transactionId)
    }
  }

  ngOnInit() {
    // Socket Config
    // Refresh product
    this._sockerService.hubconnection.on("GetProductFromGroup", (msg) => {
      this.product = msg;
    });
    // Refresh productOfferSell
    this._sockerService.hubconnection.on("GetProductOfferSellGroup", (msg) => {
      this.productOffersSell = msg.productOfferSell;
    });
    // Refresh productOfferBuy
    this._sockerService.hubconnection.on("GetProductOfferBuyGroup", (msg) => {
      this.productOffersBuy = msg.productOfferBuy;
    });
    // Refresh Stats
    this._sockerService.hubconnection.on("RefreshProductStats", (msg) => {
      this.productStats.stats = msg;
      this.productStats.drawGraph(msg);
      this.productStats.chart.chart.update();
    });
    // Socket Config

    this.isRunning = true;
    this.blockChainOfferDto = new BlockChainOfferDto();
    this.offerDto = new OfferDto();
    this.creditCurrencyDto = new CreditCurrencyDto();

    this.discussion = new Discussion();
    if (this.project.beginEstimatedDate != null) {
      this.project.beginEstimatedDate = new Date(this.project.beginEstimatedDate);
    }
    this._projectService.getProductOffersFromStreams(this.product.productName).subscribe(
      (productOffers) => {
        this.isRunning = false;
        this.productOffersBuy = productOffers.buyOffers;
        this.productOffersSell = productOffers.sellOffers;
      },
      (err) => {
        this.isRunning = false;
      }
    )
    //this.syncData(this.productStats.activeSyncData);
    if (this._projectService.sellButtonIsClicked) {
      console.log("sell button is clicked");
      this.offerType = 1;
      this.getCreateOfferModal();
    }
    if (this._projectService.buyButtonIsClicked) {
      console.log("buy button is clicked");
      this.offerType = 0;
      this.getCreateOfferModal();
    }
    // this.ToasterVisible = true;
    // this.userAssetBalance = 0;
    if (this.currentUser.id != null) {
      this._projectService.GetUserAssetBalance(this.product.productName, this.currentUser.id).subscribe(
        (data: any) => {
          this.userAssetBalance = data;
        }, (err) => {
          this.userAssetBalance = 0;
        }
      );
    }
  }

  ngAfterViewInit() {
    this._sockerService.hubconnection.invoke("JoinGroup", this.product.productName);
  }

  ngOnDestroy() {
    //this.stopSyncData();
    if (this.product != null)
      this._sockerService.hubconnection.invoke("LeaveGroup", this.product.productName);
  }

  getCreateOfferModal() {
    if (this.currentUser == null) {
      $('#loginModal').modal('show');
    } else {
      this.showCreateOfferModal();
    }
  }

  sendToCreateBlockChainOffer(form: NgForm) {
    this.formInvalid = form.invalid;
    if (!this.formInvalid) {
      this.isRunning = true;
      this.sendForm = true;
      this.getUserCryptoCurrencyAmount();
    }
    $('#actionsErrorModal').on('hidden.bs.modal', () => {
      this.showModalButtons = false;
    })

  }

  sendAcceptOffer() {
    this.offerDto.userId = this.currentUser.id;
    this.offerDto.productOffer = this.accetpedProductOffer;
    this.offerDto.productName = this.product.productName;
    if (this.offerType == undefined) {
      this.offerType = this.accetpedProductOffer.offerTpye;
    }
    this.isRunning = true;
    $('#acceptOfferModal').modal('hide');
    this._projectService.acceptOffer(this.offerDto, this.productStats.zoom).subscribe(
      (data: any) => {
        var productOffers = data.productOffers;
        var stats = data.stats;
        this.product.transactionVariation = data.transactionVariation;
        this.product.lastTransaction = data.lastTransaction;
        this.product.totalCapitalization = data.totalCapitalization;
        this.product.maxSellValue = data.maxSellValue;
        this.product.minBuyValue = data.minBuyValue;
        this.product.transactions = stats;
        this.productStats.stats = stats;
        this.productStats.drawGraph(this.productStats.stats);
        this.productStats.chart.chart.update();

        this._sockerService.hubconnection.invoke("SendStatsToGroup", this.productStats.stats, this.product.productName);
        this._sockerService.hubconnection.invoke("SendProductToGroup", this.product);
        this._sockerService.hubconnection.invoke("RefreshProductsListingPage", this.product);

        this.offerDto = new OfferDto();
        this.productOffersBuy = productOffers.buyOffers;
        this.productOffersSell = productOffers.sellOffers;
        this._sockerService.hubconnection.invoke("ProductOfferSellGroup", this.productOffersSell, this.product.productName);
        this._sockerService.hubconnection.invoke("ProductOfferBuyGroup", this.productOffersBuy, this.product.productName);
        this.isRunning = false;
        this.sharedModal.modalType = 'success';
        // this.sharedModal.modalTitle = 'Succés';
        // this.sharedModal.modalMessage = "Votre acceptation a été bien réussie";
        this.setSharedModalMessage('BlockChainMessages.Success', 'BlockChainMessages.SuccessAcceptationOffer');
        this.sharedModal.openModal();
        //this.productStats.updateStats(this.productStats.zoom);
        //this.updateProduct();
      },
      (err) => {
        this.isRunning = false;
        this.success = false;
        $('#createOfferModal').modal('hide');
        // init success modal
        this.sharedModal.modalType = 'fail';

        if (err.error) {
          if ((err.error.errorCode == -6) && this.offerType == OfferTypeEnum.Buy) { this.setErrorModalMessage('BlockChainMessages.ErrorNofunds'); this.showModalButtons = true; }
          ((err.error.errorCode == -6) && this.offerType == OfferTypeEnum.Sell) ? this.setErrorModalMessage('BlockChainMessages.ErrorNoActions') : null;
          (err.error.errorCode == -5) ? this.setErrorModalMessage('BlockChainMessages.ErrorPrivateKey') : null;
          (err.error.errorCode == -26) ? this.setErrorModalMessage('BlockChainMessages.ErrorOfferUnavailable') : null;
          (err.error.errorCode == 99) ? this.setErrorModalMessage('BlockChainMessages.ErrorOfferInvalidLock') : null;
          $('#actionsErrorModal').modal('show')
        } else {
          this.setSharedModalMessage('BlockChainMessages.Error', 'BlockChainMessages.ErrorCreationOffer');
        }
        this.sharedModal.modalMessage != null ? this.sharedModal.openModal() : null;
      }
    )
  }

  sendCancelOffer() {
    this.offerDto.userId = this.currentUser.id;
    this.offerDto.productOffer = this.accetpedProductOffer;
    this.offerDto.productName = this.product.productName;
    this.isRunning = true;
    $('#cancelOfferModal').modal('hide');
    this._projectService.cancelOffer(this.offerDto).subscribe(
      (data: any) => {
        var productOffers = data.productOffers;
        this.product.minBuyValue = data.minBuyValue;
        this.product.maxSellValue = data.maxSellValue;
        this.offerDto = new OfferDto();
        this.productOffersBuy = productOffers.buyOffers;
        this.productOffersSell = productOffers.sellOffers;

        this._sockerService.hubconnection.invoke("ProductOfferSellGroup", this.productOffersSell, this.product.productName);
        this._sockerService.hubconnection.invoke("ProductOfferBuyGroup", this.productOffersBuy, this.product.productName);
        this._sockerService.hubconnection.invoke("RefreshProductsListingPage", this.product);

        this.isRunning = false;
        this.sharedModal.modalType = 'success';
        // this.sharedModal.modalTitle = 'Succés';
        // this.sharedModal.modalMessage = "Votre annulation a été bien réussie";
        this.setSharedModalMessage('BlockChainMessages.Success', 'BlockChainMessages.SuccessCancelOffer');
        this.sharedModal.openModal();
      },
      (err) => {
        this.isRunning = false;
        this.success = false;
        $('#createOfferModal').modal('hide');
        // init success modal
        this.sharedModal.modalType = 'fail';

        if (err.error) {
          if ((err.error.errorCode == -6) && this.offerType == OfferTypeEnum.Buy) { this.setErrorModalMessage('BlockChainMessages.ErrorNofunds'); this.showModalButtons = true; }
          ((err.error.errorCode == -6) && this.offerType == OfferTypeEnum.Sell) ? this.setErrorModalMessage('BlockChainMessages.ErrorNoActions') : null;
          (err.error.errorCode == -5) ? this.setErrorModalMessage('BlockChainMessages.ErrorPrivateKey') : null;
          (err.error.errorCode == -26) ? this.setErrorModalMessage('BlockChainMessages.ErrorOfferUnavailable') : null;
          $('#actionsErrorModal').modal('show')
        }
        else {
          this.setSharedModalMessage('BlockChainMessages.Error', 'BlockChainMessages.ErrorCreationOffer');
        }
        this.sharedModal.modalMessage != null ? this.sharedModal.openModal() : null;
      }
    )
  }

  onAcceptOffer(productOffer: ProductOffer) {
    this.accetpedProductOffer = productOffer;
    this.offerDto.commissionFees = this.accetpedProductOffer.askQuantity * (1 / 100);
    this.totalPSale = +((this.accetpedProductOffer.offerQuantity / this.accetpedProductOffer.askQuantity) * this.product.totalCoinNumber * (this.product.project.percentageAsset / 100)).toFixed(2);
  }

  creditCurrency(form: NgForm) {
    this.formInvalid = form.invalid;
    if (!this.formInvalid) {
      this.isRunning = true;
      $('#acceptOfferModal').modal('hide');
      $('#buyCryptomodal').modal('hide');
      this.creditCurrencyDto.userId = this.currentUser.id;
      this.creditCurrencyDto.walletId = this.currentUser.walletId;
      this.creditCurrencyDto.projectId = this.product.project.id;
      this.creditCurrencyDto.redirectPage = RedirectPageEnum.Product;
      this._projectService.creditCurrency(this.creditCurrencyDto).subscribe(
        (data: PayInCardWebDTO) => {
          this.isRunning = false;
          window.open(data.redirectURL, "_self");
        },
        (err) => {
          this.isRunning = false;
        }
      )
    }
  }

  openBuypopup() {
    $('#buyCryptomodal').modal('show');
  }

  openAnswerModal(discussion: Discussion) {
    this.discussion = discussion;
    $('#answerQuestionModal').modal('show');
  }

  addQuestionToProject() {
    this.discussion.projectId = this.project.id;
    this.discussion.userId = this.currentUser.id;
    let token = this._localStorageServicve.getData('user').token;
    this.isRunning = true;
    this._projectService.addQuestionToProject(this.discussion, token).subscribe(
      (discussions) => {
        this.isRunning = false;
        this.project.discussions = discussions;
        this._commonService.orderBy(this.project.discussions, 'asc');
        this.discussion = new Discussion();
      },
      (err: HttpErrorResponse) => {
        this.isRunning = false;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
      })
  }

  updateValues() {
    this.blockChainOfferDto.totalPrice = +this.blockChainOfferDto.totalPrice.toFixed(2);
    this.blockChainOfferDto.unitPrice = +this.blockChainOfferDto.unitPrice.toFixed(2);
    this.blockChainOfferDto.commissionFees = +(this.blockChainOfferDto.totalPrice * (1 / 100)).toFixed(2);
    this.totalPBuy = +((this.blockChainOfferDto.unitPrice * this.product.totalCoinNumber) * (this.product.project.percentageAsset / 100)).toFixed(2);

    if (this.blockChainOfferDto.unitPrice != null && this.product.maxSellValue != "-" && this.offerType == 0) {
      this.betterSellValue = (parseInt(this.product.maxSellValue.split('€')[0]) < this.blockChainOfferDto.unitPrice);
    }
    if (this.blockChainOfferDto.unitPrice != null && this.product.minBuyValue != "-" && this.offerType == 1) {
      this.betterBuyValue = (parseInt(this.product.minBuyValue.split('€')[0]) > this.blockChainOfferDto.unitPrice)
    }
  }

  setSharedModalMessage(title: string, message: string) {
    this.translate.get(title).subscribe((title) => {
      this.sharedModal.modalTitle = title;
    });
    this.translate.get(message).subscribe((message) => {
      this.sharedModal.modalMessage = message;
    });
  }

  setErrorModalMessage(message: string) {
    this.translate.get(message).subscribe((message) => {
      this.errorModalMessage = message;
    });
  }

  ProductQuantityControl() {
    if (!isNaN(this.blockChainOfferDto.productQuantity) && this.blockChainOfferDto.productQuantity < this.product.totalCoinNumber) {
      this.blockChainOfferDto.productQuantity = this.blockChainOfferDto.productQuantity;
    }
    else {
      if (this.blockChainOfferDto.productQuantity > this.product.totalCoinNumber) {
        this.blockChainOfferDto.productQuantity = this.product.totalCoinNumber;
      }
    }
    this.updateValues();
  }

  addAnswerToQuestion() {
    let discussion = Object.assign({}, this.discussion);
    discussion.answerContent = this.answerContent;
    let token = this._localStorageServicve.getData('user').token;
    this.isRunning = true;
    this._projectService.addAnswerToQuestion(discussion, token).subscribe(
      (discussions) => {
        this.isRunning = false;
        this.project.discussions = discussions;
        this.answerContent = null;
        this._commonService.orderBy(this.project.discussions, 'asc');
      },
      (err: HttpErrorResponse) => {
        this.isRunning = false;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
      });
  }

  refreshProductsListingPage() {
    this.product.transactionVariation = 99;
    this.product.lastTransaction = 99;
    this.product.totalCapitalization = 99;
    this.product.minBuyValue = "99";
    this.product.maxSellValue = "99";
    this.product.transactions = [1, 5, 8, 2, 3];
    this._sockerService.hubconnection.invoke("RefreshProductsListingPage", this.product);
  }

  async getUserCryptoCurrencyAmount() {
    if (this.offerType != 0) {
      this.sendCreateOffer();
    } else {
      this.isRunning = true;
      await this._projectService.GetUserEuroBalance(this.currentUser.id).subscribe(
        (data) => {
          this.isRunning = false;
          this.userCryptoCurrency = data;
          if (this.userCryptoCurrency > this.blockChainOfferDto.totalPrice + this.blockChainOfferDto.commissionFees) {
            this.sendCreateOffer();
          } else {
            $('#createOfferModal').modal('hide');
            this.openBuypopup();
          }
        },
        (err) => {
          this.isRunning = false;
        }
      )
    }
  }

  async getUserCryptoCurrencyAmountAcceptOffer() {
    this.isRunning = true;
    await this._projectService.GetUserEuroBalance(this.currentUser.id).subscribe(
      (data) => {
        this.isRunning = false;
        this.userCryptoCurrency = data;
        if (this.userCryptoCurrency > this.accetpedProductOffer.askQuantity + this.offerDto.commissionFees) {
          this.sendAcceptOffer();
        } else {
          $('#acceptOfferModal').modal('hide');
          this.openBuypopup();
        }
      },
      (err) => {
        this.isRunning = false;
      }
    )
  }

  showCreateOfferModal() {
    this.betterSellValue = false;
    this.betterBuyValue = false;
    this.blockChainOfferDto.unitPrice = null;
    this.blockChainOfferDto.totalPrice = null;
    this.blockChainOfferDto.productQuantity = null;
    this.blockChainOfferDto.privateKey = null;
    $('#createOfferModal').modal('show');
  }

  sendCreateOffer() {
    this.blockChainOfferDto.userId = this.currentUser.id;
    this.blockChainOfferDto.productName = this.product.productName;
    this.blockChainOfferDto.offerType = this.offerType;
    this.blockChainOfferDto.currency = "EUR";
    this.blockChainOfferDto.currencyQuantity = this.blockChainOfferDto.unitPrice * this.blockChainOfferDto.productQuantity;
    this.isRunning = true;
    this._projectService.sendToCreateBlockChainOffer(this.blockChainOfferDto).subscribe(
      (data: any) => {
        var productOffers = data.productOffers;
        this.product.minBuyValue = data.minBuyValue;
        this.product.maxSellValue = data.maxSellValue;
        this.blockChainOfferDto = new BlockChainOfferDto();
        this.productOffersBuy = productOffers.buyOffers;
        this.productOffersSell = productOffers.sellOffers;

        this._sockerService.hubconnection.invoke("ProductOfferSellGroup", this.productOffersSell, this.product.productName);
        this._sockerService.hubconnection.invoke("ProductOfferBuyGroup", this.productOffersBuy, this.product.productName);
        this._sockerService.hubconnection.invoke("RefreshProductsListingPage", this.product);

        this.success = true;
        this.isRunning = false;
        $('#createOfferModal').modal('hide');
        // init success modal
        this.sharedModal.modalType = 'success';
        // this.sharedModal.modalTitle = 'Succés';
        // this.sharedModal.modalMessage = 'Votre offre a été crée avec succés';
        this.setSharedModalMessage('BlockChainMessages.Success', 'BlockChainMessages.SuccesCreationOffer');
        this.sharedModal.openModal();
      },
      (err) => {
        this.isRunning = false;
        this.success = false;
        $('#createOfferModal').modal('hide');
        // init success modal
        this.sharedModal.modalType = 'fail';

        if (err.error) {

          if ((err.error.errorCode == -6) && this.offerType == OfferTypeEnum.Buy) { this.setErrorModalMessage('BlockChainMessages.ErrorNofunds'); this.showModalButtons = true; }
          ((err.error.errorCode == -6) && this.offerType == OfferTypeEnum.Sell) ? this.setErrorModalMessage('BlockChainMessages.ErrorNoActions') : null;
          (err.error.errorCode == -5) ? this.setErrorModalMessage('BlockChainMessages.ErrorPrivateKey') : null;
          (err.error.errorCode == -26) ? this.setErrorModalMessage('BlockChainMessages.ErrorOfferUnavailable') : null;
          $('#actionsErrorModal').modal('show')
        }
        else {
          this.setSharedModalMessage('BlockChainMessages.Error', 'BlockChainMessages.ErrorCreationOffer');
        }
        this.sharedModal.modalMessage != null ? this.sharedModal.openModal() : null;
      }
    )
  }

  async creditCryptoCurrency(userId: number, transactionId: string) {
    await this._projectService.creditCryptoCurrency(userId, transactionId).subscribe((res) => {
      //console.log("buy crypteuro done");
      this.isRunning = false;
      this.buyCryptoEuroDone = true;
      $('#buyCryptoEuroDoneModal').modal('show');
    }, (err) => {
      this.isRunning = false;
      this.buyCryptoEuroDone = false;
      $('#buyCryptoEuroDoneModal').modal('show');
    });
  }

  // productToasterShow() {
  //   this.ToasterVisible = true;
  //   document.getElementById("productToaster").style.left = "5px";
  //   document.getElementById("productToaster").style.height = "auto";
  //   document.getElementById("productToaster").style.transition = "0.7s";
  //   document.getElementById("toasterTitle").style.display = "block";
  //   document.getElementById("toasterBody").style.display = "block";
  //   this._projectService.GetUserAssetBalance(this.product.productName, this.currentUser.id).subscribe(
  //     (data: any) => {
  //       this.userAssetBalance = data;
  //     }
  //   );
  // }
  // productToasterHide() {
  //   this.ToasterVisible = false;
  //   document.getElementById("productToaster").style.left = "-155px";
  //   document.getElementById("productToaster").style.height = "45px"
  //   document.getElementById("productToaster").style.transition = "0.3s"
  //   document.getElementById("toasterTitle").style.display = "none";
  //   document.getElementById("toasterBody").style.display = "none";
  // }
}

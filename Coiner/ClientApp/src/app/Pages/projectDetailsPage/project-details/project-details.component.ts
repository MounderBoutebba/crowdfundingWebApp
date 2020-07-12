import { InvestWithCryptoEuroDto } from './../../../models/investWithCryptoEuroDto';
import { CreditCurrencyDto } from './../../../models/creditCurrencyDto';
import { AddCoinsDto } from './../../../models/addCoinsDto';
import { InvestCurrencyDto } from './../../../models/investCurrencyDto';
import { ProjectUpdate } from './../../../models/projectupdate';
import { UploadedDocument } from './../../../models/uploadedDocument';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../../services/projectService';
import { Project } from '../../../models/project';
import { Constants } from '../../../constants';
import { NgxCarousel, NgxCarouselStore } from 'ngx-carousel';
import { CommonService } from '../../../services/commonService';
import { LocalStorageService } from '../../../services/local-storage.service';
import { UserService } from '../../../services/userService';
import { CustomTooltip } from '../../../customClasses/customTooltip';
import { User } from '../../../models/user';
import { Coin } from '../../../models/coin';
import { HttpErrorResponse } from '@angular/common/http';
import { Slider } from '../../../models/slider';
import { NouisliderComponent } from 'ng2-nouislider';
import { Discussion } from '../../../models/discussion';
import { PayInCardWebDTO } from '../../../models/PayInCardWebDTO';
import { NgForm } from '@angular/forms';
import { RedirectPageEnum } from '../../../models/enums/redirectPageEnum';

declare var $;
declare const Plyr: any;
@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.css']
})
export class ProjectDetailsComponent implements OnInit {
  gaugeType = "arch";
  gaugeValue = 0;
  //gaugeLabel = "Speed";
  gaugeAppendText = "%";
  foregroundColor = "#337BAE";
  size = 100;
  creditCurrencyQuantity: number;
  balence: number;
  coinsNumberInvested: number = 0;
  transactionId: string;
  formInvalid: boolean = false;
  formInvalidCryptoEuro: boolean = false;
  userInvestedCoin: boolean = false;
  buyCryptoEuroDone: boolean = false;
  privateKey: string;
  carouselBanner: NgxCarousel;
  constants = Constants;
  investCurrencyDto: InvestCurrencyDto;
  creditCurrencyDto: CreditCurrencyDto;
  investWithCryptoEuroDto: InvestWithCryptoEuroDto;
  addCoinsDto: AddCoinsDto;
  project: Project;
  currentUser: User;
  usedCoins: number = 0;
  someKeyboardConfig: any = {
    behaviour: 'drag',
    connect: [true, false],
    tooltips: new CustomTooltip,
    start: 0,
    keyboard: true, // same as [keyboard]="true"
    step: 1,
    pageSteps: 10, // number of page steps, defaults to 10
    // range: {
    //   min: 0,
    //   max: 0
    // }
  }
  ListOfVideos: UploadedDocument[] = [];
  discussion: Discussion;
  answerContent: string;
  maxCoinsEstimated: number;
  userWallet: any;
  isRunning: boolean = false;
  successInvest: boolean = false;
  fundingPercentage: number;
  coinPercentage: string;
  constructor(public _projectService: ProjectService,
    private _router: Router,
    public _commonService: CommonService,
    public _userService: UserService,
    private _route: ActivatedRoute,
    private _localStorageServicve: LocalStorageService) {
    if (!this._projectService.smallDetailsIsClicked) {
      this.project = _route.snapshot.data.projectsDetails;
      if (this.project == null) {
        this._router.navigateByUrl('/');
        return;
      } else {
        this._projectService.projectDetails = this.project;
        this._commonService.orderBy(this.project.projectUpdates, 'desc');
        this._commonService.orderBy(this.project.discussions, 'asc');
      }
    } else {
      this.project = this._projectService.projectDetails;
      if (this.project != null) {
        this._commonService.orderBy(this.project.projectUpdates, 'desc');
        this._commonService.orderBy(this.project.discussions, 'asc');
      }
    }
    this._userService.currentUser.subscribe(
      (currentUser) => {
        this.project.slider = new Slider();
        this.project.slider.sliderValue = 0;
        if (this.project == null) {
          this._router.navigateByUrl('/');
        } else {
          this.currentUser = currentUser;
          if (this.currentUser != null) {
            this.getUserCoinsForProject(this.project);
            if (this.currentUser.userCoinsNumber == 0 && this.usedCoins == 0) {
              this.project.slider.sliderMax = 1
              this.project.slider.disabledSlider = true;
            } else {
              this.project.slider.disabledSlider = false;
            }
          }
        }
      }
    );
    this.addCoinsDto = new AddCoinsDto();
    this.transactionId = this._router.url.split("?transactionId=")[1];
    if (this.transactionId != undefined) {
      this.isRunning = true;
      this.checkUserCryptoEuro();
    }
  }

  ngOnInit() {

    const scroll = $('#scrollable');
    let card2 = $('#card2').offset();
    $(scroll).scroll(function () {
      const width = (window.innerWidth > 0) ? window.innerWidth : document.documentElement.clientWidth;
      if (width > 1024) {
        card2 = $('#card2').offset();
        const scrolled = $(scroll).scrollTop();
        if (scrolled > card2.top + 130) {
          $('#card2').css('position', 'fixed').css('top', '71px');
        } else {
          $('#card2').css('position', 'static');
        }
      }
    }
    );



    this.investCurrencyDto = new InvestCurrencyDto();
    this.creditCurrencyDto = new CreditCurrencyDto();
    this.investWithCryptoEuroDto = new InvestWithCryptoEuroDto();
    this.discussion = new Discussion();
    $('[data-toggle="tooltip"]').tooltip();
    this.carouselBanner = {
      grid: { xs: 1, sm: 1, md: 1, lg: 1, all: 0 },
      slide: 1,
      speed: 400,
      interval: 2000,
      point: {
        visible: true
      },
      load: 2,
      loop: false,
      touch: true
    };
    if (this.project.beginEstimatedDate != null) {
      this.project.beginEstimatedDate = new Date(this.project.beginEstimatedDate);
    }

    this.project.projectUpdates.forEach((projectUpdate) => {
      projectUpdate.creationDate = new Date(projectUpdate.creationDate);
    })

    for (let document of this.project.documents) {
      if (document.extention == "mp4") {
        this.ListOfVideos.push(document);
      }
    }
    this.fundingPercentage = this._commonService.getProgressionPercentage(this.project.receivedFunding, this.project.fundingGoal);
    this.coinPercentage = (this.constants.coinValue * 100 / this.project.fundingGoal).toFixed(2);

  }
  sliderMinus() {
    if (this.project.projectStatus != 2) {
      if (this.project.slider.sliderValue > 0)
        this.project.slider.sliderValue -= 10;
      if (this.project.slider.sliderValue > 0)
        this.rangeChanged(this.project.slider.sliderValue);
      else {
        this.rangeChanged(0);
      }
    }
  }
  sliderPlus() {
    if (this.project.projectStatus != 2) {
      if (this.project.slider.sliderValue < this.project.fundingGoal)
        this.project.slider.sliderValue += 10;
      // the test again is when it exceeds max then set value equal max
      if (this.project.slider.sliderValue < this.project.fundingGoal)
        this.rangeChanged(this.project.slider.sliderValue);
      else {
        this.rangeChanged(this.project.fundingGoal);
      }
    }
  }
  openConfirmModal() {
    if (this.currentUser == null) {
      $('#loginModal').modal('show');
    } else {
      this.chekCoinsAdded(this.project);
      $('#confirmCoinsModal').modal('show');
    }
  }

  redirectToPaymentPage() {
    this.isRunning = true;
    $('#confirmCoinsModal').modal('hide');
    this.checkUserCryptoUser();
  }

  chekCoinsAdded(project: Project) {
    let keepGoing = true;
    project.coins.forEach(coin => {
      if (keepGoing) {
        if (coin.userId == this.currentUser.id) {
          keepGoing = false;
          if (this.project.slider.sliderValue == coin.coinsNumber) {
            this.userInvestedCoin = true;
          }
          else {
            this.coinsNumberInvested = coin.coinsNumber;
            this.userInvestedCoin = false;
          }
        }
      }
    });
  }

  checkUserCryptoUser() {
    this._projectService.GetUserEuroBalance(this.currentUser.id).subscribe((data) => {
      this.balence = data;
      if (this.userInvestedCoin) {
        this._localStorageServicve.setData("CoinNumber", this.project.slider.sliderValue)
        this._localStorageServicve.setData("CoinsAddedToProject", this.project.slider.sliderValue)
      } else {
        this._localStorageServicve.setData("CoinsAddedToProject", this.project.slider.sliderValue)
        this._localStorageServicve.setData("CoinNumber", this.project.slider.sliderValue - this.coinsNumberInvested)
      }
      if (this.balence == 0) {
        this.investCurrencyDto.userId = this.currentUser.id;
        this.investCurrencyDto.projectId = this.project.id;
        this.investCurrencyDto.walletId = this.currentUser.walletId;
        this.investCurrencyDto.currencyQuantity = this._localStorageServicve.getData("CoinNumber");
        this._projectService.investCurrency(this.investCurrencyDto).subscribe(
          (data: PayInCardWebDTO) => {
            this.isRunning = false;
            window.open(data.redirectURL, "_self");
          },
          (err) => {
            this.isRunning = false;
          }
        )
      } else if (this.balence < this._localStorageServicve.getData("CoinNumber")) {
        this.isRunning = false;
        $('#buyCryptomodal').modal('show');
      } else if (this.balence >= this._localStorageServicve.getData("CoinNumber")) {
        this.isRunning = false;
        $('#investWithCryptoEuro').modal('show');
      }
    }, (err) => {
      this.isRunning = false;
    })
  }

  investWithCryptoEuro(form: NgForm) {
    this.formInvalidCryptoEuro = form.invalid;
    if (!this.formInvalidCryptoEuro) {
      this.isRunning = true;
      let coin = new Coin();
      coin.coinValue = this.constants.coinValue;
      coin.coinsNumber = this._localStorageServicve.getData("CoinsAddedToProject")
      coin.userId = this.currentUser.id;
      coin.projectId = this.project.id;
      this.investWithCryptoEuroDto.coin = coin;
      this.investWithCryptoEuroDto.userId = this.currentUser.id;
      this.investWithCryptoEuroDto.cryptoEuroQuantity = this._localStorageServicve.getData("CoinNumber");
      this.investWithCryptoEuroDto.privateKey = this.privateKey;
      this.investWithCryptoEuroDto.ownerProjectWalletId = this.project.user.walletId;
      $('#investWithCryptoEuro').modal('hide');
      this._projectService.investWithCryptoEuro(this.investWithCryptoEuroDto).subscribe(
        (data) => {
          this._localStorageServicve.removeData("CoinNumber");
          this.isRunning = false;
          this.successInvest = true;
          data[1].slider = new Slider();
          data[1].slider.sliderMax = data[0];
          this.userWallet = data[2];
          this.project = this._projectService.projectDetails = data[1];
          if (this.project.beginEstimatedDate != null) {
            this.project.beginEstimatedDate = new Date(this.project.beginEstimatedDate);
          }
          this._userService.currentUser.value.userCoinsNumber = data[0];
          this.getUserCoinsForProject(this.project);
          if (this._userService.currentUser.value.userCoinsNumber == 0 && this.usedCoins == 0) {
            this.project.slider.sliderMax = 1;
            this.project.slider.sliderValue = 0;
          } else {
            this.getUserCoinsForProject(this.project);
          }
          if (this._userService.currentUser.value.userCoinsNumber == 0 && this.usedCoins == 0) {
            this.project.slider.disabledSlider = true;
          } else {
            this.project.slider.disabledSlider = false;
          }
          this.openSuccessInvestDaialogue();
        }, (err) => {
          this.isRunning = false;
          this.successInvest = false;
          this._localStorageServicve.removeData("CoinNumber");
          this.openSuccessInvestDaialogue();
        });
    }
  }

  creditCurrency(form: NgForm) {
    this.formInvalid = form.invalid;
    if (!this.formInvalid) {
      this.isRunning = true;
      $('#buyCryptomodal').modal('hide');
      this.creditCurrencyDto.userId = this.currentUser.id;
      this.creditCurrencyDto.walletId = this.currentUser.walletId;
      this.creditCurrencyDto.projectId = this.project.id;
      this.creditCurrencyDto.redirectPage = RedirectPageEnum.Project;
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

  addCoinsToProject() {
    let coin = new Coin();
    let token = this._localStorageServicve.getData('user').token;
    coin.coinValue = this.constants.coinValue;
    coin.coinsNumber = this._localStorageServicve.getData("CoinsAddedToProject")
    coin.userId = this.currentUser.id;
    coin.projectId = this.project.id;
    this.isRunning = true;
    this.addCoinsDto.coin = coin;
    this.addCoinsDto.transactionId = this.transactionId;
    this.addCoinsDto.userWallerID = this.project.user.walletId;
    this._projectService.addCoinsToProject(this.addCoinsDto, token).subscribe(
      // data : 0: userCoinsNumber, 1 currentProject
      (data) => {
        this._localStorageServicve.removeData("CoinNumber");
        this.isRunning = false;
        this.successInvest = true;
        data[1].slider = new Slider();
        data[1].slider.sliderMax = data[0];
        this.userWallet = data[2];
        this.project = this._projectService.projectDetails = data[1];
        if (this.project.beginEstimatedDate != null) {
          this.project.beginEstimatedDate = new Date(this.project.beginEstimatedDate);
        }
        this._userService.currentUser.value.userCoinsNumber = data[0];
        this.getUserCoinsForProject(this.project);
        if (this._userService.currentUser.value.userCoinsNumber == 0 && this.usedCoins == 0) {
          this.project.slider.sliderMax = 1;
          this.project.slider.sliderValue = 0;
        } else {
          this.getUserCoinsForProject(this.project);
        }
        if (this._userService.currentUser.value.userCoinsNumber == 0 && this.usedCoins == 0) {
          this.project.slider.disabledSlider = true;
        } else {
          this.project.slider.disabledSlider = false;
        }
        this.openSuccessInvestDaialogue();
      },
      (err: HttpErrorResponse) => {
        this.isRunning = false;
        this.successInvest = false;
        this.openSuccessInvestDaialogue();
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
      });
  }

  ngAfterViewInit() {
    if (!this._commonService.detectEdge()) {
      for (let document of this.project.documents) {
        if (document.extention == "mp4") {
          var plyr = new Plyr('#player' + document.id);
        }
      }
    }
  }

  openSuccessInvestDaialogue() {
    $('#InvestDone').modal('show');
  }

  getUserCoinsForProject(project: Project) {
    this.maxCoinsEstimated = Math.ceil((this.project.fundingGoal - this.project.receivedFunding) / this.constants.coinValue);
    if (project.coins.length != 0) {
      let keepGoing = true;
      project.coins.forEach(coin => {
        if (keepGoing) {
          if (coin.userId == this.currentUser.id) {
            keepGoing = false;
            this.usedCoins = coin.coinsNumber;
            this.project.slider.sliderValue = coin.coinsNumber;
            if (this.maxCoinsEstimated < this.currentUser.userCoinsNumber) {
              this.project.slider.sliderMax = coin.coinsNumber + this.maxCoinsEstimated;
            } else {
              this.project.slider.sliderMax = coin.coinsNumber + this.currentUser.userCoinsNumber;
            }
          }
          else {
            if (this.maxCoinsEstimated < this.currentUser.userCoinsNumber) {
              this.project.slider.sliderMax = this.maxCoinsEstimated;
              if (this.maxCoinsEstimated < 1) this.project.slider.sliderMax = 1;
            }
            else
              this.project.slider.sliderMax = this.currentUser.userCoinsNumber;
          }
        }
      })
    } else {
      this.usedCoins = 0;
      if (this.maxCoinsEstimated < this.currentUser.userCoinsNumber) {
        this.project.slider.sliderMax = this.maxCoinsEstimated;
        if (this.maxCoinsEstimated < 1) this.project.slider.sliderMax = 1;
      } else {
        this.project.slider.sliderMax = this.currentUser.userCoinsNumber;
      }
    }
    this.fundingPercentage = Math.round(this._commonService.getProgressionPercentage(this.project.receivedFunding, this.project.fundingGoal));
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
      });
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

  goToProduct() {
    this._projectService.smallDetailsIsClicked = false;
    this._router.navigateByUrl("details-produit/" + this.project.id);
    this._commonService.scrollTop();
  }

  rangeChanged(updatedRange) {
    this.gaugeValue = (updatedRange * 100) / this.project.fundingGoal;
  }

  async checkUserCryptoEuro() {
    await this._projectService.GetUserEuroBalance(this.currentUser.id).subscribe((data) => {
      let userBalence = data;
      if (userBalence == 0) {
        this.addCoinsToProject();
      } else {
        this._projectService.creditCryptoCurrency(this.currentUser.id, this.transactionId).subscribe((res) => {
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
    });
  }
}

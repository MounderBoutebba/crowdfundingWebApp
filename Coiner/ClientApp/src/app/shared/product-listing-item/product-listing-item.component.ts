import { Product } from './../../models/product';
import { ProjectService } from './../../services/projectService';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Constants } from '../../constants';
import { Chart } from 'chart.js'
import { CommonService } from '../../services/commonService';
import { BaseChartDirective } from 'ng2-charts/charts/charts';

@Component({
  selector: 'product-listing-item',
  templateUrl: './product-listing-item.component.html',
  styleUrls: ['./product-listing-item.component.css']
})
export class ProductListingItemComponent implements OnInit {
  constants = Constants;
  @ViewChild(BaseChartDirective) public chart: BaseChartDirective;
  
  //product: any;
  // chart = [];
  stats: any[] = undefined;
  @Input() product: Product;
  @Input() slider: string;
  public lineChartData: Array<any>;
  public lineChartLabels: Array<any>;
  public lineChartOptions: any = {
    responsive: true,
    legend: {
      display: false
    },
    elements: { point: { radius: 0 } },
    scales: {
      xAxes: [{
        display: false
      }],
      yAxes: [{
        display: false,
        ticks: {
          beginAtZero: true
        }
      }],
    }
  };
  public lineChartColors: Array<any> = [
    {
      backgroundColor: "rgba(228,244,249,1)",
      borderColor: "rgba(98,194,208,0.75)",
      // backgroundColor: 'rgba(148,159,177,0.2)',
      // borderColor: 'rgba(148,159,177,1)',
      // pointBackgroundColor: 'rgba(148,159,177,1)',
      // pointBorderColor: '#fff',
      // pointHoverBackgroundColor: '#fff',
      // pointHoverBorderColor: 'rgba(148,159,177,0.8)'
    }
  ];
  public lineChartLegend: boolean = true;
  public lineChartType: string = 'line';

  constructor(private _projectService: ProjectService,
    private _router: Router,
    public _commonService: CommonService) {

  }

  ngOnInit() {
    this.lineChartData = [
      { data: this.product.transactions, label: 'PxCoin', lineTension: 0 },
    ];
    this.lineChartLabels = this.product.transactions;
  }

  buybuttonClicked() {
    this.CompIsClick();
    this._projectService.buyButtonIsClicked = true;
    this._projectService.sellButtonIsClicked = false;
  }

  sellbuttonClicked() {
    this.CompIsClick();
    this._projectService.sellButtonIsClicked = true;
    this._projectService.buyButtonIsClicked = false;
  }

  getProductDetails() {
    this.CompIsClick();
    this._projectService.sellButtonIsClicked = false;
    this._projectService.buyButtonIsClicked = false;

  }

  CompIsClick() {
    this._projectService.productDetails = this.product;
    this._router.navigateByUrl("details-produit/" + this.product.project.id);
    this._commonService.scrollTop();
    this._projectService.smallDetailsIsClicked = true;
  }

  drawGraph(stats) {
    this.stats = stats;
    this.lineChartData = [
      { data: stats, label: 'PxCoin', lineTension: 0 },
    ];
    this.lineChartLabels = [];
    this.lineChartLabels = stats;
    var max = this.stats.reduce((a, b) => {
      return Math.max(a, b);
    });
    // var min = this.stats.reduce((a, b) => {
    //   return Math.min(a, b);
    // });
    if (this.chart != undefined && this.chart.chart != undefined) {
    } else {   // This re-renders the canvas element.
      this.lineChartOptions.scales.yAxes[0].ticks.max = Math.round(max + ((10 / 100) * max));
      // this.lineChartOptions.scales.yAxes[0].ticks.min = min;
    }
  }
  
  //changeSyncData() {
  //  this.activeSyncData = !this.activeSyncData;
  //  this.switchSyncData.emit(this.activeSyncData);
  //}


}

BaseChartDirective.prototype.ngOnChanges = function (changes) {
  if (this.initFlag) {
    // Check if the changes are in the data or datasets
    if (changes.hasOwnProperty('data') || changes.hasOwnProperty('datasets')) {
      if (changes['data']) {
        this.updateChartData(changes['data'].currentValue);
      }
      else {
        this.updateChartData(changes['datasets'].currentValue);
      }
      // add label change detection every time
      if (changes['labels']) {
        if (this.chart && this.chart.data && this.chart.data.labels) {
          this.chart.data.labels = changes['labels'].currentValue;
          var max = this.chart.data.labels.reduce((a, b) => {
            return Math.max(a, b);
          });
          // var min = this.chart.data.labels.reduce((a, b) => {
          //   return Math.min(a, b);
          // });
          this.chart.options.scales.yAxes[0].ticks.max = Math.round(max + ((10 / 100) * max));
          // this.chart.options.scales.yAxes[0].ticks.min = min;
        }
      }
      this.chart.update();
    }
    else {
      // otherwise rebuild the chart
      this.refresh();
    }
  }
};


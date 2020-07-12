import { HubConnection } from '@aspnet/signalr';
import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { Product } from '../../models/product';
import { ProjectService } from '../../services/projectService';
import { BaseChartDirective } from 'ng2-charts/charts/charts';

declare var $;

@Component({
  selector: 'product-stats',
  templateUrl: './product-stats.component.html',
  styleUrls: ['./product-stats.component.css']
})
export class ProductStatsComponent implements OnInit {

  @Input() product: Product;
  //@Output() switchSyncData: EventEmitter<boolean> = new EventEmitter();

  @ViewChild(BaseChartDirective) public chart: BaseChartDirective;

  messages: string[] = [];
  hubconnection: HubConnection;
  zoom: string; // week, year, month
  stats: any[] = undefined;
  isRunning: boolean;
  activeSyncData: boolean = false;

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
        // display: false
        ticks: {
          beginAtZero: true
        }
      }],
    }
  };
  public lineChartColors: Array<any> = [
    {
      backgroundColor: "rgba(255,255,255,0)",
      borderColor: "#337BAE"
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

  constructor(private _projectService: ProjectService) { }

  ngOnInit() {
    this.zoom = "year";
    this.updateStats(this.zoom);
  }

  setZoom(zoom: string) {
    this.zoom = zoom;
    this.isRunning = true;
    this.updateStats(zoom);
  }

  updateStats(zoom: string) {
    this._projectService.updateStats(this.product.productName, zoom)
      .subscribe(
        (stats) => {
          this.isRunning = false;
          this.drawGraph(stats);
        }),
      (err) => {
        this.isRunning = false;
      }
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

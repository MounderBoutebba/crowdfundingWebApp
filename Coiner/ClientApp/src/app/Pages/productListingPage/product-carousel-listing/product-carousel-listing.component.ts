import { NgxCarousel } from 'ngx-carousel';
import { Component, OnInit, Input } from '@angular/core';
import { ProjectService } from '../../../services/projectService';
import { CommonService } from '../../../services/commonService';
import { Router } from '@angular/router';
import { Constants } from '../../../constants';

@Component({
  selector: 'product-carousel-listing',
  templateUrl: './product-carousel-listing.component.html',
  styleUrls: ['./product-carousel-listing.component.css']
})
export class ProductCarouselListingComponent implements OnInit {

  @Input() carouselTileItems: Array<any>;
  @Input() loader: boolean;
  displayAllProductsButton: boolean = false;
  public carouselTile: NgxCarousel;

  options: any = {
    infinite: false,
    rows: 2,
    slidesPerRow: 3,
    touchMove: false,
    swipe: false,
    nextArrow: `<button style="position: absolute;margin: auto;
    bottom: 350px;width: 70px;height: 70px;     
    border-radius: 999px;    
    right: -95px;    
    outline: none;    
    box-sizing: border-box;    
    border: 2px solid #337BAE;    
    background-color: #F5F6FA;}">
    <span style="color: #337BAE !important;font-size: 18px !important" class="fa fa-arrow-right"></span>
    </button>`
    , prevArrow: `<button style="position: absolute;margin: auto;
    bottom: 350px;width: 70px;height: 70px;     
    border-radius: 999px;    
    left: -95px;    
    outline: none;    
    box-sizing: border-box;    
    border: 2px solid #337BAE;    
    background-color: #F5F6FA;}">
    <span style="color: #337BAE !important;font-size: 18px !important" class="fa fa-arrow-left"></span>
    </button>`
  }

  constructor(private _router: Router,
    private _commonService: CommonService,
    private _projectService: ProjectService) {
    this._projectService.productCarouselsCount.subscribe(
      (productsCount) => {
        this.displayAllProductsButton = (productsCount > Constants.MaxCarouselProjectsNumber)
      }
    )
  }

  ngOnInit() {
    this.carouselTile = {
      grid: { xs: 1, sm: 2, md: 3, lg: 3, all: 0 },
      slide: 3,
      speed: 400,
      animation: 'lazy',
      point: {
        visible: false
      },
      load: 2,
      touch: false,
      easing: 'ease'
    }
  }

  dispalyAllProducts() {
    // window.scroll(0,0);
    this._commonService.scroll();
    this._router.navigateByUrl('liste-produits');
  }

}

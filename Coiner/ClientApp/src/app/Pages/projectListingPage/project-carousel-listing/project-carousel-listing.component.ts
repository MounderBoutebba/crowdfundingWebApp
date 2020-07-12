import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { NgxCarousel } from 'ngx-carousel';
import { CommonService } from '../../../services/commonService';
import { ProjectService } from '../../../services/projectService';
import { Constants } from '../../../constants';

import { SlickSliderComponent } from '../../../customClasses/slickSlider';
@Component({
  selector: 'project-carousel-listing',
  templateUrl: './project-carousel-listing.component.html',
  styleUrls: ['./project-carousel-listing.component.css']
})
export class ProjectCarouselListingComponent implements OnInit {

  constructor(private _router: Router,
    private _commonService: CommonService,
    private _projectService: ProjectService) {
    this._projectService.projectCarouselsCount.subscribe(
      (projectsCount) => {
        this.displayAllProjectsButton = (projectsCount > Constants.MaxCarouselProjectsNumber)
      }
    )
  }
  @Input() carouselTileItems: Array<any>;
  @Input() loader: boolean;
  displayAllProjectsButton: boolean = false;
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

  dispalyAllProjects() {
    // window.scroll(0,0);
    this._commonService.scroll();
    this._router.navigateByUrl('liste-projets');
  }

}

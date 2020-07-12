import { Injectable } from '@angular/core';
import { Resolve, Router } from '@angular/router';


import { ProjectService } from '../services/projectService';
import { ActivatedRouteSnapshot } from '@angular/router';

@Injectable()
export class ProductDetailsFromUrlResolver implements Resolve<any> {
  productName: string;
  constructor(private _projectService: ProjectService) { }

  resolve(r: ActivatedRouteSnapshot) {
    if (!this._projectService.smallDetailsIsClicked) {
      this.productName = r.params['name'];
      return this._projectService.getProductFromUrl(this.productName);//"asset_"+this.productName);
    }
  }
}

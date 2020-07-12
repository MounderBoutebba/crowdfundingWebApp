import { Injectable } from '@angular/core';

import { Resolve } from '@angular/router';

import { Observable } from 'rxjs';


import { ProjectService } from '../services/projectService';

@Injectable()
export class ProductsCarouselCountResolver implements Resolve<any> {
  constructor(private projectService: ProjectService) {}

  resolve() {
    return this.projectService.GetLatestProducts();
  }
}

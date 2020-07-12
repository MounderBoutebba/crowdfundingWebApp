import { ProductFilterDto } from './../../../models/productFilterDto';
import { CommonService } from './../../../services/commonService';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, HostListener, Renderer, OnDestroy, AfterViewInit, ViewChildren, QueryList } from '@angular/core';
import { Product } from '../../../models/product';
import { ProjectService } from '../../../services/projectService';
import { Constants } from '../../../constants';
import { SocketService } from '../../../services/socket-service';
import { ProductListingItemComponent } from '../../../shared/product-listing-item/product-listing-item.component';

declare var $;

@Component({
  selector: 'app-product-listing',
  templateUrl: './product-listing.component.html',
  styleUrls: ['./product-listing.component.css']
})
export class ProductListingComponent implements OnInit, OnDestroy, AfterViewInit {
  products: Product[];
  filteredProductsCount: number;
  displayScrollTo: boolean = false;
  itemsPerPage = 6;
  page = 1;
  productFilterDto: ProductFilterDto;
  projects: any[] = [];
  mode: string;
  scrollEvent: any;
  emptyProducts: boolean=false;
  seeMore;
  isRunning: boolean = false;
  elementHeight: any;
  scrollHeight: number;
  $: any;

  @ViewChildren('produtcItems') produtcItems: QueryList<ProductListingItemComponent>;

  constructor(private _projectService: ProjectService,
    private _router: Router,
    private _socketService: SocketService,
    private route: ActivatedRoute,
    public _commonService: CommonService,
    private renderer: Renderer) {    
    var productsCount = this.route.snapshot.data.products[1];
    this._projectService.productCarouselsCount.next(productsCount);
    if (productsCount == 0) {
      this.emptyProducts = true;
    }
    this.productFilterDto = new ProductFilterDto();
    this.scrollHeight = 0;
    this.elementHeight = 0;
  }

    ngOnInit() {
        this._socketService.hubconnection.on("OnRefreshProductsListingPage", (product: Product) => {
            this.produtcItems.toArray().forEach(productItem => {
                if (productItem.product.productName == product.productName) {
                    productItem.product = product;
                    productItem.stats = productItem.product.transactions;
                    productItem.drawGraph(productItem.stats);
                    productItem.chart.chart.update();
                }
            });
        });
    this.isRunning=true;
    this._commonService.scroll();   
    this._projectService.filteredProducts.subscribe(
      (filteredProducts) => {
        this.products = filteredProducts;
        this.isRunning = false;
        if (this.products.length >= 6) { 
             this.displayScrollTo = true;
        }  
      }
     );     
    if (!this.emptyProducts) {   
    this.newFilter();
    } 
    this._projectService.filteredProductsCount.subscribe(
      (filteredProductsCount) => { 
        this.filteredProductsCount = filteredProductsCount;
          if(this.products.length == this.filteredProductsCount) clearInterval(this.seeMore);
        }
      ) 
  }

    ngAfterViewInit() {
        this.seeMore = setInterval(() => {
            this.scrollHeight = document.getElementById('scrollable').scrollHeight;
            this.elementHeight = $('#scrollable').height();
            if (this.scrollHeight > this.elementHeight) {
                clearInterval(this.seeMore); return;
            }
            this.getMoreProjects();
        }, 3000);
    this.scrollEvent = this.renderer.listen(document.getElementById('scrollable'), 'scroll', () => {
      this.onWindowScroll();
    })
    this._socketService.hubconnection.invoke("JoinGroup", "ProductsListingPage");
  } 

  ngOnDestroy() {
    this.seeMore;
    this.scrollEvent();
    this._projectService.filteredProducts.next([]);
    this._socketService.hubconnection.invoke("LeaveGroup", "ProductsListingPage");
  }

  getMoreProjects() {
    this.page += 1;
    this.mode = 'getMore';
    this.getFilteredProducts();
  }

  // @HostListener('window:scroll')
  onWindowScroll() {      
    if (this.displayScrollTo) {
      if (this._commonService.scrollDisabled || this.products.length == this.filteredProductsCount) {
        return;
      }
      let offsetHeight = document.getElementById('scrollable').offsetHeight;
      let scrollHeight = document.getElementById('scrollable').scrollHeight;
      let scrollTop = document.getElementById('scrollable').scrollTop;
      let heightDifference = scrollHeight-offsetHeight;
      
      if ( scrollTop > (heightDifference-20) ) {
        this._commonService.disableScroll();
        this._commonService.scrollDisabled = true;       
        this.getMoreProjects();
      }
    }
  }

  newFilter() {   
    this.mode = 'newFilter';
    this._projectService.filteredProducts.next([]);
    this.itemsPerPage = 6;
    this.page = 1;
    this.getFilteredProducts();    
  }

  getFilteredProducts() {
    this.isRunning = true;
    this.productFilterDto.pageIndex = (this.page - 1) * this.itemsPerPage;
    this.productFilterDto.pageSize = this.itemsPerPage;
    this._projectService.getFiltredProducts(this.productFilterDto).subscribe(
      (data: any) => {
        let newProducts = [];
        let oldProducts = this._projectService.filteredProducts.value;
        newProducts = oldProducts.concat(data.products);
        this._projectService.filteredProducts.next(newProducts);
        this._projectService.filteredProductsCount.next(data.productsCount);
        this._commonService.enableScroll();
        this._commonService.scrollDisabled = false;
        this.isRunning = false;        
      },
      (err) => {
        this.isRunning = false;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
      }
    )
  }

}

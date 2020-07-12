import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../services/projectService';
import { LocalStorageService } from '../../services/local-storage.service';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from '../../services/commonService';
import { UserService } from '../../services/userService';
import { User } from '../../models/user';
import { ProductStats } from '../../models/ProductStats';


@Component({
  selector: 'user-stats-page',
  templateUrl: './user-stats-page.component.html',
  styleUrls: ['./user-stats-page.component.css']
})
export class UserStatsPageComponent implements OnInit {

  currentUser : User;
  token :string;
  productStatsList : ProductStats[]; 
  constructor(private _projectService : ProjectService,
    private _localStorageService : LocalStorageService,
    private _router : Router,private _route : ActivatedRoute,
    private _commonService : CommonService,private translate: TranslateService,
    public _userService: UserService,
      ) 
    { 
 
      let user = _route.snapshot.data.currentUser;
      if (user == null) {
        this._router.navigateByUrl('/');
      } else {
        this.currentUser = user.currentUser;
        this.token = this._localStorageService.getData('user').token;
      }
    }
  ngOnInit() {
    this._commonService.scroll();
    this._projectService.GetUserStatsDetails(this.currentUser.id).subscribe(
      (Data)=>{
        this.productStatsList = Data;        
      },(err) =>{
        console.log("error getuser products");
      }
    )
  }
  goToProduct(projectId : string){
    this._projectService.smallDetailsIsClicked = false;
    this._router.navigateByUrl("details-produit/" + projectId);
    this._commonService.scrollTop();    
  }
}

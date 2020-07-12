import { InvestWithCryptoEuroDto } from './../models/investWithCryptoEuroDto';
import { PayOutEncaseDto } from './../models/PayOutEncaseDto';
import { AddCoinsDto } from './../models/addCoinsDto';
import { ProductFilterDto } from './../models/productFilterDto';
import { ProjectFilterDto } from './../models/projectFilterDto';
import { Injectable } from "@angular/core";
import { Project } from "../models/project";
import { Constants } from "../constants";

import { HttpClient, HttpHeaders } from "@angular/common/http";
import { BehaviorSubject } from 'rxjs';
import { Coin } from '../models/coin';
import { UploadedDocument } from '../models/uploadedDocument';
import { ProjectUpdate } from '../models/projectupdate';
import { LocalStorageService } from './local-storage.service';
import { Discussion } from '../models/discussion';
import { Product } from '../models/product';
import { BlockChainOfferDto } from '../models/blockChainOfferDto';
import { OfferDto } from '../models/acceptOfferDto';
import { CreditCurrencyDto } from '../models/creditCurrencyDto';
import { Bill } from '../models/bill';
import { InvestCurrencyDto } from '../models/investCurrencyDto';

@Injectable()
export class ProjectService {

    readonly apiBasePath = "api/Projects/";
    public projectDetails: Project = null;
    public projectModified: Project = null;
    public projectNews: Project = null;
    public projectConfirmed: Project = null;
    public projectEndSurvey: Project = null;
    public smallDetailsIsClicked = false;
    public sellButtonIsClicked = false;
    public buyButtonIsClicked = false;
    public filteredProjects = new BehaviorSubject<Project[]>([]);
    public filteredUserProjects = new BehaviorSubject<Project[]>([]);
    public filteredProducts = new BehaviorSubject<Product[]>([]);
    public filteredProjectsCount = new BehaviorSubject<number>(0);
    public filteredUserProjectsCount = new BehaviorSubject<number>(0);
    public filteredProductsCount = new BehaviorSubject<number>(0);
    public projectCarouselsCount = new BehaviorSubject<number>(0);
    public productCarouselsCount = new BehaviorSubject<number>(0);
    public productDetails: Product = null;

    constructor(private _http: HttpClient,
        private _localStorageService: LocalStorageService) {

    }
    createProject(project: Project, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "CreateProject";
        let headers: HttpHeaders = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');
        headers = headers.append('Authorization', 'bearer ' + token);
        var data = JSON.stringify(project);
        return this._http.post(url, data, { headers: headers })
    }

    getprojectsList() {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetProjects";
        return this._http.get<any[]>(url);
    }

    getLatestProjectsList() {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetLatestProjects";
        return this._http.get<any[]>(url);
    }

    getFiltredProjects(projectfilterDto: ProjectFilterDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetFilteredProjects";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(projectfilterDto);
        return this._http.post(url, data, { headers: headers })
    }

    getUserProjects(userId: number, token: string, pageIndex: number, pageSize: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserProjects/" + userId + "/" + pageIndex + "/" + pageSize;
        let headers = new HttpHeaders({ 'Authorization': 'bearer ' + token });
        return this._http.get(url, { headers: headers })
    }

    addCoinsToProject(addCoinsDto: AddCoinsDto, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "AddCoinsToProject";
        let headers: HttpHeaders = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');
        headers = headers.append('Authorization', 'bearer ' + token);
        var data = JSON.stringify(addCoinsDto);
        return this._http.post(url, data, { headers: headers })
    }

    searchForProjects(searchInput: any) {
        var url = Constants.baseUrlServer + this.apiBasePath + "SearchForProjects/" + searchInput;
        return this._http.get<Project[]>(url)
    }

    fetchImagesContent(projectId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "FetchProjectImagesContent/" + projectId;
        return this._http.get<any[]>(url)
    }

    fetchDocumentsContent(projectId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "FetchProjectDocumentsContent/" + projectId;
        return this._http.get<UploadedDocument[]>(url)
    }

    updateProject(project: Project, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "UpdateProject";
        let headers: HttpHeaders = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');
        headers = headers.append('Authorization', 'bearer ' + token);
        var data = JSON.stringify(project);
        return this._http.post(url, data, { headers: headers })
    }

    addToFavorites(projectId: number) {
        if (this.checkOfferAlreadyFavorite(projectId)) {
            this.removeOfferfromFavorites(projectId);
            return;
        } else {
            let ids: any[] = this._localStorageService.getData('favoriteChocolateIds');
            if (ids == null) {
                ids = [];
            }

            if (ids.length >= Constants.maxFavoriteCount) {
                // $('#maxFavoriteLimitModal').modal('show');
            } else {
                ids.push(projectId);
                this._localStorageService.setData('favoriteChocolateIds', ids);
            }
        }
    }

    checkOfferAlreadyFavorite(projectId: number) {
        let ids: any[] = this._localStorageService.getData('favoriteChocolateIds');
        if (ids == null) {
            return false;
        }
        let id = ids.find(o => o == projectId);
        return (id == undefined) ? false : true;
    }

    removeOfferfromFavorites(projectId: number) {
        let ids: any[] = this._localStorageService.getData('favoriteChocolateIds');
        let index = ids.indexOf(projectId);
        ids.splice(index, 1)
        this._localStorageService.setData('favoriteChocolateIds', ids)
    }

    getFavoriteOffersCount() {
        let ids = this._localStorageService.getData('favoriteChocolateIds', []);
        return ids.length;
    }

    getfavoriteProjects(ids: any) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetFavoriteProjects";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        var data = JSON.stringify(ids);
        return this._http.post(url, data, { headers: headers })
    }

    addNewsToPrject(newsContent: ProjectUpdate, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "AddNewsToProject";
        let headers: HttpHeaders = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');
        headers = headers.append('Authorization', 'bearer ' + token);
        var data = JSON.stringify(newsContent);
        return this._http.post(url, data, { headers: headers })
    }

    getUserCoinsForProject(userId: number, projectId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserCoinsForProject/" + userId + "/" + projectId;
        return this._http.get<number>(url);
    }

    addQuestionToProject(dicussion: Discussion, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "AddQuestionToProject";
        let headers: HttpHeaders = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');
        headers = headers.append('Authorization', 'bearer ' + token);
        var data = JSON.stringify(dicussion);
        return this._http.post<Discussion[]>(url, data, { headers: headers })
    }

    addAnswerToQuestion(dicussion: Discussion, token: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "AddAnswerToQuestion";
        let headers: HttpHeaders = new HttpHeaders();
        headers = headers.append('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');
        headers = headers.append('Authorization', 'bearer ' + token);
        var data = JSON.stringify(dicussion);
        return this._http.post<Discussion[]>(url, data, { headers: headers })
    }

    // GetLatestDummyProducts() {
    //     var url = Constants.baseUrlServer + this.apiBasePath + "GetLatestDummyProducts";
    //     return this._http.get(url)
    // }

    GetLatestProducts() {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetLatestProducts";
        return this._http.get(url)
    }

    getFiltredProducts(productfilterDto: ProductFilterDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetFilteredProducts";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(productfilterDto);
        return this._http.post(url, data, { headers: headers })
    }

    getProjectId(projectId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetProjectId/" + projectId;
        return this._http.get<Project>(url);
    }

    endSurvey(projectId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "EndSurvey/" + projectId;
        return this._http.get<Project>(url);
    }


    refundUsers(projectId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "RefundUsers/" + projectId;
        return this._http.get<Project>(url);
    }

    getProductFromUrl(productName: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetProductFromUrl/" + productName;
        return this._http.get<string>(url);
    }

    sendToCreateBlockChainOffer(blockChainOfferDto: BlockChainOfferDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "SendToCreateBlockChainOffer";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(blockChainOfferDto);
        return this._http.post(url, data, { headers: headers })
    }
    GetUserAssetBalance(productName: string, userId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserAssetBalance/" + productName + "/" + userId;
        return this._http.get<any>(url);
    }
    getProductOffersFromStreams(productName: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetProductOffersFromStreams/" + productName;
        return this._http.get<any>(url);
    }

    confirmProject(projectId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "confirmProject/" + projectId;
        return this._http.get<any>(url);
    }

    acceptOffer(offerDto: OfferDto, zoom: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "AcceptOffer";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(offerDto);
        return this._http.post(url, data, { headers: headers })
    }

    cancelOffer(offerDto: OfferDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "CancelOffer";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(offerDto);
        return this._http.post(url, data, { headers: headers })
    }

    creditCurrency(creditCurrencyDto: CreditCurrencyDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "CreditCurrency";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(creditCurrencyDto);
        return this._http.post(url, data, { headers: headers })
    }

    investCurrency(investCurrencyDto: InvestCurrencyDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "InvestCurrency";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(investCurrencyDto);
        return this._http.post(url, data, { headers: headers })
    }

    updateStats(productName: string, zoom: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "UpdateStats/" + productName + "/" + zoom;
        return this._http.get<any>(url);
    }

    getUserProductsBuyList(userId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserProductsBuyList/" + userId;
        return this._http.get<any>(url);
    }

    GetUserEuroBalance(userId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserEuroBalance/" + userId;
        return this._http.get<any>(url);
    }

    GetUserStatsDetails(userId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserStatsDetails/" + userId;
        return this._http.get<any>(url);
    }

    RemoveFroalaImages(imageName: string[]) {
        var url = Constants.baseUrlServer + this.apiBasePath + "RemoveFroalaImages";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(imageName);
        return this._http.post(url, data, { headers: headers })
    }

    RemoveFroalaVideos(videosName: string[]) {
        var url = Constants.baseUrlServer + this.apiBasePath + "RemoveFroalaVideos";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(videosName);
        return this._http.post(url, data, { headers: headers })
    }

    RemoveFroalaFiles(filesName: string[]) {
        var url = Constants.baseUrlServer + this.apiBasePath + "RemoveFroalaFiles";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(filesName);
        return this._http.post(url, data, { headers: headers })
    }

    GetUserBills(userId: number) {
        var url = Constants.baseUrlServer + this.apiBasePath + "GetUserBills/" + userId;
        return this._http.get<Bill[]>(url);
    }

    creditCryptoCurrency(userId: number, transactionId: string) {
        var url = Constants.baseUrlServer + this.apiBasePath + "CreditCryptoCurrency/" + userId + "/" + transactionId;
        return this._http.get(url);
    }
    payOut(payOutEncaseDto: PayOutEncaseDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "payOut";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(payOutEncaseDto);
        return this._http.post(url, data, { headers: headers })
    }

    investWithCryptoEuro(investWithCryptoEuroDto: InvestWithCryptoEuroDto) {
        var url = Constants.baseUrlServer + this.apiBasePath + "investWithCryptoEuro";
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        headers.append('Accept', 'application/json');
        var data = JSON.stringify(investWithCryptoEuroDto);
        return this._http.post(url, data, { headers: headers })
    }
}

import { Constants } from "./constants";
import { ProductsCarouselCountResolver } from "./customClasses/productsCarouselCoutResolver";
import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpClientModule, HttpClient } from "@angular/common/http";
import { RouterModule } from "@angular/router";

import { AppComponent } from "./app.component";
import { NavMenuComponent } from "./nav-menu/nav-menu.component";
import { HomeComponent } from "./home/home.component";
import { NavBarComponent } from "./Pages/homePage/nav-bar/nav-bar.component";
import { HomeContentComponent } from "./Pages/homePage/home-content/home-content.component";
import { ProjectCreationWizardComponent } from "./Pages/projectCreationPage/project-creation-wizard/project-creation-wizard.component";
import { ProjectService } from "./services/projectService";
import { HttpModule } from "@angular/http";
import { ProjectListingItemComponent } from "./shared/project-listing-item/project-listing-item.component";
import { ProjectListingComponent } from "./Pages/projectListingPage/project-listing/project-listing.component";
import { ImageCropperModule } from "ng2-img-cropper";
import { UserPageComponent } from "./Pages/user-page/user-page.component";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { UserService } from "./services/userService";
import { NgxCarouselModule } from "ngx-carousel";
import "hammerjs";
import { ProjectCarouselListingComponent } from "./Pages/projectListingPage/project-carousel-listing/project-carousel-listing.component";
import { UserLoginComponent } from "./Pages/loginPage/user-login/user-login.component";
import { NouisliderModule } from "ng2-nouislider";
import { ProjectDetailsComponent } from "./Pages/projectDetailsPage/project-details/project-details.component";
import { CommonService } from "./services/commonService";
import { ProjectFilterComponent } from "./shared/project-filter/project-filter.component";
import { MultiselectDropdownModule } from "angular-2-dropdown-multiselect";
import { NgxPaginationModule } from "ngx-pagination";
import { UserProjectsComponent } from "./Pages/userProjectsPage/user-projects/user-projects.component";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { UserProfilComponent } from "./Pages/userProfilPage/user-profil/user-profil.component";
import { UserFormComponent } from "./shared/user-form/user-form.component";
import { OnlyLoggedInUsersGuard } from "./customClasses/onlyLoggedInUsersGuard";
import { LocalStorageService } from "./services/local-storage.service";
import { UpdateUserPasswordComponent } from "./Pages/loginPage/update-user-password/update-user-password.component";
import { ProjectSharedWizardComponent } from "./shared/project-shared-wizard/project-shared-wizard.component";
import { ProjectEditWizard } from "./Pages/editProjectPage/edit-project/project-edit-wizard.component";
import { SearchPageComponent } from "./Pages/searchPage/search-page/search-page.component";
import { ProductListingItemComponent } from "./shared/product-listing-item/product-listing-item.component";
import { ProductCarouselListingComponent } from "./Pages/productListingPage/product-carousel-listing/product-carousel-listing.component";
import { SlickSliderComponent } from "./customClasses/slickSlider";
import { UserFavoriteProjectsComponent } from "./Pages/userFavoriteProjectsPage/user-favorite-projects/user-favorite-projects.component";
import { LocalizedDatePipe } from "./Pipes/localizedDate";
import { registerLocaleData } from "@angular/common";
import localeFr from "@angular/common/locales/fr";
import { TimeAgoPipe } from "./Pipes/timeAgoPipe";
import { FroalaEditorModule, FroalaViewModule } from "angular-froala-wysiwyg";
import {
  RecaptchaModule,
  RECAPTCHA_SETTINGS,
  RecaptchaSettings,
  RECAPTCHA_LANGUAGE,
} from "ng-recaptcha";
import { AppRoutingModule } from "./app-routing.module";
import { ProjectsCarouselCountResolver } from "./customClasses/projectsCarouselCountResolver";
import { ProductListingComponent } from "./Pages/productListingPage/product-listing/product-listing.component";
import { ChartsModule } from "ng2-charts/ng2-charts";
import { LoaderComponent } from "./shared/loader/loader.component";
import { ProjectModifiedResolver } from "./customClasses/projectModifiedResolver";
import { ProjectDetailsFromUrlResolver } from "./customClasses/projectDetailsFromUrlResolver";
import { ProductDetailsComponent } from "./Pages/productDetailsPage/product-details/product-details.component";
import { UserProfilResolver } from "./customClasses/userProfilResolver";
import { BuySellBookComponent } from "./shared/buy-sell-book/buy-sell-book.component";
import { SharedModalComponent } from "./shared/shared-modal/shared-modal.component";
import { OrderModule } from "ngx-order-pipe";
import { AccountVerificationComponent } from "./Pages/account-verification-component/account-verification-component.component";
import {
  SocialLoginModule,
  AuthServiceConfig,
  LoginOpt,
} from "angularx-social-login";
import {
  GoogleLoginProvider,
  FacebookLoginProvider,
  LinkedInLoginProvider,
} from "angularx-social-login";
import { ProductStatsComponent } from "./shared/product-stats/product-stats.component";
import { ProductDetailsFromUrlResolver } from "./customClasses/ProductDetailsFromUrlResolver";
import { firstUpperPipe } from "./Pipes/firstUpperPipe";
import { DashboardComponent } from "./Pages/dashboard/dashboard.component";
import { UserNotificationsComponent } from "./Pages/user-notifications/user-notifications.component";
import { UserNotificationListingItemComponent } from "./Pages/user-notification-listing-item/user-notification-listing-item.component";
import { BrowserAnimationsModule } from "../../node_modules/@angular/platform-browser/animations";
import { SocketService } from "./services/socket-service";
import { UserProfileDetailsComponent } from "./Pages/userProfileDetailsPage/user-profile-details/user-profile-details.component";
import { UserWalletPageComponent } from "./Pages/user-wallet-page/user-wallet-page.component";
import { UserStatsPageComponent } from "./Pages/user-stats-page/user-stats-page.component";
import { PdfViewerModule } from "ng2-pdf-viewer";
import { NgxGaugeModule } from "ngx-gauge";

registerLocaleData(localeFr, "fr");
export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, "./assets/i18n/", ".json");
}
const googleLoginOptions: LoginOpt = {
  scope: "profile email",
};
let config = new AuthServiceConfig([
  {
    id: GoogleLoginProvider.PROVIDER_ID,
    provider: new GoogleLoginProvider(
      Constants.googleApiKey,
      googleLoginOptions
    ),
  },
]);

export function provideConfig() {
  return config;
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    NavBarComponent,
    HomeContentComponent,
    ProjectCreationWizardComponent,
    ProjectListingItemComponent,
    ProjectListingComponent,
    UserPageComponent,
    ProjectCarouselListingComponent,
    UserLoginComponent,
    ProjectDetailsComponent,
    ProjectFilterComponent,
    UserProjectsComponent,
    UserProfilComponent,
    UserFormComponent,
    UpdateUserPasswordComponent,
    ProjectEditWizard,
    ProjectSharedWizardComponent,
    ProductListingItemComponent,
    ProductCarouselListingComponent,
    SearchPageComponent,
    SlickSliderComponent,
    UserFavoriteProjectsComponent,
    LocalizedDatePipe,
    TimeAgoPipe,
    LoaderComponent,
    ProductListingComponent,
    ProductDetailsComponent,
    BuySellBookComponent,
    SharedModalComponent,
    AccountVerificationComponent,
    ProductStatsComponent,
    firstUpperPipe,
    DashboardComponent,
    UserNotificationsComponent,
    UserNotificationListingItemComponent,
    UserProfileDetailsComponent,
    UserWalletPageComponent,
    UserStatsPageComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: "serverApp" }),
    HttpClientModule,
    BsDatepickerModule.forRoot(),
    HttpModule,
    FormsModule,
    ImageCropperModule,
    NgxCarouselModule,
    BrowserAnimationsModule,
    MultiselectDropdownModule,
    NouisliderModule,
    NgxPaginationModule,
    PdfViewerModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
    }),
    FroalaEditorModule.forRoot(),
    FroalaViewModule.forRoot(),
    RecaptchaModule.forRoot(),
    ChartsModule,
    OrderModule,
    SocialLoginModule,
    AppRoutingModule,
    NgxGaugeModule,
  ],
  providers: [
    ProjectService,
    UserService,
    CommonService,
    LocalStorageService,
    SocketService,
    OnlyLoggedInUsersGuard,
    ProjectsCarouselCountResolver,
    ProductsCarouselCountResolver,
    ProjectModifiedResolver,
    ProjectDetailsFromUrlResolver,
    ProductDetailsFromUrlResolver,
    UserProfilResolver,
    OnlyLoggedInUsersGuard,

    {
      provide: RECAPTCHA_SETTINGS,
      useValue: {
        siteKey: "",
      } as RecaptchaSettings,
    },

    {
      provide: AuthServiceConfig,
      useFactory: provideConfig,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}

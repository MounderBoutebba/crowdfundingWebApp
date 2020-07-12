import { ProductsCarouselCountResolver } from './customClasses/productsCarouselCoutResolver';
import { ProductListingComponent } from './Pages/productListingPage/product-listing/product-listing.component';

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeContentComponent } from './Pages/homePage/home-content/home-content.component';
// import { CounterComponent } from './counter/counter.component';
// import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { ProjectCreationWizardComponent } from './Pages/projectCreationPage/project-creation-wizard/project-creation-wizard.component';
import { OnlyLoggedInUsersGuard } from './customClasses/onlyLoggedInUsersGuard';
import { ProjectListingComponent } from './Pages/projectListingPage/project-listing/project-listing.component';
import { UserPageComponent } from './Pages/user-page/user-page.component';
import { ProjectDetailsComponent } from './Pages/projectDetailsPage/project-details/project-details.component';
//import { UserProjectsComponent } from './Pages/userProjectsPage/user-projects/user-projects.component';
import { UpdateUserPasswordComponent } from './Pages/loginPage/update-user-password/update-user-password.component';
import { ProjectEditWizard } from './Pages/editProjectPage/edit-project/project-edit-wizard.component';
//import { UserProfilComponent } from './Pages/userProfilPage/user-profil/user-profil.component';
import { SearchPageComponent } from './Pages/searchPage/search-page/search-page.component';
//import { UserFavoriteProjectsComponent } from './Pages/userFavoriteProjectsPage/user-favorite-projects/user-favorite-projects.component';
import { ProjectsCarouselCountResolver } from './customClasses/projectsCarouselCountResolver';
import { ProjectModifiedResolver } from './customClasses/projectModifiedResolver';
import { ProjectDetailsFromUrlResolver } from './customClasses/projectDetailsFromUrlResolver';
import { ProductDetailsComponent } from './Pages/productDetailsPage/product-details/product-details.component';
import { UserProfilResolver } from './customClasses/userProfilResolver';
import { AccountVerificationComponent } from './Pages/account-verification-component/account-verification-component.component';
import { ProductDetailsFromUrlResolver } from './customClasses/ProductDetailsFromUrlResolver';
import { DashboardComponent } from './Pages/dashboard/dashboard.component';

const appRoutes: Routes = [
  { path: '', component: HomeContentComponent, pathMatch: 'full' },
  //{ path: 'counter', component: CounterComponent },
  //{ path: 'fetch-data', component: FetchDataComponent },
  { path: 'creer-projet', component: ProjectCreationWizardComponent, canActivate: [OnlyLoggedInUsersGuard], resolve: { currentUser: UserProfilResolver } },
  { path: 'liste-projets', component: ProjectListingComponent, resolve: { projects: ProjectsCarouselCountResolver } },
  { path: 'liste-produits', component: ProductListingComponent, resolve: { products: ProductsCarouselCountResolver } },
  { path: 'creer-compte', component: UserPageComponent },
  { path: 'details-produit/:name', component: ProductDetailsComponent, resolve: { productDetails: ProductDetailsFromUrlResolver } },
  { path: 'details-projet/:id', component: ProjectDetailsComponent, resolve: { projectsDetails: ProjectDetailsFromUrlResolver } },
  // { path: 'mes-projets', component: UserProjectsComponent, canActivate: [OnlyLoggedInUsersGuard], resolve: { currentUser: UserProfilResolver } },
  { path: 'modifier-password/:id', component: UpdateUserPasswordComponent },
  { path: 'modifier-projet', component: ProjectEditWizard, resolve: { projectModified: ProjectModifiedResolver } },
  //{ path: 'profil-utilisateur', component: UserProfilComponent, canActivate: [OnlyLoggedInUsersGuard], resolve: { currentUser: UserProfilResolver } },
  { path: 'rechercher-projet/:input', component: SearchPageComponent },
  //{ path: 'mes-favoris', component: UserFavoriteProjectsComponent },
  { path: 'activation-compte/:state', component: AccountVerificationComponent },
  { path: 'dashboard', component: DashboardComponent, resolve: { currentUser: UserProfilResolver } },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

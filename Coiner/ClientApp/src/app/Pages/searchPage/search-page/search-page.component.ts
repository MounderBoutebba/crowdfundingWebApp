import { Project } from './../../../models/project';
import { ProjectService } from './../../../services/projectService';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-search-page',
  templateUrl: './search-page.component.html',
  styleUrls: ['./search-page.component.css']
})
export class SearchPageComponent implements OnInit {

  constructor(private _projectServicve: ProjectService, private _route: ActivatedRoute) {
   }

   projects: any[];
   latestprojects: any[];
   carouselBanner: any;
   searchInput: string;
   projectsSearchResult: Project[] = [];
   applySearch: boolean = false;
   isRunning: boolean = false;
   success: boolean = false;
   sub: any;
   
  ngOnInit() {
    this.sub = this._route.params.subscribe(params => {
      this.searchInput = params['input'];
      this.searchForProjects();
  });
  
}

  searchForProjects() {
    if (this.searchInput.length == 0) {
      return;
    }
    this.applySearch = true;
    this.isRunning = true;
    this._projectServicve.searchForProjects(this.searchInput).subscribe(
      (projects) => {
        this.projectsSearchResult = projects;
        this.success = true;
        this.isRunning = false;
      },
      (err) => {
        this.isRunning = false;
        this.success = false;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          console.log("Server-side Error occured");
        }
      })
  }

  clearSearchInput() {
    this.searchInput = '';
    this.projectsSearchResult = [];
    this.success = false;
    this.applySearch = false;
  }
  
}

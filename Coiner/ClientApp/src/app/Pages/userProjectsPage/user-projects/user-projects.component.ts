import { UserCheckTokenDto } from './../../../models/userCheckTokenDto';
import { User } from './../../../models/user';
import { Constants } from './../../../constants';
import { Component, OnInit, Renderer, Input } from '@angular/core';
import { ProjectService } from '../../../services/projectService';
import { HttpErrorResponse } from '@angular/common/http';
import { UserService } from '../../../services/userService';
import { Router, ActivatedRoute } from '@angular/router';
import { LocalStorageService } from '../../../services/local-storage.service';
import { ProjectUpdate } from '../../../models/projectupdate';
import { NgForm } from '@angular/forms';
import { CommonService } from '../../../services/commonService';
import { Project } from '../../../models/project';

declare var $: any;

@Component({
  selector: 'app-user-projects',
  templateUrl: './user-projects.component.html',
  styleUrls: ['./user-projects.component.css']
})
export class UserProjectsComponent implements OnInit {
  froalaEditorValid: boolean = false;
  readonly apiBasePath = "api/Projects/";
  options = {
    key: '1F4J4A11D9eF5C4B3D4E2B2B6D6A3B3xqctG-7B9yE-13eC-9lcC-7ryA1wzF-10==',
    placeholderText: 'Description!',
    heightMin: 300,
    heightMax: 300,
    toolbarSticky: false,
    videoMaxSize: 50 * 1024 * 1024,
    imageMaxSize: 5 * 1024 * 1024,
    imageUploadURL: Constants.baseUrlServer + this.apiBasePath + "UploadFroalaImages",
    videoUploadURL: Constants.baseUrlServer + this.apiBasePath + "UploadFroalaVideos",
    fileUploadURL: Constants.baseUrlServer + this.apiBasePath + "UploadFroalaFiles",
    imagePaste: false,
    fileAllowedTypes: ['text/plain', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/x-pdf', 'application/pdf', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'application/vnd.ms-powerpoint', 'application/vnd.openxmlformats-officedocument.presentationml.presentation', 'application/vnd.ms-excel', 'application/json', 'text/html', 'application/docx']

  };
  froalaImagesListe: string[] = [];
  froalaFilesListe: string[] = [];
  froalaVideosListe: string[] = [];

  projects: Project[];
  filteredUserProjectsCount: number;
  displayScrollTo: boolean = false;
  itemsPerPage = 6;
  page = 1;
  pageIndex = 1;
  pageSize = 6;
  mode: string;
  scrollEvent: any;
  success: boolean = false;
  error: any;
  news: ProjectUpdate = new ProjectUpdate();
  readonly Constants = Constants;
  formInvalid: boolean = false;
  formInputsInvalide: boolean = false;
  newsAdded: boolean = false;
  DefaultMessage: boolean = true;
  isRunning: boolean = false;
  status: any;
  emptyList: string[] = [];
  currentUser: User;
  token: string;
  seeMore: any;
  scrollHeight: number;
  elementHeight: any;
  $: any;
  deleteFromServer: boolean = true;
  constructor(private _projectService: ProjectService,
    private _userService: UserService,
    private _localStorageServicve: LocalStorageService,
    private _route: ActivatedRoute,
    public _commonService: CommonService,
    private _router: Router, private renderer: Renderer) {
    let user = _route.snapshot.data.currentUser;
    if (user == null) {
      this._router.navigateByUrl('/');
    } else {
      this.currentUser = user.currentUser;
      this.token = this._localStorageServicve.getData('user').token;
    }
    this.scrollHeight = 0;
    this.elementHeight = 0;
  }

  ngOnInit() {
    this._commonService.scroll();
    this._projectService.filteredUserProjects.subscribe(
      (filteredUserProjects) => {
        this.projects = filteredUserProjects;
        if (this.projects.length >= 6) {
          this.displayScrollTo = true;
        }
      }
    )
    this.newFilter();
    this._projectService.filteredUserProjectsCount.subscribe(
      (filteredUserProjectsCount) => {
        this.filteredUserProjectsCount = filteredUserProjectsCount;
        if (this.projects.length == this.filteredUserProjectsCount) clearInterval(this.seeMore);
      }
    )
  }

  newFilter() {
    this.mode = 'newFilter';
    this._projectService.filteredUserProjects.next([]);
    this.itemsPerPage = 6;
    this.page = 1;
    this.getUserFilteredProjects();
  }

  ngAfterViewInit() {

    this.initFroalaEditor();
    $('#projectNewsModel').on('hidden.bs.modal', () => {
      if (this.deleteFromServer == true) {
        this._projectService.RemoveFroalaImages(this.froalaImagesListe).subscribe();
        this._projectService.RemoveFroalaFiles(this.froalaFilesListe).subscribe();
        this._projectService.RemoveFroalaVideos(this.froalaVideosListe).subscribe();
      }
      $('#froalaEditor').froalaEditor('html.set', '');
      this.froalaEditorValid = false;
      this.deleteFromServer = true;
      this.froalaImagesListe = [];
      this.froalaFilesListe = [];
      this.froalaVideosListe = [];
    });
    this.seeMore = setInterval(() => {
      this.scrollHeight = document.getElementById('scrollable').scrollHeight;
      this.elementHeight = $('#scrollable').height();
      if (this.scrollHeight > this.elementHeight) {
        clearInterval(this.seeMore); return;
      }
      this.getMoreProjects();
    }, 1000);
    this.scrollEvent = this.renderer.listen(document.getElementById('scrollable'), 'scroll', () => {
      this.onWindowScroll();
    })
  }

  ngOnDestroy() {

    if (this.currentUser != null) {
      this.seeMore;
      this.scrollEvent();
      this._projectService.filteredUserProjects.next([]);
    }
  }
  initFroalaEditor() {
    $('#froalaEditor').froalaEditor(this.options)
      .on('froalaEditor.image.removed', (e, editor, $img) => {
        let imageLink = $img.attr('src');
        let ss = Constants.baseUrlServer + Constants.froalaImagesPath;
        let imageName = imageLink.substring(ss.length, imageLink.length);
        let index = this.froalaImagesListe.indexOf(imageName);
        this.froalaImagesListe.splice(index, 1);
      }).on('froalaEditor.image.inserted', (e, editor, $img) => {
        let imageLink = $img.attr('src');
        let ss = Constants.baseUrlServer + Constants.froalaImagesPath;
        let imageName = imageLink.substring(ss.length, imageLink.length);
        this.froalaImagesListe.push(imageName);
      }).on('froalaEditor.file.unlink', (e, editor, file) => {
        let fileLink = file.href;
        let ss = Constants.baseUrlServer + Constants.froalaFilesPath;
        let fileName = fileLink.substring(ss.length, fileLink.length);
        let index = this.froalaFilesListe.indexOf(fileName);
        this.froalaFilesListe.splice(index, 1);

      }).on('froalaEditor.file.uploaded', (e, editor, response) => {
        let nameEnd = response.length - 2;
        let nameStart = response.lastIndexOf('\\') + 1;
        let fileName = response.substring(nameStart, nameEnd);
        this.froalaFilesListe.push(fileName);
      }).on('froalaEditor.video.beforeRemove', (e, editor, $video) => {
        let videoLink = $video.context.firstChild.currentSrc;
        let ss = Constants.baseUrlServer + Constants.froalaVideosPath;
        let videoName = videoLink.substring(ss.length, videoLink.length);
        let index = this.froalaVideosListe.indexOf(videoName);
        this.froalaVideosListe.splice(index, 1);
      }).on('froalaEditor.video.uploaded', (e, editor, response) => {
        let nameEnd = response.length - 2;
        let nameStart = response.lastIndexOf('\\') + 1;
        let VideoName = response.substring(nameStart, nameEnd);
        this.froalaVideosListe.push(VideoName);

      }).on('froalaEditor.image.error', (e, editor, error, response) => {
        // Bad link.
        if (error.code == 1) {
          editor.popups.areVisible()
            .find('.fr-image-progress-bar-layer.fr-error .fr-message')
            .text("Bad link.");
        }

        // No link in upload response.
        else if (error.code == 2) {
          editor.popups.areVisible()
            .find('.fr-image-progress-bar-layer.fr-error .fr-message')
            .text("No link in upload response.");
        }

        // Error during image upload.
        else if (error.code == 3) {
          editor.popups.areVisible()
            .find('.fr-image-progress-bar-layer.fr-error .fr-message')
            .text("Error during image upload.");
        }

        // Parsing response failed.
        else if (error.code == 4) {
          editor.popups.areVisible()
            .find('.fr-image-progress-bar-layer.fr-error .fr-message')
            .text("Parsing response failed.");
        }

        // Image too text-large.
        else if (error.code == 5) {
          editor.popups.areVisible()
            .find('.fr-image-progress-bar-layer.fr-error .fr-message')
            .text("Image size too big.");
        }

        // Invalid image type.
        else if (error.code == 6) {
          editor.popups.areVisible()
            .find('.fr-image-progress-bar-layer.fr-error .fr-message')
            .text("Invalid image type.");
        }

        // Image can be uploaded only to same domain in IE 8 and IE 9.
        else if (error.code == 7) {
          editor.popups.areVisible()
            .find('.fr-image-progress-bar-layer.fr-error .fr-message')
            .text("Image can be uploaded only to same domain in IE 8 and IE 9.");
        }

        // Response contains the original server response to the request if available.
      }).on('froalaEditor.video.error', (e, editor, error, response) => {
        // Bad link.
        if (error.code == 1) {
          editor.popups.areVisible()
            .find('.fr-video-progress-bar-layer.fr-error .fr-message')
            .text("Bad link.");
        }

        // No link in upload response.
        else if (error.code == 2) {
          editor.popups.areVisible()
            .find('.fr-video-progress-bar-layer.fr-error .fr-message')
            .text("No link in upload response.");
        }

        // Error during image upload.
        else if (error.code == 3) {
          editor.popups.areVisible()
            .find('.fr-video-progress-bar-layer.fr-error .fr-message')
            .text("Error during video upload.");
        }

        // Parsing response failed.
        else if (error.code == 4) {
          editor.popups.areVisible()
            .find('.fr-video-progress-bar-layer.fr-error .fr-message')
            .text("Parsing response failed.");
        }

        // Image too text-large.
        else if (error.code == 5) {
          editor.popups.areVisible()
            .find('.fr-video-progress-bar-layer.fr-error .fr-message')
            .text("video size too big.");
        }

        // Invalid image type.
        else if (error.code == 6) {
          editor.popups.areVisible()
            .find('.fr-video-progress-bar-layer.fr-error .fr-message')
            .text("Invalid video type.");
        }

        // Image can be uploaded only to same domain in IE 8 and IE 9.
        else if (error.code == 7) {
          editor.popups.areVisible()
            .find('.fr-video-progress-bar-layer.fr-error .fr-message')
            .text("Video can be uploaded only to same domain in IE 8 and IE 9.");
        }

        // Response contains the original server response to the request if available.
      }).on('froalaEditor.file.error', (e, editor, error, response) => {
        // Bad link.
        if (error.code == 1) {
          editor.popups.areVisible()
            .find('.fr-file-progress-bar-layer.fr-error .fr-message')
            .text("Bad link.");
        }

        // No link in upload response.
        else if (error.code == 2) {
          editor.popups.areVisible()
            .find('.fr-file-progress-bar-layer.fr-error .fr-message')
            .text("No link in upload response.");
        }

        // Error during image upload.
        else if (error.code == 3) {
          editor.popups.areVisible()
            .find('.fr-file-progress-bar-layer.fr-error .fr-message')
            .text("Error during file upload.");
        }

        // Parsing response failed.
        else if (error.code == 4) {
          editor.popups.areVisible()
            .find('.fr-file-progress-bar-layer.fr-error .fr-message')
            .text("Parsing response failed.");
        }

        // Image too text-large.
        else if (error.code == 5) {
          editor.popups.areVisible()
            .find('.fr-file-progress-bar-layer.fr-error .fr-message')
            .text("file size too big.");
        }

        // Invalid image type.
        else if (error.code == 6) {
          editor.popups.areVisible()
            .find('.fr-file-progress-bar-layer.fr-error .fr-message')
            .text("Invalid file type.");
        }

        // Image can be uploaded only to same domain in IE 8 and IE 9.
        else if (error.code == 7) {
          editor.popups.areVisible()
            .find('.fr-file-progress-bar-layer.fr-error .fr-message')
            .text("file can be uploaded only to same domain in IE 8 and IE 9.");
        }

        // Response contains the original server response to the request if available.
      });
  }
  getMoreProjects() {
    this.page += 1;
    this.mode = 'getMore';
    this.getUserFilteredProjects();
  }

  onWindowScroll() {
    if (this.projects.length == this.filteredUserProjectsCount) clearInterval(this.seeMore);
    if (this.displayScrollTo) {
      if (this._commonService.scrollDisabled || this.projects.length == this.filteredUserProjectsCount) {
        return;
      }
      let offsetHeight = document.getElementById('scrollable').offsetHeight;
      let scrollHeight = document.getElementById('scrollable').scrollHeight;
      let scrollTop = document.getElementById('scrollable').scrollTop;
      let heightDifference = scrollHeight - offsetHeight;

      if (scrollTop > (heightDifference - 20)) {
        this._commonService.disableScroll();
        this._commonService.scrollDisabled = true;
        document.getElementById('loader').style.display = 'block';
        this.getMoreProjects();
      }
    }

  }
  ressetPopUp() {
    this.newsAdded = false;
    this.DefaultMessage = true;
    this.formInvalid = false;
    this.froalaEditorValid = false;
    this.formInputsInvalide = false;
    this.news.newsContent = "";
  }

  hideProjectNewsModel() {
    $('#projectNewsModel').modal('hide');
    this.ressetPopUp();
  }

  addNews(aform: NgForm) {
    this.froalaEditorValid = $('#froalaEditor').froalaEditor('core.isEmpty');
    this.formInputsInvalide = aform.invalid;
    this.formInvalid = this.formInputsInvalide || this.froalaEditorValid;
    if (!this.formInvalid) {
      this.news.projectId = this._projectService.projectNews.id;
      this.news.newsContent = $('#froalaEditor').froalaEditor('html.get');
      let token = this._localStorageServicve.getData('user').token;
      this.isRunning = true;
      this._projectService.addNewsToPrject(this.news, token).subscribe(res => {
        this.isRunning = false;
        this.newsAdded = true;
        this.deleteFromServer = false;
        this.DefaultMessage = false;
        this.hideProjectNewsModel();
      }, err => {
        this.isRunning = false;
        this.newsAdded = false;
        this.deleteFromServer = true;
        this.DefaultMessage = false;
      })
    }
  }

  onValidateProject(status: boolean) {
    this.status = status;
    this.projects.forEach(project => {
      if (project.id == this._projectService.projectConfirmed.id) {
        project.projectStatus = 1;
        return;
      }
    })

    $('#confirmDone').modal('show');
  }

  OnloaderIsRunning(status: boolean) {
    this.isRunning = status;
  }

  getUserFilteredProjects() {

    this.pageIndex = (this.page - 1) * this.itemsPerPage;
    this.pageSize = this.itemsPerPage;
    this._projectService.getUserProjects(this.currentUser.id, this.token, this.pageIndex, this.pageSize).subscribe(
      (data: any) => {
        let newProjects = [];
        if (this.mode == 'getMore') {
          let oldProjects = this._projectService.filteredUserProjects.value;
          newProjects = oldProjects.concat(data.projects);
        } else {
          newProjects = data.projects;
        }
        this._projectService.filteredUserProjects.next(newProjects);
        this._projectService.filteredUserProjectsCount.next(data.projectsCount);
        this._commonService.enableScroll();
        this._commonService.scrollDisabled = false;
        document.getElementById('loader').style.display = 'none';
        this.success = true;
        this.isRunning = false;
      },
      (err: HttpErrorResponse) => {
        this.success = false;
        this.isRunning = false;
        if (err.error instanceof Error) {
          console.log("Client-side Error occured");
        } else {
          if (err.status == 401) {
            this.error = 401;
            console.log("You're not authorized to use this api");
          } else {
            console.log("Server-side Error occured");
          }
        }
      });
  }
}

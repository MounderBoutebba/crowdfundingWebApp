import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ProjectTypeEnum } from './../../models/enums/projectTypeEnum';
import { NgForm } from '@angular/forms';
import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { Project } from '../../models/project';
import { ProjectService } from '../../services/projectService';
import { HttpErrorResponse } from '@angular/common/http/src/response';
import { ImageCropperComponent, CropperSettings } from 'ng2-img-cropper';
import { Picture } from '../../interfaces/picture';
import { UploadedFile } from '../../interfaces/uploadedFile';
import { Router, NavigationStart } from '@angular/router';
import { forEach } from '@angular/router/src/utils/collection';
import { UploadedDocument } from '../../models/uploadedDocument';
import { ProjectImage } from '../../models/projectImage';
import { Constants } from '../../constants';
import { CommonService } from '../../services/commonService';
import { UserService } from '../../services/userService';
import { User } from '../../models/user';
import { DomSanitizer } from '@angular/platform-browser';
import { LocalStorageService } from '../../services/local-storage.service';
import { HttpClient } from '@angular/common/http';
import { TranslateService } from '@ngx-translate/core';

declare var $: any;

@Component({
  selector: 'project-shared-wizard',
  templateUrl: './project-shared-wizard.component.html',
  styleUrls: ['./project-shared-wizard.component.css']
})
export class ProjectSharedWizardComponent implements OnInit {

  @Input() Mode: string; // new, edit

  @ViewChild('cropModal1') cropModal: ElementRef;

  readonly Constants = Constants;
  user: User;
  formInvalid: boolean = false;
  formInputsInvalide: boolean = false;
  froalaEditorValid: boolean = true;
  isRunning: boolean = false;
  adminLogin: string;
  //Cropper 2 data
  data: any;
  cropperSettings: CropperSettings;
  @ViewChild('cropper1', undefined) cropper: ImageCropperComponent;

  froalaImagesListe: string[] = [];
  froalaFilesListe: string[] = [];
  froalaVideosListe: string[] = [];
  pictures: Picture[] = [];
  pictureId: number;
  newPicture: Picture;
  files: UploadedFile[] = [];
  isMaxSize: boolean = false;
  isNotImage: boolean = false;
  isMaxNumberFiles: boolean = false;
  fileUploadMaxSize = Constants.FileUploadMaxSize;
  maxNumberFiles = Constants.MaxNumberFiles;
  minBeginDate: any;
  maxBeginDate: any;
  minEnchereDate: any;
  maxEnchereDate: any;
  creationDateSocity: any;
  mincreationDateSocity: any;
  bsConfig: Partial<BsDatepickerConfig>;
  success: boolean = false;
  project: Project;
  currentStep = 1;
  lastStep = this.currentStep;
  activityTypes: any;
  readonly apiBasePath = "api/Projects/";
  fundraisingPeriods = [{ name: "1 mois", value: 1 },
  { name: "3 mois", value: 3 },
  { name: "6 mois", value: 6 },
  { name: "12 mois", value: 12 }
  ];

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
  }


  constructor(private projectService: ProjectService,
    private _router: Router,
    public _commonService: CommonService,
    private _userService: UserService,
    private _sanitizer: DomSanitizer,
    private _localStorageServicve: LocalStorageService,
    private _http: HttpClient,
    public _translateService: TranslateService, ) {
    this._userService.currentUser.subscribe((currenUser) => {
      this.user = currenUser;
      //Cropper settings
      this.cropperSettings = new CropperSettings();

      this.cropperSettings.width = 700;
      this.cropperSettings.height = 600;
      this.cropperSettings.minWidth = 200;
      this.cropperSettings.minHeight = 200;

      this.cropperSettings.croppedWidth = 700;
      this.cropperSettings.croppedHeight = 600;

      this.cropperSettings.cropperDrawSettings.strokeColor = 'rgba(255,255,255,1)';
      this.cropperSettings.cropperDrawSettings.strokeWidth = 2;
      this.cropperSettings.noFileInput = true;

      let windowWidth = $(window).width();
      if (windowWidth >= 768) {
        this.cropperSettings.canvasWidth = 568;
        this.cropperSettings.canvasHeight = 300;
      } else {
        this.cropperSettings.canvasWidth = windowWidth - 52;
        this.cropperSettings.canvasHeight = 200;
      }
      this.data = {};
    })
    this._translateService.onLangChange.subscribe(() => {
      this.getActivityTypes();
    })
    this._userService.adminLogin.subscribe((adminLogin) => {
      this.adminLogin = adminLogin;
    });
  }

  ngOnInit() {
    this._commonService.scrollTop();
    let todayDate = new Date();
    this.maxBeginDate = new Date(todayDate.getFullYear() + 3, 11, 31);
    this.creationDateSocity = new Date(1800, 0, 1);
    this.mincreationDateSocity = new Date(todayDate.getFullYear(), todayDate.getMonth(), todayDate.getDate());
    let colorTheme = 'theme-dark-blue';
    this.bsConfig = Object.assign({}, { containerClass: colorTheme, dateInputFormat: 'DD-MM-YYYY' });
    if (this.Mode == 'new') {
      this.project = new Project();
      this.project.fundraisingPeriod = this.fundraisingPeriods[0].value;
      this.minBeginDate = new Date(todayDate.getFullYear(), todayDate.getMonth(), todayDate.getDate());
      this.minEnchereDate = new Date(todayDate.getFullYear(), todayDate.getMonth(), todayDate.getDate());
    } else {
      this.currentStep = 2;
      this.project = this.projectService.projectModified;
      this.minBeginDate = this.project.beginEstimatedDate;

      if (this.project.society_CreationDate != null) {
        this.project.society_CreationDate = new Date(this.project.society_CreationDate);
      }
      this.getActivityTypes();
      this.fetchProjectImages(this.project.id);
      this.fetchProjectDocuments(this.project.id);
    }
  }

  ngAfterViewInit() {
    //Initialize Froala Editor
    this.initFroalaEditor();
    if (this.Mode == 'edit') {
      $('#froalaEditor').froalaEditor('html.set', this.project.projectDescription);
    }
    //Initialize tooltips
    $('.nav-tabs > li a[title]').tooltip();

    //Wizard
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

      var $target = $(e.target);

      if ($target.parent().hasClass('disabled')) {
        return false;
      }
    });
  }

  setProjectType(projecType: number) {
    this.resetForm();
    this.setCurrentStep(1);
    this.project.projectType = projecType;
    this.getActivityTypes();
    this.project.activityType = this.activityTypes[0].value;
    this.changeCurrentStep();
    this.nextStep();
    this._commonService.scrollTop();
  }

  nextTab(elem) {
    $(elem).next().find('a[data-toggle="tab"]').click();
  }

  prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
  }

  nextStep() {
    var $active = $('.wizard .nav-tabs li.active');
    $active.next().removeClass('disabled');
    this.nextTab($active);
  }

  resetForm() {
    this.formInputsInvalide = false;
    this.formInvalid = false;
  }

  checkInputFileds(form: NgForm) {
    this.project.projectDescription = $('#froalaEditor').froalaEditor('html.get');
    this.froalaEditorValid = $('#froalaEditor').froalaEditor('core.isEmpty');
    this.formInputsInvalide = form.invalid;
    this.formInvalid = this.formInputsInvalide || this.froalaEditorValid;
    if (this.Mode == 'edit') {
      if (!this.formInvalid) {
        this.updateProject();
      } else {
        this._commonService.scrollTop();
      }
    } else {
      if (!this.formInvalid) {
        this.changeCurrentStep();
      } else {
        this._commonService.scrollTop();
      }
    }
  }

  changeCurrentStep() {
    if (this.currentStep == 1) {
      this.setCurrentStep(2);
    } else if (this.currentStep == 2) {
      this.checkDefaultImageIfExists();
      this.setCurrentStep(3);
    } else if (this.currentStep == 3) {
      this.setCurrentStep(4);
    } else {
      this.setCurrentStep(Math.min(this.currentStep + 1, 3));
    }
    this._commonService.scrollTop();
  }

  checkMaxLenght($event, maxLength: number) {
    if ($event.target.value.length > maxLength) {
      $event.target.value = $event.target.value.slice(0, maxLength);
    }
  }

  checkPercentLength($event) {
    if (!isNaN($event.target.value) && $event.target.value.toString().indexOf('.') != -1) {
      $event.target.value = $event.target.value.slice(0, 5);
    }
    else {
      $event.target.value = $event.target.value.slice(0, Constants.PercentageMaxLength);
    }
    if ($event.target.value.length == Constants.PercentageMaxLength && $event.target.value > 100) {
      $event.target.value = 100;
    }
  }

  prevStep() {
    this.setCurrentStep(Math.max(this.currentStep - 1, 1));
    this._commonService.scrollTop();
  };

  setCurrentStep(step: number) {
    this.currentStep = step;
    this.lastStep = Math.max(this.lastStep, step);
  }

  checkDisabledTab(isDisabled: boolean, step: number) {
    if (!isDisabled) {
      this.currentStep = step;
      this.checkDefaultImageIfExists();
    }
  }

  createProject() {
    this.saveImagesOnProject();
    this.saveDocumentsOnProject();
    this.project.userId = this.user.id;
    let token = this._localStorageServicve.getData('user').token;
    this.isRunning = true;
    this.projectService.createProject(this.project, token)
      .subscribe(
        res => {
          this.isRunning = false;
          this.success = true;
          console.log(res);
          $('#projectModal').modal('show');
          $('#projectModal').on('hidden.bs.modal', () => {
            this._router.navigateByUrl('dashboard?page=myProjects');
          })
        },
        (err: HttpErrorResponse) => {
          this.success = false;
          this.isRunning = false;
          if (err.error instanceof Error) {
            console.log("Client-side Error occured");
          } else {
            console.log("Server-side Error occured");
          }
          $('#projectModal').modal('show');
        });
  }
  ProjectComissionFundingControl($event) {
    if (!isNaN($event.target.value) && $event.target.value < this.project.fundingGoal) {
      $event.target.value = $event.target.value;
    }
    else {
      if ($event.target.value > this.project.fundingGoal) {
        $event.target.value = this.project.fundingGoal - 1;
      }
    }
  }
  updateProject() {
    this.saveImagesOnProject();
    this.saveDocumentsOnProject();
    let token = this._localStorageServicve.getData('user').token;
    this.isRunning = true;
    this.projectService.updateProject(this.project, token)
      .subscribe(
        res => {
          this.isRunning = false;
          this.success = true;
          console.log(res);
          $('#projectEditModal').modal('show');
          $('#projectEditModal').on('hidden.bs.modal', () => {
            this._router.navigateByUrl('dashboard?page=myProjects');
          })
        },
        (err: HttpErrorResponse) => {
          this.isRunning = false;
          this.success = false;
          if (err.error instanceof Error) {
            console.log("Client-side Error occured");
          } else {
            console.log("Server-side Error occured");
          }
          $('#projectEditModal').modal('show');
        });
  }

  checkValidImage(file: File) {
    return !(file.type.indexOf('image') === -1);
  }

  fileChangeListener($event) {
    this.isMaxSize = false;
    this.isNotImage = false;
    var image: any = new Image();
    var file: File = $event.target.files[0];
    if (!this.checkValidImage(file)) {
      this.isNotImage = true;
      $event.target.value = "";
      return;
    }
    if (file.size >= Constants.FileUploadMaxSize) {
      this.isMaxSize = true;
      this.isNotImage = !this.checkValidImage(file);
      $event.target.value = "";
      return;
    }
    var myReader: FileReader = new FileReader();
    var that = this;
    myReader.onloadend = function (loadEvent: any) {
      image.src = loadEvent.target.result;
      that.cropper.setImage(image);
    };
    $event.target.value = "";
    myReader.readAsDataURL(file);

    this.newPicture = {
      id: this.pictureId,
      oldImage: image
    };

  }

  savePicture(pict: any) {
    this.newPicture.img = pict;
    if (this.newPicture.id == 0) {
      this.newPicture.isDefault = true;
    } else {
      this.newPicture.isDefault = false;
    };

    this.pictures[this.newPicture.id] = this.newPicture;
    this.pictureId = null;
    this.data = {};
    this.closeCropModal();
  }

  closeCropModal() {
    $(this.cropModal.nativeElement).modal('hide');
    $('#CropImage1').on('hidden.bs.modal', () => {
      this.isMaxSize = false;
      this.isNotImage = false;
    })
  }

  setPictureId(id: number) {
    this.pictureId = id;
    this.cropper.reset();
  }

  getModified(id: number) {
    this.pictureId = id;
    this.newPicture = this.pictures[id];
    var image: any = new Image();
    image.src = this.newPicture.oldImage.src;
    this.cropper.setImage(image);
  }

  saveImagesOnProject() {
    this.project.projectImages = [];
    for (let i = 0; i < this.pictures.length; ++i) {
      let picture = this.pictures[i];
      if (picture == null) {
        continue;
      }
      let projectImage = new ProjectImage();
      let image = picture.img.image.toString().split(',')[1].split(' ')[0];
      projectImage.content = image;
      projectImage.isDefault = false;
      this.project.projectImages.push(projectImage);
    }
    if (this.project.projectImages.length > 0) {
      this.project.projectImages[0].isDefault = true;
    }
  }

  setImageAsDefault(id) {
    let oldDefaultImg = this.pictures[0];
    let newDefaultImg = this.pictures[id];

    this.pictures[0] = newDefaultImg;
    this.pictures[id] = oldDefaultImg;

    newDefaultImg.isDefault = true;
    newDefaultImg.id = 0;

    if (oldDefaultImg) {
      oldDefaultImg.isDefault = false;
      oldDefaultImg.id = id;
    }
  }

  checkDefaultImageIfExists() {
    if (this.pictures.length > 0 && this.pictures[0] == null) {
      for (let i = 0; i < this.pictures.length; ++i) {
        if (this.pictures[i] != null) {
          this.setImageAsDefault(i);
          return;
        }
      }
    }
  }

  deleteImage(id) {
    this.pictures[id] = null;
  }

  onFileChange(event) {
    if (event.target.files && event.target.files.length > 0 && event.target.files.length <= Constants.MaxNumberFiles) {
      if (this.files.length < Constants.MaxNumberFiles) {
        this.isMaxNumberFiles = false;
        for (var i = 0; i < event.target.files.length; i++) { //for multiple files          
          ((file) => {
            let reader = new FileReader();
            let that = this;
            if (file.size >= Constants.FileUploadMaxSize) {
              that.isMaxSize = true;
              return;
            }
            reader.readAsDataURL(file);
            reader.onload = () => {
              let uploadedFile: UploadedFile;
              uploadedFile = {
                fileName: file.name,
                description: "",
                extention: (file.name as string).split('.').pop(),
                value: (reader.result as string).split(',')[1],
                size: file.size
              };
              if (this.files.length < Constants.MaxNumberFiles) {
                that.files.push(uploadedFile);
                this.isMaxSize = false;
              } else {
                this.isMaxSize = true;
              }
            }
          })(event.target.files[i]);
        }
      } else {
        this.isMaxNumberFiles = true;
      }
    } else {
      this.isMaxNumberFiles = true;
    }
  }

  deleteFile(file: any) {
    var index = this.files.indexOf(file);
    if (index !== -1) this.files.splice(index, 1);
  }

  saveDocumentsOnProject() {
    this.project.documents = [];
    for (let i = 0; i < this.files.length; ++i) {
      let document = this.files[i];
      let uploadedDocument = new UploadedDocument();
      uploadedDocument.title = document.fileName;
      uploadedDocument.description = document.description;
      uploadedDocument.extention = document.extention;
      uploadedDocument.content = document.value;
      this.project.documents.push(uploadedDocument);
    }
  }

  formatSizeUnits(bytes) {
    if (bytes >= 1073741824) { bytes = (bytes / 1073741824).toFixed(2) + ' GB'; }
    else if (bytes >= 1048576) { bytes = (bytes / 1048576).toFixed(2) + ' MB'; }
    else if (bytes >= 1024) { bytes = (bytes / 1024).toFixed(2) + ' KB'; }
    else if (bytes > 1) { bytes = bytes + ' bytes'; }
    else if (bytes == 1) { bytes = bytes + ' byte'; }
    else { bytes = '0 byte'; }
    return bytes;
  }

  fetchProjectImages(projectId: number) {
    this.projectService.fetchImagesContent(projectId).subscribe(
      (projectImages) => {
        this.pictures = [];
        let images = projectImages;
        for (let idx = 0; idx < images.length; ++idx) {
          let src = "data:image/jpg;base64," + images[idx].content;
          this.pictures[idx] = {
            id: idx,
            img: {
              image: this._sanitizer.bypassSecurityTrustResourceUrl(src)
            },
            isDefault: (idx === 0),
            oldImage: {
              src: src
            }
          };
        }
      },
      (err) => {
      }
    )
  }

  fetchProjectDocuments(projectId: number) {
    this.projectService.fetchDocumentsContent(projectId).subscribe(
      (projectDocuments) => {
        this.files = [];
        let documents = projectDocuments;
        for (let idx = 0; idx < documents.length; ++idx) {
          // let src = "data:image/jpg;base64," + documents[idx].content;
          this.files[idx] = {
            fileName: documents[idx].title,
            description: documents[idx].description,
            extention: documents[idx].extention,
            value: documents[idx].content
            // size: 0
          };
        }
      },
      (err) => {
      }
    )
  }

  getActivityTypes() {
    switch (this.project.projectType) {
      case ProjectTypeEnum.Project:
        this._translateService.get('ActivityTypesCreation.Project').subscribe(
          (data) => {
            this.activityTypes = data;
          }
        );
        break;
      case ProjectTypeEnum.Society:
        this._translateService.get('ActivityTypesCreation.Project').subscribe(
          (data) => {
            this.activityTypes = data;
          }
        );
        break;
      case ProjectTypeEnum.Product:
        this._translateService.get('ActivityTypesCreation.Product').subscribe(
          (data) => {
            this.activityTypes = data;
          }
        );
        break;
      case ProjectTypeEnum.Career:
        var activityTypesProject;
        var activityTypesProduct;
        this._translateService.get('AllActivityTypesCreation').subscribe(
          (data) => {
            this.activityTypes = data;
          }
        );
        break;
    }
  }

  ngOnDestroy(): void {
    if (!this.success) {
      this.projectService.RemoveFroalaImages(this.froalaImagesListe).subscribe();
      this.projectService.RemoveFroalaFiles(this.froalaFilesListe).subscribe();
      this.projectService.RemoveFroalaVideos(this.froalaVideosListe).subscribe();
    }
  }

  initFroalaEditor() {
    $('#froalaEditor').froalaEditor(this.options)
      .on('froalaEditor.image.removed', (e, editor, $img) => {
        let imageLink = $img.attr('src');
        let ss = Constants.baseUrlServer + Constants.froalaImagesPath;
        let imageName = imageLink.substring(ss.length, imageLink.length);
        this.projectService.RemoveFroalaImages([imageName]).subscribe(() => {
          let index = this.froalaImagesListe.indexOf(imageName);
          this.froalaImagesListe.splice(index, 1);
        });
      }).on('froalaEditor.image.inserted', (e, editor, $img) => {
        let imageLink = $img.attr('src');
        let ss = Constants.baseUrlServer + Constants.froalaImagesPath;
        let imageName = imageLink.substring(ss.length, imageLink.length);
        this.froalaImagesListe.push(imageName);
      }).on('froalaEditor.file.unlink', (e, editor, file) => {
        let fileLink = file.href;
        let ss = Constants.baseUrlServer + Constants.froalaFilesPath;
        let fileName = fileLink.substring(ss.length, fileLink.length);
        this.projectService.RemoveFroalaFiles([fileName]).subscribe(() => {
          let index = this.froalaFilesListe.indexOf(fileName);
          this.froalaFilesListe.splice(index, 1);
        });
      }).on('froalaEditor.file.uploaded', (e, editor, response) => {
        let nameEnd = response.length - 2;
        let nameStart = response.lastIndexOf('\\') + 1;
        let fileName = response.substring(nameStart, nameEnd);
        this.froalaFilesListe.push(fileName);
      }).on('froalaEditor.video.beforeRemove', (e, editor, $video) => {
        let videoLink = $video.context.firstChild.currentSrc;
        let ss = Constants.baseUrlServer + Constants.froalaVideosPath;
        let videoName = videoLink.substring(ss.length, videoLink.length);
        this.projectService.RemoveFroalaVideos([videoName]).subscribe(() => {
          let index = this.froalaVideosListe.indexOf(videoName);
          this.froalaVideosListe.splice(index, 1);
        });
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

}

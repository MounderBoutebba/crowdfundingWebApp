
import { ProjectService } from '../../services/projectService';
import { ComponentFixture, async, TestBed } from '@angular/core/testing';
import { ProductStatsComponent } from './product-stats.component';
import { Product } from '../../models/product';
import { TranslatePipe, TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { Http, HttpModule, BaseRequestOptions, ResponseOptions, XHRBackend } from '@angular/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { BaseChartDirective, ChartsModule } from 'ng2-charts/charts/charts';
import { LocalStorageService } from '../../services/local-storage.service';
import { Project } from '../../models/project';
import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { By } from '@angular/platform-browser';
import { Product1Mock } from '../../ModelMocks/ProductMock';

//before mocking our component using testbed
describe('unit testing product stat component', () => {
    let component: ProductStatsComponent;
    let projectService: ProjectService;

    it('component undefined ', () => {
        expect(component).toBeUndefined();
    });
    it('projectService undefined ', () => {
        expect(projectService).toBeUndefined();
    })
});

const TRANSLATIONS_EN = require('./../../../assets/i18n/en.json');
const TRANSLATIONS_FR = require('./../../../assets/i18n/fr.json');

describe("trying fixture to initialise my componont ", () => {
    let translate: TranslateService;
    let localStorageService: LocalStorageService;
    let projectService: ProjectService;
    let http: HttpTestingController;
    let component: ProductStatsComponent;
    let fixture: ComponentFixture<ProductStatsComponent>;
    let compiled;
    let el;
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ProductStatsComponent],
            imports: [
                HttpClientTestingModule,
                TranslateModule.forRoot(
                    {
                        loader: {
                            provide: TranslateLoader,
                            useFactory: (httpClient: HttpClient) => {
                                return new TranslateHttpLoader(httpClient, './../../../assets/i18n', '.json');
                            },
                            deps: [HttpClient]
                        }
                    }),
                ChartsModule,
                HttpClientModule,
                HttpModule,

            ],
            providers: [
                TranslateService,
                ProjectService,
                LocalStorageService,
                Http,
            ],
        }).compileComponents();

        translate = TestBed.get(TranslateService);
        localStorageService = TestBed.get(LocalStorageService);
        projectService = TestBed.get(ProjectService);
        http = TestBed.get(HttpTestingController);

        fixture = TestBed.createComponent(ProductStatsComponent);
        component = fixture.debugElement.componentInstance;

        el = fixture.debugElement;
        compiled = fixture.debugElement.nativeElement;
        component.product = Product1Mock;

    }));

    it('should create the test component (mock the component)', async(() => {
        fixture = TestBed.createComponent(ProductStatsComponent);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    }));

    it('should be truthy', async(() => {
        fixture = TestBed.createComponent(ProductStatsComponent);
        component = fixture.debugElement.componentInstance;
        expect(fixture).toBeTruthy();
    }));

    it('should load fr translations', async(() => {

        spyOn(translate, 'getBrowserLang').and.returnValue('en');
        // the DOM should be empty for now since the translations haven't been rendered yet
        //use lang u want 
        translate.use('fr');
        //put relative path
        http.expectOne('./../../../assets/i18nfr.json').flush(TRANSLATIONS_FR);

        fixture.detectChanges();
        // // the content should be translated to french now
        expect(compiled.querySelector('.sell-label').textContent).toEqual(TRANSLATIONS_FR.StatsPanel.Sale);
    }));

    it('should load en translations', () => {
        translate.use('en');
        http.expectOne('./../../../assets/i18nen.json').flush(TRANSLATIONS_EN);
        fixture.detectChanges();
        expect(compiled.querySelector('.sell-label').textContent).toEqual(TRANSLATIONS_EN.StatsPanel.Sale);
    });

    it('testing some css with query selector', () => {
        //just for accessing dom elements purposes we don't need this test 
        let styleTest = compiled.querySelector('.period-label').style.color;
        styleTest = 'red'
        fixture.detectChanges();
        expect(styleTest).toBe('red');
    });

    it('testing draw graph function ', () => {

        let stats = [1, 51, 31, 54, 88]
        component.drawGraph(stats);
        expect(component.stats).toBe(stats);
        fixture.detectChanges();
    });

    it('testing set zoom (month) with faking call return', () => {

        let stats = [8, 2, 5, 54, 25];
        component.setZoom('month');
        spyOn(component, 'updateStats').and.callFake(() => {
            //for testing purposes only (rendering in browser)
            component.isRunning = false;
            component.drawGraph(stats);
        });
        expect(component.zoom).toBe('month');
        fixture.detectChanges()
    })
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { DataInterchangeService } from './services/data-interchange.service';
import { TranslationService } from './services/translation.service';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { ConfigLoader, ConfigModule, ConfigService } from '@ngx-config/core';
import { HttpClient } from '@angular/common/http';
import { ConfigHttpLoader } from '@ngx-config/http-loader';
import { configFactory } from './app.module';


// class MockTransService extends TranslationService{
//   public isAuthenticated(){
//     return true;
//   }
// }

fdescribe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let translationService: TranslationService;
  let dataInterchangeService : DataInterchangeService;
  let httpTestCtrl: HttpTestingController;
  let config: ConfigService;
  // var data = require('../assets/config/default.json');
// const mock = require('../assets/config/default.json') as ConfigService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule, FormsModule],
      declarations: [ AppComponent ],
      providers :[ DataInterchangeService, TranslationService,ConfigService,ConfigLoader
      //   ,{
      //    provide: ConfigService, useValue: mock 
      // }
        ]
    }).compileComponents();
    
    // TestBed.overrideComponent(
    //   AppComponent,
    //   {set: {providers: [{ provide: TranslationService, useClass: MockTransService }]}}
    // );

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    translationService = TestBed.inject(TranslationService);
  }));

  beforeEach(() => {
    // fixture = TestBed.createComponent(AppComponent);
    //  translationService = TestBed.inject(TranslationService);
     dataInterchangeService= TestBed.inject(DataInterchangeService); 
     httpTestCtrl = TestBed.inject(HttpTestingController);
  
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

afterEach(() => {
  httpTestCtrl.verify();
});

  //should check instance of service with 1st injection method
  // it('should check the instance of the service', () =>{
  //     expect(translationService instanceof TranslationService).toBeTruthy();
  // });

  // //Checking if we can call object from JSON file
  // it('should create the app', () => {
  //   // let newData = data.foundationServices.translationServiceUrl;
  //   // console.log("----newData",newData);

  //   expect(component).toBeTruthy();
  // });

  // // Mock Services using component overriding
  // it('should test injected service injected using component overiding', () =>{
  //   let overRiddenService = fixture.debugElement.injector.get(TranslationService);
  //   expect(overRiddenService instanceof MockTransService).toBeTruthy();
  // });

  // // should check if property exist 
  // it(`should have isLogedIn property`, () => {
  //   expect(component.isLogedIn).toBeFalsy;
  // });

  // it('should render title', () => {
  //   const fixture = TestBed.createComponent(AppComponent);
  //   fixture.detectChanges();
  //   const compiled = fixture.nativeElement;
  //   expect(compiled.querySelector('.content span').textContent).toContain('ngHereApp app is running!');
  // });
});

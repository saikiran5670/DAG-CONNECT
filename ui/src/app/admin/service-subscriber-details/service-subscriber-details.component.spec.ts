import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceSubscriberDetailsComponent } from './service-subscriber-details.component';
import { TranslationService } from '../../services/translation.service';
import { EmployeeService } from 'src/app/services/employee.service';
// import { HttpDataService } from '../../services/sampleService/http-data.service';
import { ConfigService, ConfigLoader } from '@ngx-config/core';

import { HttpClient, HttpHandler } from '@angular/common/http';

describe('ServiceSubscriberDetailsComponent', () => {
  let component: ServiceSubscriberDetailsComponent;
  let fixture: ComponentFixture<ServiceSubscriberDetailsComponent>;
  let translationService: TranslationService;
  let employeeService: EmployeeService;
  // let configService: ConfigService;
  // let httpClient: HttpClient;
  // let httpHandler: HttpHandler;


  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceSubscriberDetailsComponent ],
      providers : [ TranslationService,EmployeeService, HttpClient, ConfigService,HttpHandler,ConfigLoader ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceSubscriberDetailsComponent);
    // translationService = TestBed.inject(TranslationService);
    // employeeService= TestBed.inject(EmployeeService);
    // httpClient = TestBed.inject(HttpClient);
    // configService = TestBed.inject(ConfigService);

    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    // spyOn(translationService, 'getTranslationLabel');
    
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganisationDetailsComponent } from './organisation-details.component';
import { TranslationService } from '../../services/translation.service';
// import { HttpDataService } from '../../services/sampleService/http-data.service';
import { ConfigService, ConfigLoader } from '@ngx-config/core';

import { HttpClient, HttpHandler } from '@angular/common/http';

describe('OrganisationDetailsComponent', () => {
  let component: OrganisationDetailsComponent;
  let fixture: ComponentFixture<OrganisationDetailsComponent>;
  let translationService: TranslationService;
  // let configService: ConfigService;
  // let httpClient: HttpClient;
  // let httpHandler: HttpHandler;


  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganisationDetailsComponent ],
      providers : [ TranslationService, HttpClient, ConfigService, HttpHandler, ConfigLoader ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganisationDetailsComponent);
    // translationService = TestBed.inject(TranslationService);
    // httpClient = TestBed.inject(HttpClient);
    // configService = TestBed.inject(ConfigService);

    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

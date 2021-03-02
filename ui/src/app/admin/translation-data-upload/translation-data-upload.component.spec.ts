import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TranslationDataUploadComponent } from './translation-data-upload.component';

describe('TranslationDataUploadComponent', () => {
  let component: TranslationDataUploadComponent;
  let fixture: ComponentFixture<TranslationDataUploadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TranslationDataUploadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TranslationDataUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

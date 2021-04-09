import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonImportComponent } from './common-import.component';

describe('CommonImportComponent', () => {
  let component: CommonImportComponent;
  let fixture: ComponentFixture<CommonImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommonImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

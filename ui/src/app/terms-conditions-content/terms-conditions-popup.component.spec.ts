import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TermsConditionsPopupComponent } from './terms-conditions-popup.component';

describe('TermsConditionsPopupComponent', () => {
  let component: TermsConditionsPopupComponent;
  let fixture: ComponentFixture<TermsConditionsPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TermsConditionsPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TermsConditionsPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TermsAndConditionPopupComponent } from './terms-and-condition-popup.component';

describe('TermsAndConditionPopupComponent', () => {
  let component: TermsAndConditionPopupComponent;
  let fixture: ComponentFixture<TermsAndConditionPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TermsAndConditionPopupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TermsAndConditionPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

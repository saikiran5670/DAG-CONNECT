import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TermsConditionsManagementComponent } from './terms-conditions-management.component';

describe('TermsConditionsManagementComponent', () => {
  let component: TermsConditionsManagementComponent;
  let fixture: ComponentFixture<TermsConditionsManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TermsConditionsManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TermsConditionsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TermsConditionsContentComponent } from './terms-conditions-content.component';

describe('TermsConditionsContentComponent', () => {
  let component: TermsConditionsContentComponent;
  let fixture: ComponentFixture<TermsConditionsContentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TermsConditionsContentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TermsConditionsContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

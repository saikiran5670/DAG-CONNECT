import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TripReportPreferenceComponent } from './trip-report-preference.component';

describe('TripReportPreferenceComponent', () => {
  let component: TripReportPreferenceComponent;
  let fixture: ComponentFixture<TripReportPreferenceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TripReportPreferenceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TripReportPreferenceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

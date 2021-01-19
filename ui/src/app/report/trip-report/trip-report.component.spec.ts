import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TripReportComponent } from './trip-report.component';

describe('TripReportComponent', () => {
  let component: TripReportComponent;
  let fixture: ComponentFixture<TripReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TripReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TripReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehiclePerformanceReportComponent } from './vehicle-performance-report.component';

describe('VehiclePerformanceReportComponent', () => {
  let component: VehiclePerformanceReportComponent;
  let fixture: ComponentFixture<VehiclePerformanceReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehiclePerformanceReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehiclePerformanceReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

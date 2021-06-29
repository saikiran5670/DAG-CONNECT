import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetFuelReportComponent } from './fleet-fuel-report.component';

describe('FleetFuelReportComponent', () => {
  let component: FleetFuelReportComponent;
  let fixture: ComponentFixture<FleetFuelReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetFuelReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetFuelReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

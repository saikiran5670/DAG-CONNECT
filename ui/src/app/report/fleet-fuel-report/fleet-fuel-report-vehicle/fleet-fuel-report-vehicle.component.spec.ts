import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetFuelReportVehicleComponent } from './fleet-fuel-report-vehicle.component';

describe('FleetFuelReportVehicleComponent', () => {
  let component: FleetFuelReportVehicleComponent;
  let fixture: ComponentFixture<FleetFuelReportVehicleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetFuelReportVehicleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetFuelReportVehicleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

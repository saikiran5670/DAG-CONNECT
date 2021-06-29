import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetFuelReportDriverComponent } from './fleet-fuel-report-driver.component';

describe('FleetFuelReportDriverComponent', () => {
  let component: FleetFuelReportDriverComponent;
  let fixture: ComponentFixture<FleetFuelReportDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetFuelReportDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetFuelReportDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

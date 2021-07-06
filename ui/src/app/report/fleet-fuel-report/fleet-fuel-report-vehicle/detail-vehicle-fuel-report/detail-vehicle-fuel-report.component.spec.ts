import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailVehicleFuelReportComponent } from './detail-vehicle-fuel-report.component';

describe('DetailVehicleFuelReportComponent', () => {
  let component: DetailVehicleFuelReportComponent;
  let fixture: ComponentFixture<DetailVehicleFuelReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailVehicleFuelReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailVehicleFuelReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

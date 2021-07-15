import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailVehicleReportComponent } from './detail-vehicle-report.component';

describe('DetailVehicleReportComponent', () => {
  let component: DetailVehicleReportComponent;
  let fixture: ComponentFixture<DetailVehicleReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailVehicleReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailVehicleReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

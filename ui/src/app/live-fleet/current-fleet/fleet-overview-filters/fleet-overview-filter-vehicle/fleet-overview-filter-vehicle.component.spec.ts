import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetOverviewFilterVehicleComponent } from './fleet-overview-filter-vehicle.component';

describe('FleetOverviewFilterVehicleComponent', () => {
  let component: FleetOverviewFilterVehicleComponent;
  let fixture: ComponentFixture<FleetOverviewFilterVehicleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetOverviewFilterVehicleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetOverviewFilterVehicleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

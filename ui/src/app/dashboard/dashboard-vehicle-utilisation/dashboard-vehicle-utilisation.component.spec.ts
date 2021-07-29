import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardVehicleUtilisationComponent } from './dashboard-vehicle-utilisation.component';

describe('DashboardVehicleUtilisationComponent', () => {
  let component: DashboardVehicleUtilisationComponent;
  let fixture: ComponentFixture<DashboardVehicleUtilisationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardVehicleUtilisationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardVehicleUtilisationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleGroupManagementComponent } from './vehicle-group-management.component';

describe('VehicleGroupManagementComponent', () => {
  let component: VehicleGroupManagementComponent;
  let fixture: ComponentFixture<VehicleGroupManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleGroupManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleGroupManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

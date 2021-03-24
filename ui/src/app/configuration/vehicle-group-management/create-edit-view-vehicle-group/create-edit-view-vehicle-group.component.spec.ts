import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewVehicleGroupComponent } from './create-edit-view-vehicle-group.component';

describe('CreateEditViewVehicleGroupComponent', () => {
  let component: CreateEditViewVehicleGroupComponent;
  let fixture: ComponentFixture<CreateEditViewVehicleGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewVehicleGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewVehicleGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

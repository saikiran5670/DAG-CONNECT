import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditVehicleDetailsComponent } from './create-edit-vehicle-details.component';

describe('CreateEditVehicleDetailsComponent', () => {
  let component: CreateEditVehicleDetailsComponent;
  let fixture: ComponentFixture<CreateEditVehicleDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditVehicleDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditVehicleDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

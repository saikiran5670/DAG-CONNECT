import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewVehicleAccessRelationshipComponent } from './create-edit-view-vehicle-access-relationship.component';

describe('CreateEditViewVehicleAccessRelationshipComponent', () => {
  let component: CreateEditViewVehicleAccessRelationshipComponent;
  let fixture: ComponentFixture<CreateEditViewVehicleAccessRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewVehicleAccessRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewVehicleAccessRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

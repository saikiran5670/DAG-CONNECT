import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewVehicleAccountAccessRelationshipComponent } from './create-edit-view-vehicle-account-access-relationship.component';

describe('CreateEditViewVehicleAccountAccessRelationshipComponent', () => {
  let component: CreateEditViewVehicleAccountAccessRelationshipComponent;
  let fixture: ComponentFixture<CreateEditViewVehicleAccountAccessRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewVehicleAccountAccessRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewVehicleAccountAccessRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

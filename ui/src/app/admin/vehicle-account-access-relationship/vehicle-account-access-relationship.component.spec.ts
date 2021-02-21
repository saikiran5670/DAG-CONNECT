import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleAccountAccessRelationshipComponent } from './vehicle-account-access-relationship.component';

describe('VehicleAccountAccessRelationshipComponent', () => {
  let component: VehicleAccountAccessRelationshipComponent;
  let fixture: ComponentFixture<VehicleAccountAccessRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleAccountAccessRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleAccountAccessRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

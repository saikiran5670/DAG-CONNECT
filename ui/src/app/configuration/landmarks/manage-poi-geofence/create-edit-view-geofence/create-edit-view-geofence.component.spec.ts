import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewGeofenceComponent } from './create-edit-view-geofence.component';

describe('CreateEditViewGeofenceComponent', () => {
  let component: CreateEditViewGeofenceComponent;
  let fixture: ComponentFixture<CreateEditViewGeofenceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewGeofenceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewGeofenceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

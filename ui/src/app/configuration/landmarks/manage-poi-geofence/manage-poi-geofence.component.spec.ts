import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagePoiGeofenceComponent } from './manage-poi-geofence.component';

describe('ManagePoiGeofenceComponent', () => {
  let component: ManagePoiGeofenceComponent;
  let fixture: ComponentFixture<ManagePoiGeofenceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManagePoiGeofenceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManagePoiGeofenceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

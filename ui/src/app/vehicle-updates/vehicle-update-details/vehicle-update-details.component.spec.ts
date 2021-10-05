import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleUpdateDetailsComponent } from './vehicle-update-details.component';

describe('VehicleUpdateDetailsComponent', () => {
  let component: VehicleUpdateDetailsComponent;
  let fixture: ComponentFixture<VehicleUpdateDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleUpdateDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleUpdateDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleUpdatesComponent } from './vehicle-updates.component';

describe('VehicleUpdatesComponent', () => {
  let component: VehicleUpdatesComponent;
  let fixture: ComponentFixture<VehicleUpdatesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleUpdatesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleUpdatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

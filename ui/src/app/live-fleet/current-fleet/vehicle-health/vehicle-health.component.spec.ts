import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleHealthComponent } from './vehicle-health.component';

describe('VehicleHealthComponent', () => {
  let component: VehicleHealthComponent;
  let fixture: ComponentFixture<VehicleHealthComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleHealthComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleHealthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetFuelPreferencesComponent } from './fleet-fuel-preferences.component';

describe('FleetFuelPreferencesComponent', () => {
  let component: FleetFuelPreferencesComponent;
  let fixture: ComponentFixture<FleetFuelPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetFuelPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetFuelPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

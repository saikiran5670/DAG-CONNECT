import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FuelDeviationPreferencesComponent } from './fuel-deviation-preferences.component';

describe('FuelDeviationPreferencesComponent', () => {
  let component: FuelDeviationPreferencesComponent;
  let fixture: ComponentFixture<FuelDeviationPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FuelDeviationPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FuelDeviationPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

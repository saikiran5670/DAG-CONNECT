import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FuelBenchmarkPreferencesComponent } from './fuel-benchmark-preferences.component';

describe('FuelBenchmarkPreferencesComponent', () => {
  let component: FuelBenchmarkPreferencesComponent;
  let fixture: ComponentFixture<FuelBenchmarkPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FuelBenchmarkPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FuelBenchmarkPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

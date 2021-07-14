import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FuelBenchmarkingComponent } from './fuel-benchmarking.component';

describe('FuelBenchmarkingComponent', () => {
  let component: FuelBenchmarkingComponent;
  let fixture: ComponentFixture<FuelBenchmarkingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FuelBenchmarkingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FuelBenchmarkingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

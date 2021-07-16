import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FuelBenchmarkingTableComponent } from './fuel-benchmarking-table.component';

describe('FuelBenchmarkingTableComponent', () => {
  let component: FuelBenchmarkingTableComponent;
  let fixture: ComponentFixture<FuelBenchmarkingTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FuelBenchmarkingTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FuelBenchmarkingTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FuelDeviationReportComponent } from './fuel-deviation-report.component';

describe('FuelDeviationReportComponent', () => {
  let component: FuelDeviationReportComponent;
  let fixture: ComponentFixture<FuelDeviationReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FuelDeviationReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FuelDeviationReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

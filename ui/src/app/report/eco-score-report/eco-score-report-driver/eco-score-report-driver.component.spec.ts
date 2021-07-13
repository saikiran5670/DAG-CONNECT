import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcoScoreReportDriverComponent } from './eco-score-report-driver.component';

describe('EcoScoreReportDriverComponent', () => {
  let component: EcoScoreReportDriverComponent;
  let fixture: ComponentFixture<EcoScoreReportDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcoScoreReportDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcoScoreReportDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

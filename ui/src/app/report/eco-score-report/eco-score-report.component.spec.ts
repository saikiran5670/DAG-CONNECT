import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcoScoreReportComponent } from './eco-score-report.component';

describe('EcoScoreReportComponent', () => {
  let component: EcoScoreReportComponent;
  let fixture: ComponentFixture<EcoScoreReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcoScoreReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcoScoreReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

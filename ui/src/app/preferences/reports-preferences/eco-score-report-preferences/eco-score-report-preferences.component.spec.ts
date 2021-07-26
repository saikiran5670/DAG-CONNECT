import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcoScoreReportPreferencesComponent } from './eco-score-report-preferences.component';

describe('EcoScoreReportPreferencesComponent', () => {
  let component: EcoScoreReportPreferencesComponent;
  let fixture: ComponentFixture<EcoScoreReportPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcoScoreReportPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcoScoreReportPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

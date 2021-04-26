import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportSchedulerComponent } from './report-scheduler.component';

describe('ReportSchedulerComponent', () => {
  let component: ReportSchedulerComponent;
  let fixture: ComponentFixture<ReportSchedulerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportSchedulerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportSchedulerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

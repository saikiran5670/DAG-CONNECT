import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewReportSchedulerComponent } from './view-report-scheduler.component';

describe('ViewReportSchedulerComponent', () => {
  let component: ViewReportSchedulerComponent;
  let fixture: ComponentFixture<ViewReportSchedulerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewReportSchedulerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewReportSchedulerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

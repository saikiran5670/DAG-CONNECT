import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewReportSchedulerComponent } from './create-edit-view-report-scheduler.component';

describe('CreateEditViewReportSchedulerComponent', () => {
  let component: CreateEditViewReportSchedulerComponent;
  let fixture: ComponentFixture<CreateEditViewReportSchedulerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewReportSchedulerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewReportSchedulerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

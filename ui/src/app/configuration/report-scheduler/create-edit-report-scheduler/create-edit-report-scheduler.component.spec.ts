import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateEditReportSchedulerComponent } from './create-edit-report-scheduler.component';


describe('CreateEditReportSchedulerComponent', () => {
  let component: CreateEditReportSchedulerComponent;
  let fixture: ComponentFixture<CreateEditReportSchedulerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditReportSchedulerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditReportSchedulerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

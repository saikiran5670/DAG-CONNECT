import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UnsubscribeReportComponent } from './unsubscribe-report.component';

describe('UnsubscribeReportComponent', () => {
  let component: UnsubscribeReportComponent;
  let fixture: ComponentFixture<UnsubscribeReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UnsubscribeReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UnsubscribeReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

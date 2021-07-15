import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailDriverReportComponent } from './detail-driver-report.component';

describe('DetailDriverReportComponent', () => {
  let component: DetailDriverReportComponent;
  let fixture: ComponentFixture<DetailDriverReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailDriverReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailDriverReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

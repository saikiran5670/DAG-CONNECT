import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetOverviewSummaryComponent } from './fleet-overview-summary.component';

describe('FleetOverviewSummaryComponent', () => {
  let component: FleetOverviewSummaryComponent;
  let fixture: ComponentFixture<FleetOverviewSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetOverviewSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetOverviewSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

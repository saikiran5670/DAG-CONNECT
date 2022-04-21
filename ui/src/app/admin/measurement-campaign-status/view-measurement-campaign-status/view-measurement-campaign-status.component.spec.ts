import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMeasurementCampaignStatusComponent } from './view-measurement-campaign-status.component';

describe('ViewMeasurementCampaignStatusComponent', () => {
  let component: ViewMeasurementCampaignStatusComponent;
  let fixture: ComponentFixture<ViewMeasurementCampaignStatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewMeasurementCampaignStatusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewMeasurementCampaignStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

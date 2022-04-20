import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MeasurementCampaignStatusComponent } from './measurement-campaign-status.component';

describe('MeasurementCampaignStatusComponent', () => {
  let component: MeasurementCampaignStatusComponent;
  let fixture: ComponentFixture<MeasurementCampaignStatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MeasurementCampaignStatusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MeasurementCampaignStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

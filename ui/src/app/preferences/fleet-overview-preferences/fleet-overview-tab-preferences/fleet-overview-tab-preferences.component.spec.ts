import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetOverviewTabPreferencesComponent } from './fleet-overview-tab-preferences.component';

describe('FleetOverviewTabPreferencesComponent', () => {
  let component: FleetOverviewTabPreferencesComponent;
  let fixture: ComponentFixture<FleetOverviewTabPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetOverviewTabPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetOverviewTabPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetOverviewPreferencesComponent } from './fleet-overview-preferences.component';

describe('FleetOverviewPreferencesComponent', () => {
  let component: FleetOverviewPreferencesComponent;
  let fixture: ComponentFixture<FleetOverviewPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetOverviewPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetOverviewPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

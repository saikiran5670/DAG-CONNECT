import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetOverviewFiltersComponent } from './fleet-overview-filters.component';

describe('FleetOverviewFiltersComponent', () => {
  let component: FleetOverviewFiltersComponent;
  let fixture: ComponentFixture<FleetOverviewFiltersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetOverviewFiltersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetOverviewFiltersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetOverviewFilterDriverComponent } from './fleet-overview-filter-driver.component';

describe('FleetOverviewFilterDriverComponent', () => {
  let component: FleetOverviewFilterDriverComponent;
  let fixture: ComponentFixture<FleetOverviewFilterDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetOverviewFilterDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetOverviewFilterDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

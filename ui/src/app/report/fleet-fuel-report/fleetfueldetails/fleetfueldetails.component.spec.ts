import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetfueldetailsComponent } from './fleetfueldetails.component';

describe('FleetfueldetailsComponent', () => {
  let component: FleetfueldetailsComponent;
  let fixture: ComponentFixture<FleetfueldetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetfueldetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetfueldetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

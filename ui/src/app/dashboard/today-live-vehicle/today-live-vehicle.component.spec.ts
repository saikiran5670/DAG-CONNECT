import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TodayLiveVehicleComponent } from './today-live-vehicle.component';

describe('TodayLiveVehicleComponent', () => {
  let component: TodayLiveVehicleComponent;
  let fixture: ComponentFixture<TodayLiveVehicleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TodayLiveVehicleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TodayLiveVehicleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

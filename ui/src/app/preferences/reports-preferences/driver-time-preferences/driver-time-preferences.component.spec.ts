import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverTimePreferencesComponent } from './driver-time-preferences.component';

describe('DriverTimePreferencesComponent', () => {
  let component: DriverTimePreferencesComponent;
  let fixture: ComponentFixture<DriverTimePreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DriverTimePreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DriverTimePreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

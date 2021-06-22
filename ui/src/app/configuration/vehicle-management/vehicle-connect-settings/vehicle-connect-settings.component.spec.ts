import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleConnectSettingsComponent } from './vehicle-connect-settings.component';

describe('VehicleConnectSettingsComponent', () => {
  let component: VehicleConnectSettingsComponent;
  let fixture: ComponentFixture<VehicleConnectSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleConnectSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleConnectSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

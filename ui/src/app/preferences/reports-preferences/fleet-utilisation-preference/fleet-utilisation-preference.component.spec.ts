import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetUtilisationPreferenceComponent } from './fleet-utilisation-preference.component';

describe('FleetUtilisationPreferenceComponent', () => {
  let component: FleetUtilisationPreferenceComponent;
  let fixture: ComponentFixture<FleetUtilisationPreferenceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetUtilisationPreferenceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetUtilisationPreferenceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

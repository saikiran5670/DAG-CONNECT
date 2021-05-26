import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardPreferencesComponent } from './dashboard-preferences.component';

describe('DashboardPreferencesComponent', () => {
  let component: DashboardPreferencesComponent;
  let fixture: ComponentFixture<DashboardPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

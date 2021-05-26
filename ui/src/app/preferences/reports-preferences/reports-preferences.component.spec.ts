import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsPreferencesComponent } from './reports-preferences.component';

describe('ReportsPreferencesComponent', () => {
  let component: ReportsPreferencesComponent;
  let fixture: ComponentFixture<ReportsPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportsPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

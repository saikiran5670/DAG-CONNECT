import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LogbookTabPreferencesComponent } from './logbook-tab-preferences.component';

describe('LogbookTabPreferencesComponent', () => {
  let component: LogbookTabPreferencesComponent;
  let fixture: ComponentFixture<LogbookTabPreferencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LogbookTabPreferencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LogbookTabPreferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

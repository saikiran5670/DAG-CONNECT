import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AlertsFilterComponent } from './alerts-filter.component';

describe('AlertsFilterComponent', () => {
  let component: AlertsFilterComponent;
  let fixture: ComponentFixture<AlertsFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AlertsFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AlertsFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

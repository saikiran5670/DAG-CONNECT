import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AlertAdvancedFilterComponent } from './alert-advanced-filter.component';

describe('AlertAdvancedFilterComponent', () => {
  let component: AlertAdvancedFilterComponent;
  let fixture: ComponentFixture<AlertAdvancedFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AlertAdvancedFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AlertAdvancedFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

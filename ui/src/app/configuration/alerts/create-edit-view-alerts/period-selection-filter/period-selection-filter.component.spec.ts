import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PeriodSelectionFilterComponent } from './period-selection-filter.component';

describe('PeriodSelectionFilterComponent', () => {
  let component: PeriodSelectionFilterComponent;
  let fixture: ComponentFixture<PeriodSelectionFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PeriodSelectionFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PeriodSelectionFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

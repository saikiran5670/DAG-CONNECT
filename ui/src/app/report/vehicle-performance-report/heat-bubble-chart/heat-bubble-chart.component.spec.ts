import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HeatBubbleChartComponent } from './heat-bubble-chart.component';

describe('HeatBubbleChartComponent', () => {
  let component: HeatBubbleChartComponent;
  let fixture: ComponentFixture<HeatBubbleChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeatBubbleChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeatBubbleChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

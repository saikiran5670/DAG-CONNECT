import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TripTracingComponent } from './trip-tracing.component';

describe('TripTracingComponent', () => {
  let component: TripTracingComponent;
  let fixture: ComponentFixture<TripTracingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TripTracingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TripTracingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

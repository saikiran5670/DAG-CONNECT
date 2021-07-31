import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetkpiComponent } from './fleetkpi.component';

describe('FleetkpiComponent', () => {
  let component: FleetkpiComponent;
  let fixture: ComponentFixture<FleetkpiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetkpiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetkpiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

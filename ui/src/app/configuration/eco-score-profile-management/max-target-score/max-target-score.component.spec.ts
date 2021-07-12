import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaxTargetScoreComponent } from './max-target-score.component';

describe('MaxTargetScoreComponent', () => {
  let component: MaxTargetScoreComponent;
  let fixture: ComponentFixture<MaxTargetScoreComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaxTargetScoreComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaxTargetScoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

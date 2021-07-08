import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BreakingScoreComponent } from './breaking-score.component';

describe('BreakingScoreComponent', () => {
  let component: BreakingScoreComponent;
  let fixture: ComponentFixture<BreakingScoreComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BreakingScoreComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BreakingScoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

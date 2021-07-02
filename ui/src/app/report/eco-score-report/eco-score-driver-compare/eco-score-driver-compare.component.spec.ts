import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcoScoreDriverCompareComponent } from './eco-score-driver-compare.component';

describe('EcoScoreDriverCompareComponent', () => {
  let component: EcoScoreDriverCompareComponent;
  let fixture: ComponentFixture<EcoScoreDriverCompareComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcoScoreDriverCompareComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcoScoreDriverCompareComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

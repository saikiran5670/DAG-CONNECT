import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EcoScoreProfileManagementComponent } from './eco-score-profile-management.component';

describe('EcoScoreProfileManagementComponent', () => {
  let component: EcoScoreProfileManagementComponent;
  let fixture: ComponentFixture<EcoScoreProfileManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EcoScoreProfileManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EcoScoreProfileManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserGroupStepComponent } from './user-group-step.component';

describe('UserGroupStepComponent', () => {
  let component: UserGroupStepComponent;
  let fixture: ComponentFixture<UserGroupStepComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserGroupStepComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserGroupStepComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

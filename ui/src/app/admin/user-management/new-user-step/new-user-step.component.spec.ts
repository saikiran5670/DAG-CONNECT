import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewUserStepComponent } from './new-user-step.component';

describe('NewUserStepComponent', () => {
  let component: NewUserStepComponent;
  let fixture: ComponentFixture<NewUserStepComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewUserStepComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewUserStepComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

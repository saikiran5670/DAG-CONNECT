import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SignalralertnotificationComponent } from './signalralertnotification.component';

describe('SignalralertnotificationComponent', () => {
  let component: SignalralertnotificationComponent;
  let fixture: ComponentFixture<SignalralertnotificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SignalralertnotificationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SignalralertnotificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

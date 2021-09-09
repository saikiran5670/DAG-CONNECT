import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SignalrAlertNotificationComponent } from './signalr-alert-notification.component';

describe('SignalrAlertNotificationComponent', () => {
  let component: SignalrAlertNotificationComponent;
  let fixture: ComponentFixture<SignalrAlertNotificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SignalrAlertNotificationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SignalrAlertNotificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

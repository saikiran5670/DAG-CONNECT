import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateNotificationsAlertComponent } from './create-notifications-alert.component';

describe('CreateNotificationsAlertComponent', () => {
  let component: CreateNotificationsAlertComponent;
  let fixture: ComponentFixture<CreateNotificationsAlertComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateNotificationsAlertComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateNotificationsAlertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

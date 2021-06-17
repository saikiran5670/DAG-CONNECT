import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationAdvancedFilterComponent } from './notification-advanced-filter.component';

describe('NotificationAdvancedFilterComponent', () => {
  let component: NotificationAdvancedFilterComponent;
  let fixture: ComponentFixture<NotificationAdvancedFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NotificationAdvancedFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotificationAdvancedFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

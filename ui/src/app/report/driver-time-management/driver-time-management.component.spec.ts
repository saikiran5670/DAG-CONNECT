import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverTimeManagementComponent } from './driver-time-management.component';

describe('DriverTimeManagementComponent', () => {
  let component: DriverTimeManagementComponent;
  let fixture: ComponentFixture<DriverTimeManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DriverTimeManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DriverTimeManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

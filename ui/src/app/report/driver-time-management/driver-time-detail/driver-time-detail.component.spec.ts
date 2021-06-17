import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverTimeDetailComponent } from './driver-time-detail.component';

describe('DriverTimeDetailComponent', () => {
  let component: DriverTimeDetailComponent;
  let fixture: ComponentFixture<DriverTimeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DriverTimeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DriverTimeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

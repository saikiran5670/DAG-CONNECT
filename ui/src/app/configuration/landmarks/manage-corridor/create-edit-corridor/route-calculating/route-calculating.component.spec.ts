import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RouteCalculatingComponent } from './route-calculating.component';

describe('RouteCalculatingComponent', () => {
  let component: RouteCalculatingComponent;
  let fixture: ComponentFixture<RouteCalculatingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RouteCalculatingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RouteCalculatingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

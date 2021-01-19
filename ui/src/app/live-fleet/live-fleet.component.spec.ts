import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LiveFleetComponent } from './live-fleet.component';

describe('MapComponent', () => {
  let component: LiveFleetComponent;
  let fixture: ComponentFixture<LiveFleetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LiveFleetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LiveFleetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

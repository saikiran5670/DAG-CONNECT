import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LiveFleetMapComponent } from './live-fleet-map.component';

describe('LiveFleetMapComponent', () => {
  let component: LiveFleetMapComponent;
  let fixture: ComponentFixture<LiveFleetMapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LiveFleetMapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LiveFleetMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

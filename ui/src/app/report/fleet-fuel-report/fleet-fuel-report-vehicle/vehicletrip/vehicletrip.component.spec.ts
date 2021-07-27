import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicletripComponent } from './vehicletrip.component';

describe('VehicletripComponent', () => {
  let component: VehicletripComponent;
  let fixture: ComponentFixture<VehicletripComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicletripComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicletripComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

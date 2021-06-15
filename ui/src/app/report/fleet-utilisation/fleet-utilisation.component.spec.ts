import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FleetUtilisationComponent } from './fleet-utilisation.component';

describe('FleetUtilisationComponent', () => {
  let component: FleetUtilisationComponent;
  let fixture: ComponentFixture<FleetUtilisationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FleetUtilisationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FleetUtilisationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

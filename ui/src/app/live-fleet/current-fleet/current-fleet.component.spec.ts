import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentFleetComponent } from './current-fleet.component';

describe('CurrentFleetComponent', () => {
  let component: CurrentFleetComponent;
  let fixture: ComponentFixture<CurrentFleetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CurrentFleetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CurrentFleetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageCorridorComponent } from './manage-corridor.component';

describe('ManageCorridorComponent', () => {
  let component: ManageCorridorComponent;
  let fixture: ComponentFixture<ManageCorridorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManageCorridorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageCorridorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

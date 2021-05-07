import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditCorridorComponent } from './create-edit-corridor.component';

describe('CreateEditCorridorComponent', () => {
  let component: CreateEditCorridorComponent;
  let fixture: ComponentFixture<CreateEditCorridorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditCorridorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditCorridorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

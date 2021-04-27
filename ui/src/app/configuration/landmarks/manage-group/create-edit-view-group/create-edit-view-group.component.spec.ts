import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewGroupComponent } from './create-edit-view-group.component';

describe('CreateEditViewGroupComponent', () => {
  let component: CreateEditViewGroupComponent;
  let fixture: ComponentFixture<CreateEditViewGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

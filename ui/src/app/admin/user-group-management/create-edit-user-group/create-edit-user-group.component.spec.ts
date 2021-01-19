import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditUserGroupComponent } from './create-edit-user-group.component';

describe('CreateEditUserGroupComponent', () => {
  let component: CreateEditUserGroupComponent;
  let fixture: ComponentFixture<CreateEditUserGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditUserGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditUserGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

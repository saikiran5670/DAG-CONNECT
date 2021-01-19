import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditUserRoleDetailsComponent } from './edit-user-role-details.component';

describe('EditUserRoleDetailsComponent', () => {
  let component: EditUserRoleDetailsComponent;
  let fixture: ComponentFixture<EditUserRoleDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditUserRoleDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditUserRoleDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

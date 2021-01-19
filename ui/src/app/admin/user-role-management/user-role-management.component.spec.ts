import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserRoleManagementComponent } from './user-role-management.component';

describe('UserRoleManagementComponent', () => {
  let component: UserRoleManagementComponent;
  let fixture: ComponentFixture<UserRoleManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserRoleManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserRoleManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

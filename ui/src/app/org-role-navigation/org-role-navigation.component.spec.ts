import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrgRoleNavigationComponent } from './org-role-navigation.component';

describe('OrgRoleNavigationComponent', () => {
  let component: OrgRoleNavigationComponent;
  let fixture: ComponentFixture<OrgRoleNavigationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrgRoleNavigationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrgRoleNavigationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

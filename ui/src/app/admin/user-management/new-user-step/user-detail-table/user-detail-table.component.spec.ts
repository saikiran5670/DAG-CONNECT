import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserDetailTableComponent } from './user-detail-table.component';

describe('UserDetailTableComponent', () => {
  let component: UserDetailTableComponent;
  let fixture: ComponentFixture<UserDetailTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserDetailTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserDetailTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

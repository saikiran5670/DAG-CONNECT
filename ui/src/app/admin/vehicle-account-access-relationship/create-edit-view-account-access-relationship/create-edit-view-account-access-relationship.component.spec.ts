import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewAccountAccessRelationshipComponent } from './create-edit-view-account-access-relationship.component';

describe('CreateEditViewAccountAccessRelationshipComponent', () => {
  let component: CreateEditViewAccountAccessRelationshipComponent;
  let fixture: ComponentFixture<CreateEditViewAccountAccessRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewAccountAccessRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewAccountAccessRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

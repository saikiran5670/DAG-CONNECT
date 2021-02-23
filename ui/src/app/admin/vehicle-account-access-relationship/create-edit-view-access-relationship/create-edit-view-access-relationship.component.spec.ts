import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewAccessRelationshipComponent } from './create-edit-view-access-relationship.component';

describe('CreateEditViewAccessRelationshipComponent', () => {
  let component: CreateEditViewAccessRelationshipComponent;
  let fixture: ComponentFixture<CreateEditViewAccessRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewAccessRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewAccessRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

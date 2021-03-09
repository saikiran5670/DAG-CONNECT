import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewOrganisationRelationshipComponent } from './create-edit-view-organisation-relationship.component';

describe('CreateEditViewOrganisationRelationshipComponent', () => {
  let component: CreateEditViewOrganisationRelationshipComponent;
  let fixture: ComponentFixture<CreateEditViewOrganisationRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewOrganisationRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewOrganisationRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

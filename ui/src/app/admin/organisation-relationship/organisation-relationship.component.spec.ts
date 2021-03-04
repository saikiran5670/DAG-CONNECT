import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganisationRelationshipComponent } from './organisation-relationship.component';

describe('OrganisationRelationshipComponent', () => {
  let component: OrganisationRelationshipComponent;
  let fixture: ComponentFixture<OrganisationRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganisationRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganisationRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

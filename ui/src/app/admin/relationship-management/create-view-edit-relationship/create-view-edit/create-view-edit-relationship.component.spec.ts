import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateViewEditRelationshipComponent } from './create-view-edit-relationship.component';

describe('CreateViewEditComponent', () => {
  let component: CreateViewEditRelationshipComponent;
  let fixture: ComponentFixture<CreateViewEditRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateViewEditRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateViewEditRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

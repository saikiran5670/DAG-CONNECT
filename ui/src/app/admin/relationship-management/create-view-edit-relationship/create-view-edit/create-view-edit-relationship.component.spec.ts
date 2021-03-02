import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateViewEditComponent } from './create-view-edit-relationship.component';

describe('CreateViewEditComponent', () => {
  let component: CreateViewEditComponent;
  let fixture: ComponentFixture<CreateViewEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateViewEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateViewEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

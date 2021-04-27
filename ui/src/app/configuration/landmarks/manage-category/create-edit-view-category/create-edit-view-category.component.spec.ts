import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewCategoryComponent } from './create-edit-view-category.component';

describe('CreateEditViewCategoryComponent', () => {
  let component: CreateEditViewCategoryComponent;
  let fixture: ComponentFixture<CreateEditViewCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteCategoryPopupComponent } from './delete-category-popup.component';

describe('DeleteCategoryPopupComponent', () => {
  let component: DeleteCategoryPopupComponent;
  let fixture: ComponentFixture<DeleteCategoryPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeleteCategoryPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteCategoryPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

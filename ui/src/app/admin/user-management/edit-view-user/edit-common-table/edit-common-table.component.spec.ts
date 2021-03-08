import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCommonTableComponent } from './edit-common-table.component';

describe('EditCommonTableComponent', () => {
  let component: EditCommonTableComponent;
  let fixture: ComponentFixture<EditCommonTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditCommonTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCommonTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditDriverDetailsComponent } from './edit-driver-details.component';

describe('EditDriverDetailsComponent', () => {
  let component: EditDriverDetailsComponent;
  let fixture: ComponentFixture<EditDriverDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditDriverDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditDriverDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

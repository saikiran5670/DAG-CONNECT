import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditPackageDetailsComponent } from './create-edit-package-details.component';

describe('CreateEditPackageDetailsComponent', () => {
  let component: CreateEditPackageDetailsComponent;
  let fixture: ComponentFixture<CreateEditPackageDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditPackageDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditPackageDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditFeaturesComponent } from './create-edit-features.component';

describe('CreateEditFeaturesComponent', () => {
  let component: CreateEditFeaturesComponent;
  let fixture: ComponentFixture<CreateEditFeaturesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditFeaturesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditFeaturesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

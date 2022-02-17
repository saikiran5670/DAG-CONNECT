import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateEditViewFeaturesComponent } from './create-edit-view-features.component';

describe('CreateEditViewFeaturesComponent', () => {
  let component: CreateEditViewFeaturesComponent;
  let fixture: ComponentFixture<CreateEditViewFeaturesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewFeaturesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewFeaturesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

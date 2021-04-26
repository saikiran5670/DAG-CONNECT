import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewPoiComponent } from './create-edit-view-poi.component';

describe('CreateEditViewPoiComponent', () => {
  let component: CreateEditViewPoiComponent;
  let fixture: ComponentFixture<CreateEditViewPoiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewPoiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewPoiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

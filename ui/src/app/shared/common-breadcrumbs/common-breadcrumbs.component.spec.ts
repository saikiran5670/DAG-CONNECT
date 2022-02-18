import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonBreadcrumbsComponent } from './common-breadcrumbs.component';

describe('CommonBreadcrumbsComponent', () => {
  let component: CommonBreadcrumbsComponent;
  let fixture: ComponentFixture<CommonBreadcrumbsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommonBreadcrumbsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonBreadcrumbsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

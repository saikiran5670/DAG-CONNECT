import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkOrgPopupComponent } from './link-org-popup.component';

describe('LinkOrgPopupComponent', () => {
  let component: LinkOrgPopupComponent;
  let fixture: ComponentFixture<LinkOrgPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkOrgPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkOrgPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

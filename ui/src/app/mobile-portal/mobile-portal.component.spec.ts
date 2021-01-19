import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobilePortalComponent } from './mobile-portal.component';

describe('MobilePortalComponent', () => {
  let component: MobilePortalComponent;
  let fixture: ComponentFixture<MobilePortalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobilePortalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobilePortalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

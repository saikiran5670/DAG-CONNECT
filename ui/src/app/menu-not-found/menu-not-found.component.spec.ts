import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuNotFoundComponent } from './menu-not-found.component';

describe('MenuNotFoundComponent', () => {
  let component: MenuNotFoundComponent;
  let fixture: ComponentFixture<MenuNotFoundComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MenuNotFoundComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MenuNotFoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

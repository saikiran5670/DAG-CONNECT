import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveInactiveDailogComponent } from './active-inactive-dailog.component';

describe('ActiveInactiveDailogComponent', () => {
  let component: ActiveInactiveDailogComponent;
  let fixture: ComponentFixture<ActiveInactiveDailogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActiveInactiveDailogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActiveInactiveDailogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

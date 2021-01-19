import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsentOptComponent } from './consent-opt.component';

describe('ConsentOptComponent', () => {
  let component: ConsentOptComponent;
  let fixture: ComponentFixture<ConsentOptComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsentOptComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsentOptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsentSingleOptComponent } from './consent-single-opt.component';

describe('ConsentSingleOptComponent', () => {
  let component: ConsentSingleOptComponent;
  let fixture: ComponentFixture<ConsentSingleOptComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsentSingleOptComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsentSingleOptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

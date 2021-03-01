import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsentAllOptComponent } from './consent-all-opt.component';

describe('ConsentOptComponent', () => {
  let component: ConsentAllOptComponent;
  let fixture: ComponentFixture<ConsentAllOptComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsentAllOptComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsentAllOptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

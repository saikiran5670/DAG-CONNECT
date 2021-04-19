import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DtcTranslationComponent } from './dtc-translation.component';

describe('DtcTranslationComponent', () => {
  let component: DtcTranslationComponent;
  let fixture: ComponentFixture<DtcTranslationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DtcTranslationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DtcTranslationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

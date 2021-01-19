import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TachographComponent } from './tachograph.component';

describe('TachographComponent', () => {
  let component: TachographComponent;
  let fixture: ComponentFixture<TachographComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TachographComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TachographComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

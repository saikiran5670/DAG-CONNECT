import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonMapComponent } from './common-map.component';

describe('CommonMapComponent', () => {
  let component: CommonMapComponent;
  let fixture: ComponentFixture<CommonMapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommonMapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

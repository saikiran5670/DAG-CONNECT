import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExistingTripsComponent } from './existing-trips.component';

describe('ExistingTripsComponent', () => {
  let component: ExistingTripsComponent;
  let fixture: ComponentFixture<ExistingTripsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExistingTripsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExistingTripsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

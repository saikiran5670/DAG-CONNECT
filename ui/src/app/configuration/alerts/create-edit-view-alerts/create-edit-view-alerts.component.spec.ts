import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewAlertsComponent } from './create-edit-view-alerts.component';

describe('CreateEditViewAlertsComponent', () => {
  let component: CreateEditViewAlertsComponent;
  let fixture: ComponentFixture<CreateEditViewAlertsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditViewAlertsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewAlertsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

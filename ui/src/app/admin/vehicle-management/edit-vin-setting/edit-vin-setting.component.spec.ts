import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditVINSettingComponent } from './edit-vin-setting.component';

describe('EditVINSettingComponent', () => {
  let component: EditVINSettingComponent;
  let fixture: ComponentFixture<EditVINSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditVINSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditVINSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

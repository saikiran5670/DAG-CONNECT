import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TitleComponent } from './title.component';

fdescribe('TitleComponent', () => {
  let component: TitleComponent;
  let fixture: ComponentFixture<TitleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TitleComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TitleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should render passed @input correctly', () => {
    component.message = 'Enter a new title';
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('p').textContent).toBe('Enter a new title');
  });
  it('should render  @Output correctly', () => {
    spyOn(component.changeTitleEvent, 'emit');
    const button=fixture.nativeElement.querySelector('button');
    fixture.nativeElement.querySelector('input').value='A new title';
    const inputText = fixture.nativeElement.querySelector('input').value;
    button.click();
    fixture.detectChanges();
    expect(component.changeTitleEvent.emit).toHaveBeenCalledWith(inputText);
  });
  //check whether method has been called or not
  // it('should call on button click', () => {
  //   spyOn(component, 'handleButtonClick')
  //   expect(component.handleButtonClick).toHaveBeenCalled();

  // });
});

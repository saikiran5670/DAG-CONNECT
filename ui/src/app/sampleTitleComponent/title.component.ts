import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'app-title',
  template: `
    <p>{{ message }}</p>
    <input #titleField type="text" />
    <button (click)="handleButtonClick(titleField.value)">Change Title</button>
  `,
  styles: [`h1 { font-family: Lato; }`]
  //templateUrl: './title.component.html',
  //styleUrls: ['./title.component.css']
})
export class TitleComponent implements OnInit {
  @Input() message: string;
  @Output() changeTitleEvent: EventEmitter<string> = new EventEmitter();
  @ViewChild('titleField') titleField: ElementRef;
  constructor() { }

  ngOnInit(): void {
  }
  handleButtonClick(newTitle:any) {
    if (newTitle) {
      this.changeTitleEvent.emit(newTitle);
      this.titleField.nativeElement.value='';
    }
  }

}

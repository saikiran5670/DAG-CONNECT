import { Component, Input, Output, EventEmitter,OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-corridor',
  templateUrl: './manage-corridor.component.html',
  styleUrls: ['./manage-corridor.component.less']
})

export class ManageCorridorComponent implements OnInit {
  @Input() translationData: any;
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  createEditStatus = false;
  actionType : string;
  constructor() { }

  ngOnInit(){ }

  onNewCorridor(){
    this.actionType = "create";
    this.createEditStatus = true;
    this.tabVisibility.emit(false);

  }

  onBackToPage(_eventObj) {
    this.createEditStatus = false;

    this.tabVisibility.emit(true);
  }
}

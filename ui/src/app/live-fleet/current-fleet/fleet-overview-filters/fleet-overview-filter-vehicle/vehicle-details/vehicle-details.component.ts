import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { InputEditor } from 'angular-slickgrid';

@Component({
  selector: 'app-vehicle-details',
  templateUrl: './vehicle-details.component.html',
  styleUrls: ['./vehicle-details.component.less']
})
export class VehicleDetailsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() selectedElementData: any;
  @Input() translationData: any;
  gridData: any = [];
  constructor() { }

  ngOnInit(): void {
    this.gridData = this.selectedElementData;
  }

  toBack() {
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.backToPage.emit(emitObj);
  }

  gotoHealthStatus(data: any){

  }

}

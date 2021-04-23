import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-create-edit-view-poi',
  templateUrl: './create-edit-view-poi.component.html',
  styleUrls: ['./create-edit-view-poi.component.less']
})
export class CreateEditViewPoiComponent implements OnInit {
@Output() createViewEditPoiEmit = new EventEmitter<object>();
breadcumMsg: any = ''; 
@Input() actionType: any;

  constructor() { }

  ngOnInit(): void {
    console.log("child compo called");
    this.breadcumMsg = this.getBreadcum(this.actionType);
  }

  getBreadcum(type: any){
    return `Home / Configuration / Landmarks / Add new POI`;
  }

}
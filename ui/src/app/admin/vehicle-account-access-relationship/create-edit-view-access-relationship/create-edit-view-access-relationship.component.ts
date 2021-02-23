import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-create-edit-view-access-relationship',
  templateUrl: './create-edit-view-access-relationship.component.html',
  styleUrls: ['./create-edit-view-access-relationship.component.less']
})
export class CreateEditViewAccessRelationshipComponent implements OnInit {
  @Input() translationData: any;
  breadcumMsg: any = '';  
  @Output() userCreate = new EventEmitter<object>();
  
  constructor() { }

  ngOnInit() {
    this.breadcumMsg = this.getBreadcum();
  }

  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblVehicleAccountAccessRelationship ? this.translationData.lblVehicleAccountAccessRelationship : "Vehicle/Account Access-Relationship"} / ${this.translationData.lblAccessRelationshipDetails ? this.translationData.lblAccessRelationshipDetails : 'Access Relationship Details'}`;
  }

  toBack(){
    let emitObj = {
      stepFlag: false,
      msg: ""
    }    
    this.userCreate.emit(emitObj);    
  }

}
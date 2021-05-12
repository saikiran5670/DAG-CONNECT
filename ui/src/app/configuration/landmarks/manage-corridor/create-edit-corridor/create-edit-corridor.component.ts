import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../shared/custom.validators';


@Component({
  selector: 'app-create-edit-corridor',
  templateUrl: './create-edit-corridor.component.html',
  styleUrls: ['./create-edit-corridor.component.less']
})
export class CreateEditCorridorComponent implements OnInit {
  @Input() translationData: any;
  @Input() actionType: any;
  @Output() backToPage = new EventEmitter<any>();
  breadcumMsg: any = '';
  organizationId: number;
  localStLanguage: any;
  accountId: any = 0;
  corridorTypeList = [{id:1,value:'Route Calculating'},{id:2,value:'Existing Trips'}];
  selectedCorridorTypeId : any = 1;


  constructor() {
   }

  ngOnInit(): void {
    this.breadcumMsg = this.getBreadcum();
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
  

  }

  
  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditCorridorDetails ? this.translationData.lblEditCorridorDetails : 'Edit Corridor Details') : (this.actionType == 'view') ? (this.translationData.lblViewCorridorDetails ? this.translationData.lblViewCorridorDetails : 'View Corridor Details') : (this.translationData.lblCreateNewCorridor ? this.translationData.lblCreateNewCorridor : 'Create New Corridor')}`;
  }

  corridorTypeChanged(_event){
    this.selectedCorridorTypeId = _event.value;
  }

  backToCorridorList(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

}

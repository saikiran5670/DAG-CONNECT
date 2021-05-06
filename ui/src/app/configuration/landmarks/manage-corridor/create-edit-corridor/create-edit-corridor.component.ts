import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

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
  corridorForm: FormGroup;
  corridorTypeList = [{id:1,value:'Route Calculating'},{id:2,value:'Route Calculating'}];
  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.breadcumMsg = this.getBreadcum();
    this.corridorForm = this.formBuilder.group({
      corridorType:['Regular']
    })
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditCorridorDetails ? this.translationData.lblEditCorridorDetails : 'Edit Corridor Details') : (this.actionType == 'view') ? (this.translationData.lblViewCorridorDetails ? this.translationData.lblViewCorridorDetails : 'View Corridor Details') : (this.translationData.lblCreateNewCorridor ? this.translationData.lblCreateNewCorridor : 'Create New Corridor')}`;
  }

  backToCorridorList(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

}

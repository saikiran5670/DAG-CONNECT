import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../shared/custom.validators';

@Component({
  selector: 'app-create-edit-view-poi',
  templateUrl: './create-edit-view-poi.component.html',
  styleUrls: ['./create-edit-view-poi.component.less']
})
export class CreateEditViewPoiComponent implements OnInit {
  @Output() createViewEditPoiEmit = new EventEmitter<object>();
  @Input() createStatus: boolean;
  @Input() translationData: any;
  @Input() selectedElementData: any;
  @Input() viewFlag: boolean;
  breadcumMsg: any = ''; 
  @Input() actionType: any;
  poiFormGroup: FormGroup;
  private _formBuilder: any;
  
    constructor() { }
  
    ngOnInit(): void {
      this.poiFormGroup = this._formBuilder.group({
        // category: ['', [ Validators.required, CustomValidators.noWhitespaceValidatorforDesc ]],
        // description: ['', [CustomValidators.noWhitespaceValidatorforDesc]],
        // state: ['', [CustomValidators.numberValidationForName]],
        category: ['', [ Validators.required]],
        sub_category: ['', [ Validators.required]],
        name: ['', [ Validators.required, CustomValidators.noWhitespaceValidatorforDesc]]
      },
      {
        validator: [
          // CustomValidators.specialCharValidationForName('code'),
          CustomValidators.specialCharValidationForName('name'),
          // CustomValidators.specialCharValidationForNameWithoutRequired('description')
        ]
      });
      this.breadcumMsg = this.getBreadcum(this.actionType);
    }

    toBack(){
      let emitObj = {
        stepFlag: false,
        msg: ""
      }    
      this.createViewEditPoiEmit.emit(emitObj);    
    }
  
    getBreadcum(type: any){
      return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / ${(type == 'view') ? (this.translationData.lblViewPOI ? this.translationData.lblViewPOI : 'View POI Details') : (type == 'edit') ? (this.translationData.lblEditPOI ? this.translationData.lblEditPOI : 'Edit POI Details') : (this.translationData.lblPOIDetails ? this.translationData.lblPOIDetails : 'Add New POI')}`;
    }
}

import { Options } from '@angular-slider/ngx-slider';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-breaking-score',
  templateUrl: './breaking-score.component.html',
  styleUrls: ['./breaking-score.component.less']
})
export class BreakingScoreComponent implements OnInit {

  ecoScoreProfileKPIForm: FormGroup;
  @Input() actionType: any;
  @Input() selectedElementData: any;
  @Input() kpiId: any;
  @Input() translationData: any;
  @Input() otherWtKpiVal: any;
  @Output() createKPIEmit = new EventEmitter<object>();
  kpiData: any = [];
  isKPI: any = true;
  array: any = [];
  
  title = 'ngx-slider';  
  value: number = this.kpiData.limitValue;  
  maxvalue: number = this.kpiData.targetValue;
  options: Options = {  
        floor: 0.01,  
        ceil: 0.01,
        step: 0.01,  
        showTicks: true,
        tickStep: 0,
        showOuterSelectionBars: true,
  };  

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.kpiData = this.selectedElementData;
    this.value = this.kpiData.limitValue;
    this.maxvalue =  this.kpiData.targetValue;
    this.options.floor = this.kpiData.lowerValue;
    this.options.ceil = this.kpiData.upperValue;
    this.options.step = 0.01,  
    this.options.showTicks= true,
    this.options.tickStep = this.kpiData.upperValue - this.kpiData.lowerValue > 1000 ? 100 : this.kpiData.upperValue - this.kpiData.lowerValue> 100  && this.kpiData.upperValue - this.kpiData.lowerValue < 1000 ? 50 : this.kpiData.upperValue - this.kpiData.lowerValue  > 50  && this.kpiData.upperValue - this.kpiData.lowerValue <= 100 ? 10 : 1 ;  
    this.SliderData();
  }

  SliderData(){
    this.ecoScoreProfileKPIForm = this._formBuilder.group({
      lowerValue: [''],
      upperValue: [''],
      limitValue: [''],
      targetValue: [''],
  }, {
    validator: [
      CustomValidators.numberFieldValidation('lowerValue', this.value),
      CustomValidators.numberFieldValidation('upperValue',this.kpiData.maxUpperValue),
      CustomValidators.numberFieldValidation('limitValue',this.maxvalue),
      CustomValidators.numberFieldValidation('targetValue',this.options.ceil),
      CustomValidators.numberMinFieldValidation('lowerValue', 0),
      CustomValidators.numberMinFieldValidation('upperValue',this.maxvalue),
      CustomValidators.numberMinFieldValidation('limitValue',this.options.floor),
      CustomValidators.numberMinFieldValidation('targetValue',this.value),
    ]
  });
    this.isKPI = true;
    this.setDefaultValue();
  }

  setDefaultValue(){
    this.ecoScoreProfileKPIForm.get("lowerValue").setValue(this.options.floor);
    this.ecoScoreProfileKPIForm.get("upperValue").setValue(this.options.ceil);
    this.ecoScoreProfileKPIForm.get("limitValue").setValue(this.value);
    this.ecoScoreProfileKPIForm.get("targetValue").setValue(this.maxvalue);
    this.sendData();
  }

  sendData(){
    let emitObj = {
      "kpiId": this.kpiId,
      "limitType": "N",
      "limitValue":this.ecoScoreProfileKPIForm.controls.limitValue.value ? this.ecoScoreProfileKPIForm.controls.limitValue.value : 0,
      "targetValue":this.ecoScoreProfileKPIForm.controls.targetValue.value ? this.ecoScoreProfileKPIForm.controls.targetValue.value : 0,
      "lowerValue": this.ecoScoreProfileKPIForm.controls.lowerValue.value ? this.ecoScoreProfileKPIForm.controls.lowerValue.value : 0,
      "upperValue": this.ecoScoreProfileKPIForm.controls.upperValue.value ? this.ecoScoreProfileKPIForm.controls.upperValue.value : 0
    }
    this.createKPIEmit.emit(emitObj);
  }

  sliderEvent(value: any){
    this.ecoScoreProfileKPIForm.get("limitValue").setValue(value);
    this.sendData();
    this.SliderData()
   }
 
   sliderEndEvent(endValue: any){
   this.ecoScoreProfileKPIForm.get("targetValue").setValue(endValue);
  this.sendData();
  this.SliderData()
   }
 
   changeMin(changedVal: any){
    this.value = changedVal;
    this.sendData();
    this.SliderData();
   }
 
   changeTarget(changedVal: any){
     this.maxvalue = changedVal;
    this.sendData();
    this.SliderData();
   }
 
   changeLower(changedVal: any){
     const newOptions: Options = Object.assign({}, this.options);
     newOptions.floor = parseFloat(changedVal);
     this.options = newOptions;
    this.sendData();
    this.SliderData();
   }
 
   changeUpper(changedVal: any){
     const newOptions: Options = Object.assign({}, this.options);
     newOptions.ceil = parseFloat(changedVal);
     this.options = newOptions;
    this.sendData();
    this.SliderData();
   }

}

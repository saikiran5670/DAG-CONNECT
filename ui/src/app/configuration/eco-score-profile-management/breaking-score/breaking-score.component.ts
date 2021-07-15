import { Options } from '@angular-slider/ngx-slider';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

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
  // @Input() createStatus: boolean;
  // @Input() viewFlag: boolean;
  @Output() createKPIEmit = new EventEmitter<object>();
  kpiData: any = [];
  isKPI: any = true;
  array: any = [];
  
  title = 'ngx-slider';  
  value: number = this.kpiData.limitValue;  
  maxvalue: number = this.kpiData.targetValue;
  options: Options = {  
        floor: this.kpiData.lowerValue,  
        ceil: this.kpiData.upperValue,
        step: 4,  
        showTicks: true, 
        showOuterSelectionBars: true,
  };  

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.ecoScoreProfileKPIForm = this._formBuilder.group({
      lowerValue: [''],
      upperValue: [''],
      limitValue: [''],
      targetValue: [''],
  });
  this.SliderData();
  // if(this.isCreate){
  //   this.sendData()
  // }
  }

  SliderData(){
    this.kpiData = this.selectedElementData;
    this.value = this.kpiData.limitValue;
    this.maxvalue =  this.kpiData.targetValue;
    this.options.floor = this.kpiData.lowerValue;
    this.options.ceil = this.kpiData.upperValue;
    this.options.step = this.kpiData.upperValue/10,  
    this.options.showTicks = true  
 
    this.isKPI = true;
    this.setDefaultValue();
  }

  setDefaultValue(){
    if(this.actionType == 'manage'){
    this.ecoScoreProfileKPIForm.get("lowerValue").setValue(this.options.floor);
    this.ecoScoreProfileKPIForm.get("upperValue").setValue(this.options.ceil);
    this.ecoScoreProfileKPIForm.get("limitValue").setValue(this.value);
    this.ecoScoreProfileKPIForm.get("targetValue").setValue(this.maxvalue);
    }
    else {
      this.ecoScoreProfileKPIForm.get("lowerValue").setValue('');
    this.ecoScoreProfileKPIForm.get("upperValue").setValue('');
    this.ecoScoreProfileKPIForm.get("limitValue").setValue('');
    this.ecoScoreProfileKPIForm.get("targetValue").setValue('');
    }
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
   }
 
   sliderEndEvent(endValue: any){
   this.ecoScoreProfileKPIForm.get("targetValue").setValue(endValue);
  this.sendData();
   }
 
   changeMin(changedVal: any){
     if(changedVal < 0){
       this.value = 0;
     }else 
    this.value = changedVal;
    this.sendData();
   }
 
   changeTarget(changedVal: any){
    if(changedVal < 0){
      this.maxvalue = 0;
    }else
     this.maxvalue = changedVal;
  this.sendData();
   }
 
   changeLower(changedVal: any){
     // this.options.floor = changedVal;
     const newOptions: Options = Object.assign({}, this.options);
     if(changedVal < 0){
     newOptions.floor = 0;
     this.options = newOptions;
     }else {
     newOptions.floor = changedVal;
     this.options = newOptions;
     }
    this.sendData();
   }
 
   changeUpper(changedVal: any){
     const newOptions: Options = Object.assign({}, this.options);
     if(changedVal < 0){
     newOptions.ceil = 0;
     this.options = newOptions;
     }else {
     newOptions.ceil = changedVal;
     this.options = newOptions;
     }
    
    this.sendData();
   }

}

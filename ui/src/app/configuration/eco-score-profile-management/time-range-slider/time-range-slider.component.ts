import { Options,LabelType  } from '@angular-slider/ngx-slider';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { parse } from 'path';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-time-range-slider',
  templateUrl: './time-range-slider.component.html',
  styleUrls: ['./time-range-slider.component.less']
})
export class TimeRangeSliderComponent implements OnInit {

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
  arrayMin: any = 0;
  arrayMax: any = 0;
  
  title = 'ngx-slider';  

  maxvalue: number = this.kpiData.targetValue;
  dateRange = []; 
  value: number = this.dateRange[0];  
  options: Options = {
    stepsArray: this.dateRange.map((value) => {
      return { value: value };
    }),
    translate: (value: number, label: LabelType): string => {
      return this.timeConversion(value);
    },
    floor: 0.01,  
    ceil: 0.01,
    showOuterSelectionBars: true,
    showTicks: true,
    // tickStep: 0,
  };
  

  constructor(private _formBuilder: FormBuilder) { }
  
  ngOnInit(): void {
    this.kpiData = this.selectedElementData;
    this.value = this.kpiData.targetValue;
      this.maxvalue =  this.kpiData.limitValue;
      
    this.options.tickStep = this.kpiData.upperValue - this.kpiData.lowerValue > 10000 ? 500 : this.kpiData.upperValue - this.kpiData.lowerValue > 1000 && this.kpiData.upperValue - this.kpiData.lowerValue < 10000 ? 100 : this.kpiData.upperValue - this.kpiData.lowerValue> 100  && this.kpiData.upperValue - this.kpiData.lowerValue < 1000 ? 50 : this.kpiData.upperValue - this.kpiData.lowerValue  > 50  && this.kpiData.upperValue - this.kpiData.lowerValue <= 100 ? 10 : 1 ;  
    // this.options.step = 1,  
  
    this.options.showOuterSelectionBars = true;
    // this.options.tickStep = 1 ; 
    this.arrayMin = this.kpiData.lowerValue;
    this.arrayMax = this.kpiData.upperValue;
    this.setOptions();
  }

  setOptions(){
  
    this.dateRange = this.createDateRange();
    this.options = {
      stepsArray: this.dateRange.map((value) => {
        return { value: this.backToSeconds(value) };
      }),
      translate: (value: number, label: LabelType): string => {
        return this.timeConversion(value);
      }
    };
    this.options.tickStep = this.arrayMax - this.arrayMin > 10000 ? 700 : this.arrayMax - this.arrayMin > 1000 && this.arrayMax - this.arrayMin < 10000 ? 100 : this.arrayMax - this.arrayMin > 100  && this.arrayMax - this.arrayMin < 1000 ? 50 : this.arrayMax - this.arrayMin  > 50  && this.arrayMax - this.arrayMin <= 100 ? 10 : 1 ;  
    this.options.showOuterSelectionBars = true;
    this.options.showTicks= true,
   
    this.SliderData();
  }

  createDateRange(){
    const dates = [];
    for (let i: number = this.arrayMin; i <= this.arrayMax; i++) {
      dates.push(this.timeConversion(i))
    }
    return dates;
  }

  SliderData(){
    this.ecoScoreProfileKPIForm = this._formBuilder.group({
      lowerValue: [''],
      upperValue: [''],
      limitValue: [''],
      targetValue: [''],
  }, {
    validator: [
      CustomValidators.timeMaxFieldValidation('lowerValue', this.value),
      CustomValidators.timeMaxFieldValidation('upperValue',this.kpiData.maxUpperValue == 0 ? this.options.ceil : this.kpiData.maxUpperValue ),
      CustomValidators.timeMaxFieldValidation('limitValue',this.arrayMax),
      CustomValidators.timeMaxFieldValidation('targetValue',this.maxvalue),
      CustomValidators.timeMinFieldValidation('lowerValue', 0),
      CustomValidators.timeMinFieldValidation('upperValue',this.maxvalue),
      CustomValidators.timeMinFieldValidation('limitValue',this.value),
      CustomValidators.timeMinFieldValidation('targetValue',this.arrayMin),
    ]
  });
  
    this.isKPI = true;
    this.setDefaultValue();
  }

  timeConversion(convertTime: any){
    var seconds : any = parseInt(convertTime , 10); 
    var hours : any  = Math.floor(seconds / 3600);
    var minutes : any = Math.floor((seconds - (hours * 3600)) / 60);
    seconds = seconds - (hours * 3600) - (minutes * 60);

    if (hours   < 10) {hours   = "0"+hours;}
    if (minutes < 10) {minutes = "0"+minutes;}
    if (seconds < 10) {seconds = "0"+seconds;}
    var time    = hours+':'+minutes+':'+seconds;
    return time;
  }

  backToSeconds(time: any){
    const hms = time;
    const [hours, minutes, seconds] = hms.split(':');
    const totalSeconds = (+hours) * 60 * 60 + (+minutes) * 60 + (+seconds);
    return totalSeconds;
  }

  setDefaultValue(){
    
    this.ecoScoreProfileKPIForm.get("lowerValue").setValue(this.dateRange[0]);
    this.ecoScoreProfileKPIForm.get("upperValue").setValue(this.dateRange[this.dateRange.length - 1]);
    this.ecoScoreProfileKPIForm.get("limitValue").setValue(this.timeConversion(this.maxvalue));
    this.ecoScoreProfileKPIForm.get("targetValue").setValue(this.timeConversion(this.value));

    this.sendData();
  }

  sendData(){

    let emitObj = {
      "kpiId": this.kpiId,
      "limitType": "X",
      "limitValue":this.backToSeconds(this.ecoScoreProfileKPIForm.controls.limitValue.value) ? this.backToSeconds(this.ecoScoreProfileKPIForm.controls.limitValue.value) : 0,
      "targetValue":this.backToSeconds(this.ecoScoreProfileKPIForm.controls.targetValue.value) ?this.backToSeconds(this.ecoScoreProfileKPIForm.controls.targetValue.value) : 0,
      "lowerValue": this.backToSeconds(this.ecoScoreProfileKPIForm.controls.lowerValue.value) ? this.backToSeconds(this.ecoScoreProfileKPIForm.controls.lowerValue.value ): 0,
      "upperValue": this.backToSeconds(this.ecoScoreProfileKPIForm.controls.upperValue.value) ? this.backToSeconds(this.ecoScoreProfileKPIForm.controls.upperValue.value) : 0
    }
    this.createKPIEmit.emit(emitObj);
  }

  sliderEvent(value: any){
    //this.ecoScoreProfileKPIForm.get("targetValue").setValue(value);
     this.ecoScoreProfileKPIForm.get("targetValue").setValue(this.timeConversion(value));
    this.sendData();
    this.SliderData()
  }
 
  sliderEndEvent(endValue: any){
    //this.ecoScoreProfileKPIForm.get("limitValue").setValue(endValue);
    this.ecoScoreProfileKPIForm.get("limitValue").setValue(this.timeConversion(endValue));
    this.sendData();
    this.SliderData()
  }
  
  changeMax(changedVal: any){
    this.maxvalue = this.backToSeconds(changedVal);
    this.sendData();
    this.SliderData();
  }
 
  changeTarget(changedVal: any){
    this.value = this.backToSeconds(changedVal);
    this.sendData();
    this.SliderData();
  }
 
   changeLower(changedVal: any){
     this.arrayMin= this.backToSeconds(changedVal)
     this.setOptions();
    this.sendData();
    
   }
 
   changeUpper(changedVal: any){
    this.arrayMax= this.backToSeconds(changedVal);
     this.setOptions();
    this.sendData();
   }

}

import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormGroup,FormBuilder, FormArray, FormControl } from '@angular/forms';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { Validators } from '@angular/forms';

@Component({
  selector: 'app-period-selection-filter',
  templateUrl: './period-selection-filter.component.html',
  styleUrls: ['./period-selection-filter.component.less']
})
export class PeriodSelectionFilterComponent implements OnInit {
@Input() translationData : any = [];
@Input() selectedRowData: any;
@Input() actionType: any;
isMondaySelected:  boolean= false;
periodSelectionForm: FormGroup;
localStLanguage: any;
organizationId: number;
accountId: number;
FormArrayItems:  FormArray;
days: any= [];
weekDaySelected: boolean = false;
checkboxChecked: boolean = false;

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId= parseInt(localStorage.getItem("accountId"));
    this.days= ['Sunday', 'Monday', 'tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    
    this.periodSelectionForm = this._formBuilder.group({
      // recipientLabel: ['', [ Validators.required ]],
      FormArrayItems : this._formBuilder.array([this.initPeriodItems()]),
    });

    if(this.actionType == 'create'){
    for(let i = 0; i < 6; i++ ){
    this.weekDays().push(this.initPeriodItems());
    }
  }
    else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
      for(let i = 0; i < 6; i++ ){
        this.weekDays().push(this.initPeriodItems());
        this.onDeleteCustomPeriod(i,0);
        }
    }

    if((this.actionType == 'edit' || this.actionType == 'duplicate') &&
    this.selectedRowData.alertUrgencyLevelRefs.length > 0 && 
    this.selectedRowData.alertUrgencyLevelRefs[0].alertTimingDetail.length > 0)
 {
   this.setDefaultValues();
 }
 
  }

  initPeriodItems(): FormGroup{
    return this._formBuilder.group({
      daySelection: [''],
      fulldayCustom: [''],
      FormArrayCustomItems : this._formBuilder.array([this.initCustomPeriodItems()]),
      id: []
    });
  }

  initCustomPeriodItems(): FormGroup{
    return this._formBuilder.group({
      fromTime : new FormControl('00:00'),
      toTime:  new FormControl('23:59')
    });
  }

  onChangeDaySelection(event, periodIndex){
    if(event.checked){
      this.weekDays().at(periodIndex).get("fulldayCustom").setValue('A');
      this.weekDaySelected = true;
    }
    else{
      this.weekDays().at(periodIndex).get("fulldayCustom").setValue('');
      this.weekDaySelected = false;
    }
  }
  
  onDeleteCustomPeriod(periodIndex, customIndex){
     this.customPeriods(periodIndex).removeAt(customIndex);
  }

  addCustomPeriod(periodIndex, totalTime? ,isButtonClicked?){
    if(this.actionType == 'create'){
    if(this.customPeriods(periodIndex).length < 4)
      this.customPeriods(periodIndex).push(this.initCustomPeriodItems());
    }
    else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
      if(isButtonClicked){
        if(this.customPeriods(periodIndex).length < 4){
        this.customPeriods(periodIndex).push(this.initCustomPeriodItems());}
      }
      else{
        this.customPeriods(periodIndex).push(this.setCustomPeriodItems(totalTime[0],totalTime[1]));
      }
    }
      
  }

  setCustomPeriodItems(fromTime,toTime): FormGroup{
    return this._formBuilder.group({
      fromTime : new FormControl(fromTime),
      toTime:  new FormControl(toTime)
    });
  }

  weekDays(): FormArray {
    return this.periodSelectionForm.get("FormArrayItems") as FormArray;
  }

  customPeriods(periodIndex: number) : FormArray {
    return this.weekDays().at(periodIndex).get("FormArrayCustomItems") as FormArray
  }

setDefaultValues(){
  if(this.selectedRowData.alertUrgencyLevelRefs[0].alertTimingDetail.length > 0){
  this.selectedRowData.alertUrgencyLevelRefs[0].alertTimingDetail.forEach(element => {
    // this.addMultipleItems(false,element);
  
    element.dayType.forEach((item,index) =>{
        if(item == true){
          this.checkboxChecked = true;
          this.setDayAndCustomDetails(index,element);
        }
      })  
  });
}
}

setDayAndCustomDetails(index,element){
  this.weekDays().at(index).get("daySelection").setValue('true');
  this.weekDays().at(index).get("id").setValue(element.id);
  if(element.periodType == 'A'){
    this.weekDays().at(index).get("fulldayCustom").setValue('A');
  }
  else if(element.periodType == 'C'){
    this.weekDays().at(index).get("fulldayCustom").setValue('C');
    let totalTime = this.convertTimeIntoHours(element.startDate,element.endDate);
    this.addCustomPeriod(index, totalTime);
  }
}


convertTimeIntoHours(startTime,EndTime){
  let startdateObj = new Date(startTime * 1000);
  let starthours = startdateObj.getUTCHours();
  let startminutes = startdateObj.getUTCMinutes();
  let newStartTime = starthours.toString().padStart(2, '0') + ':' + startminutes.toString().padStart(2, '0');
  let endDateobj = new Date(EndTime * 1000);
  let endhours = endDateobj.getUTCHours();
  let endminutes = endDateobj.getUTCMinutes();
  let newEndTime = endhours.toString().padStart(2, '0') + ':' + endminutes.toString().padStart(2, '0');

return [newStartTime, newEndTime]
}

getAlertTimingPayload(){
  let alertTimingRef= [];
  let weekDay : any;
  let customTime : any;
  let tempObj: any;
  this.weekDays().controls.forEach((element, index) => {
    weekDay = element['controls'];
    if (weekDay.daySelection.value) {
      if (weekDay.fulldayCustom.value == 'C') {
        this.customPeriods(index).controls.forEach(item => {
          customTime = item['controls'];
          let startTime = customTime.fromTime.value;
          let endTime = customTime.toTime.value;
          let startTimeSeconds = this.convertTimeToSeconds(startTime);
          let endTimeSeconds = this.convertTimeToSeconds(endTime);
          if(this.actionType == 'create'){
          tempObj = {
            "type": 'U',
            "refId": 0,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": 'C',
            "startDate": startTimeSeconds,
            "endDate": endTimeSeconds,
            "state": "A"
          }
        }
        else if(this.actionType == 'edit' || this.actionType == 'duplicate')
        {
          tempObj = {
            "type": "U",
            "refId": 0,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": "C",
            "startDate": startTimeSeconds,
            "endDate": endTimeSeconds,
            "state": "A",
            "id" : weekDay.id.value ? weekDay.id.value  : 0,
          }
        }
          tempObj["dayType"][index] = true;
          alertTimingRef.push(tempObj);
        })
      }
      else{
        if(this.actionType == 'create'){
        tempObj = {
          "type": 'U',
          "refId": 0,
          "dayType": [
            false, false, false, false, false, false, false
          ],
          "periodType": 'A',
          "startDate": 0,
          "endDate": 0,
          "state": "A"
        }
      }
      else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
        tempObj = {
          "type": "U",
          "refId": 0,
          "dayType": [
            false, false, false, false, false, false, false
          ],
          "periodType": "A",
          "startDate": 0,
          "endDate": 0,
          "state": "A",
          "id" : weekDay.id.value ? weekDay.id.value  : 0,
        }
      }
        tempObj["dayType"][index] = true;
        alertTimingRef.push(tempObj);
      }
    }
  })
  
  return alertTimingRef;
}

convertTimeToSeconds(time:any){
  let newstartTime= time.split(":");
  return (newstartTime[0] * 60 * 60) + (newstartTime[1] * 60);
  
}

}

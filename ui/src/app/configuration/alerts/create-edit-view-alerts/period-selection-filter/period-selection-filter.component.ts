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
isMondaySelected:  boolean= false;
periodSelectionForm: FormGroup;
localStLanguage: any;
organizationId: number;
accountId: number;
FormArrayItems:  FormArray;
days: any= [];

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

    for(let i = 1; i < 7; i++ )
      this.weekDays().push(this.initPeriodItems());
      
  }

  initPeriodItems(): FormGroup{
    return this._formBuilder.group({
      daySelection: [''],
      fulldayCustom: [''],
      FormArrayCustomItems : this._formBuilder.array([this.initCustomPeriodItems()])
    });
  }

  initCustomPeriodItems(): FormGroup{
    return this._formBuilder.group({
      fromTime : new FormControl({value: '00:00'}),
      toTime:  new FormControl({value: '00:00'})
    });
  }

  onChangeDaySelection(event, periodIndex){
    if(event.checked){
      this.weekDays().at(periodIndex).get("fulldayCustom").setValue('A');
    }
    else{
      this.weekDays().at(periodIndex).get("fulldayCustom").setValue('');
    }
  }
  
  onDeleteCustomPeriod(periodIndex, customIndex){
     this.customPeriods(periodIndex).removeAt(customIndex);
  }

  addCustomPeriod(periodIndex){
    if(this.customPeriods(periodIndex).length < 4)
      this.customPeriods(periodIndex).push(this.initCustomPeriodItems());
  }

  weekDays(): FormArray {
    return this.periodSelectionForm.get("FormArrayItems") as FormArray;
  }

  customPeriods(periodIndex: number) : FormArray {
    return this.weekDays().at(periodIndex).get("FormArrayCustomItems") as FormArray
  }

  getAlertTimingPayload(){
    let alertTimingRef= [{id : 1}];
    return alertTimingRef;
  }

}

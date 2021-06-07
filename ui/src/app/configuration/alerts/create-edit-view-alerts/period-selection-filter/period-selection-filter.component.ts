import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormGroup,FormBuilder, FormArray } from '@angular/forms';
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

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId= parseInt(localStorage.getItem("accountId"));
    this.periodSelectionForm = this._formBuilder.group({
      // recipientLabel: ['', [ Validators.required ]],
      FormArrayItems : this._formBuilder.array([this.initPeriodItems()]),
    })
  }

  initPeriodItems(){

  }

}

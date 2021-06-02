import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-alert-advanced-filter',
  templateUrl: './alert-advanced-filter.component.html',
  styleUrls: ['./alert-advanced-filter.component.css']
})
export class AlertAdvancedFilterComponent implements OnInit {
  @Input() translationData: any = [];
  alertAdvancedFilterForm: FormGroup;
  localStLanguage: any;
  organizationId: number;
  accountId: number;
  isDistanceSelected: boolean= false;
  isOccurenceSelected: boolean= false;
  isDurationSelected: boolean= false;
  selectedPoiSite: any;
  
  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId= parseInt(localStorage.getItem("accountId"));
    this.alertAdvancedFilterForm = this._formBuilder.group({
      poiSite: [''],
      distance: [''],
      occurences: [''],
      duration: ['']
    })
  }

  onChangeDistance(event: any){
    if(event.checked){
      this.isDistanceSelected= true;
    }
    else{
      this.isDistanceSelected= false;
    }
  }

  onChangeOccurence(event: any)
  {
    if(event.checked){
      this.isOccurenceSelected= true;
    }
    else{
      this.isOccurenceSelected= false;
    }
  }

  onChangeDuration(event: any)
  {
    if(event.checked){
      this.isDurationSelected= true;
    }
    else{
      this.isDurationSelected= false;
    }
  }

  onRadioButtonChange(event: any){
    this.selectedPoiSite = event.value;
  }
}

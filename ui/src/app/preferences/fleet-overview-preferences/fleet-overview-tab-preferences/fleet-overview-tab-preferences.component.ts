import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'src/app/services/message.service';
import { ReportService } from 'src/app/services/report.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-fleet-overview-tab-preferences',
  templateUrl: './fleet-overview-tab-preferences.component.html',
  styleUrls: ['./fleet-overview-tab-preferences.component.less']
})
export class FleetOverviewTabPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any = {};
  @Output() setFleetOverviewFlag = new EventEmitter<any>();
  reportId: any;
  initData: any = [];
  timerPrefData: any = [];
  vehInfoPrefData: any = [];
  selectionForSetTimerColumns = new SelectionModel(true, []);
  selectionForVehInfoColumns = new SelectionModel(true, []);
  fleetOverviewForm: FormGroup;
  requestSent:boolean = false;
  showLoadingIndicator: boolean = false;

  constructor(private messageService: MessageService, private reportService: ReportService, private router: Router, private _formBuilder: FormBuilder) { }

  ngOnInit() { 
    let repoId: any = this.reportListData.filter(i => i.name == 'Fleet Overview');
    this.fleetOverviewForm = this._formBuilder.group({
      refreshTime: ['',[Validators.required]]
    },{
      validator: [
        CustomValidators.numberFieldValidation('refreshTime', 60),
        CustomValidators.numberMinFieldValidation('refreshTime', 1)
      ]
    });
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
      this.loadFleetOverviewPreferences();
    }
   }

   loadFleetOverviewPreferences(){
     this.showLoadingIndicator=true;
      this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.showLoadingIndicator=false;
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.getTranslatedColumnName(this.initData);
    }, (error)=>{
      this.showLoadingIndicator=false;
      this.initData = [];
      this.resetColumnData();
    });
   }

  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(item => {
            let _data: any;
            if(item.key.includes('rp_fo_fleetoverview_generalvehicleinformation_')){
              _data = item;
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 40);   
              }
              this.vehInfoPrefData.push(_data);
            }
          });
      if (this.vehInfoPrefData.length > 0) { 
        this.setColumnCheckbox();
      }
    }
  }

  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForSetTimerColumns.clear();
    this.selectionForVehInfoColumns.clear();
    this.timerPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForSetTimerColumns.select(element);
      }
    });
    this.vehInfoPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForVehInfoColumns.select(element);
      }
    });
  }

   resetColumnData(){
     this.timerPrefData = [];
     this.vehInfoPrefData = [];
   }

   onCancel(){
    this.setFleetOverviewFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm() {
    if (!this.requestSent) {
      this.requestSent = true;
      let _timerArr: any = [];
      let _vehInfoArr: any = [];
      let parentDataAttr: any = [];
      this.vehInfoPrefData.forEach(element => {
        let sSearch = this.selectionForVehInfoColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (sSearch.length > 0) {
          _vehInfoArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _vehInfoArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });
      parentDataAttr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.reportId });
      let objData: any = {
        reportId: this.reportId,
        attributes: [..._timerArr, ..._vehInfoArr, ...parentDataAttr] //-- merge data
      }
      this.showLoadingIndicator=true;
      this.reportService.updateReportUserPreference(objData).subscribe((prefData: any) => {
        this.showLoadingIndicator=false;
        this.loadFleetOverviewPreferences();
        this.setFleetOverviewFlag.emit({ flag: false, msg: this.getSuccessMsg() });
        if ((this.router.url).includes("fleetoverview/fleetoverview")) {
          this.reloadCurrentComponent();
        }
        this.requestSent = false;
      }, (error) => {
        this.showLoadingIndicator=false;
      });
    }
  }

  setTimerValueInLocalStorage(timerVal: any){
    let num = (timerVal*60);
    localStorage.setItem("liveFleetTimer", num.toString());  // default set
    this.messageService.sendTimerValue(num);
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  keyPressNumbers(event: any){
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }

  isAllSelectedForTimerColumns(){
    const numSelected = this.selectionForSetTimerColumns.selected.length;
    const numRows = this.timerPrefData.length;
    return numSelected === numRows;
  }

  masterToggleForTimerColumns(){
    if(this.isAllSelectedForTimerColumns()){
      this.selectionForSetTimerColumns.clear();
    }else{
      this.timerPrefData.forEach(row => { this.selectionForSetTimerColumns.select(row) });
    }
  }

  isAllSelectedForVehInfoColumns(){
    const numSelected = this.selectionForVehInfoColumns.selected.length;
    const numRows = this.vehInfoPrefData.length;
    return numSelected === numRows;
  }

  masterToggleForVehInfoColumns(){
    if(this.isAllSelectedForVehInfoColumns()){
      this.selectionForVehInfoColumns.clear();
    }else{
      this.vehInfoPrefData.forEach(row => { this.selectionForVehInfoColumns.select(row) });
    }
  }

  checkboxLabelForColumns(rowData?: any){ }

  checkboxClicked(event: any, rowData: any){ }

}
import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ReportService } from 'src/app/services/report.service';
import { DataInterchangeService } from '../../../services/data-interchange.service';

@Component({
  selector: 'app-logbook-tab-preferences',
  templateUrl: './logbook-tab-preferences.component.html',
  styleUrls: ['./logbook-tab-preferences.component.less']
})

export class LogbookTabPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any = {};
  @Output() setLogbookFlag = new EventEmitter<any>();
  reportId: any;
  initData: any = [];
  logbookPrefData: any = [];
  selectionForLoogbookColumns = new SelectionModel(true, []);
  reqField: boolean = false;
  requestSent:boolean = false;
  showLoadingIndicator: boolean = false;

  constructor(private reportService: ReportService, private router: Router, private dataInterchangeService: DataInterchangeService) { }

  ngOnInit() {
    let repoId: any = this.reportListData.filter(i => i.name == 'Logbook');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
      this.loadLogbookPreferences();
    }
  }

  loadLogbookPreferences(reloadFlag?: any){
    this.showLoadingIndicator = true;
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.showLoadingIndicator = false;
      this.initData = prefData['userPreferences'];
      if(reloadFlag){ // refresh pref setting & goto logbook 
        let _dataObj: any = {
          prefdata: this.initData,
          type: 'logbook' 
        }
        this.dataInterchangeService.getPrefData(_dataObj);
        this.dataInterchangeService.closedPrefTab(false); // closed pref tab
      }
      this.resetColumnData();
      this.getTranslatedColumnName(this.initData);
      this.validateRequiredField();
    }, (error)=>{
      this.showLoadingIndicator = false;
      this.initData = [];
      this.resetColumnData();
    });
  }

  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any;
            if(item.key.includes('rp_lb_logbook_details_')){
              _data = item;
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 25);   
              }
              this.logbookPrefData.push(_data);
            }
          });
        }
      });
      this.setColumnCheckbox();
    }
  }

  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForLoogbookColumns.clear();
    this.logbookPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForLoogbookColumns.select(element);
      }
    });
  }

  resetColumnData(){
    this.logbookPrefData = [];
  }

  validateRequiredField(){
    let _flag = true;
    if(this.selectionForLoogbookColumns.selected.length > 0){
      let _search = this.selectionForLoogbookColumns.selected.filter(i => (i.key == 'rp_lb_logbook_details_vehiclename' || i.key == 'rp_lb_logbook_details_vin' || i.key == 'rp_lb_logbook_details_registrationplatenumber'));
      if(_search.length > 0){
        _flag = false;
      }
    }
    this.reqField = _flag;
  }

  isAllSelectedForLogbookColumns(){
    const numSelected = this.selectionForLoogbookColumns.selected.length;
    const numRows = this.logbookPrefData.length;
    return numSelected === numRows;
  }

  masterToggleForLogbookColumns(){
    if(this.isAllSelectedForLogbookColumns()){
      this.selectionForLoogbookColumns.clear();
      this.validateRequiredField();
    }else{
      this.logbookPrefData.forEach(row => { this.selectionForLoogbookColumns.select(row) });
      this.validateRequiredField();
    }
  }

  checkboxClicked(event: any, rowData: any){
    this.validateRequiredField();
  }

  checkboxLabelForColumns(row?: any): string{
    if(row)
      return `${this.isAllSelectedForLogbookColumns() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForLoogbookColumns.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  onCancel(){
    this.setLogbookFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onReset(){
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onConfirm() {
    if (!this.requestSent) {
      this.requestSent = true;
      let _dataArr: any = [];
      this.logbookPrefData.forEach(element => {
        let search = this.selectionForLoogbookColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (search.length > 0) {
          _dataArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _dataArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      _dataArr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.reportId }); // main parent
      if (this.selectionForLoogbookColumns.selected.length == this.logbookPrefData.length) { // parent selected
        _dataArr.push({ dataAttributeId: this.initData.subReportUserPreferences[0].dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.reportId });
      } else { // parent un-selected
        _dataArr.push({ dataAttributeId: this.initData.subReportUserPreferences[0].dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.reportId });
      }

      let objData: any = {
        reportId: this.reportId,
        attributes: _dataArr
      }
      this.showLoadingIndicator=true;
      this.reportService.updateReportUserPreference(objData).subscribe((_tripPrefData: any) => {
        this.showLoadingIndicator = false;
        let _reloadFlag = false;
        if ((this.router.url).includes("fleetoverview/logbook")) {
          _reloadFlag = true;
        }
        this.loadLogbookPreferences(_reloadFlag);
        this.setLogbookFlag.emit({ flag: false, msg: this.getSuccessMsg() });
        this.requestSent = false;
      }, (error) => {
        this.showLoadingIndicator = false;
      });
    }
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }
}
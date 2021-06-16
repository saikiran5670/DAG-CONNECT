import { SelectionModel } from '@angular/cdk/collections';
import { Component, Input, OnInit } from '@angular/core';
import { ReportService } from '../../../services/report.service';

@Component({
  selector: 'app-fleet-utilisation-preference',
  templateUrl: './fleet-utilisation-preference.component.html',
  styleUrls: ['./fleet-utilisation-preference.component.less']
})
export class FleetUtilisationPreferenceComponent implements OnInit {
  @Input() translationData: any;
  @Input() reportListData: any;
  reportId: any;
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  initData: any = [];
  summaryColumnData: any = [];
  chartsColumnData: any = [];
  calenderColumnData: any = [];
  detailColumnData: any = [];
  selectionForSummaryColumns = new SelectionModel(true, []);

  constructor(private reportService: ReportService) { }

  ngOnInit() { 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Fleet Utilisation Report');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 5; //- hard coded for fleet utilisation report
    }
    this.loadFleetUtilisationPreferences();
  }

  loadFleetUtilisationPreferences(){
    this.reportService.getUserPreferenceReport(this.reportId, this.accountId, this.accountOrganizationId).subscribe((prefData: any) => {
      this.initData = prefData['userPreferences'];
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.initData = [];
    });
  }

  preparePrefData(prefData: any){
    prefData.forEach(element => {
      let _data: any;
      if(element.key.includes('da_report_general')){
         _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 15);   
        }
        this.summaryColumnData.push(_data);
      }else if(element.key.includes('da_report_charts')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 14);   
        }
        this.chartsColumnData.push(_data);
      }else if(element.key.includes('da_report_calendarview')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 20);   
        }
        this.calenderColumnData.push(_data);
      }else if(element.key.includes('da_report_details')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 15);   
        }
        this.detailColumnData.push(_data);
      }
    });
  }

  getName(name: any, index: any) {
    let updatedName = name.slice(index);
    return updatedName;
  }

  masterToggleForSummaryColumns(){
    if(this.isAllSelectedForSummaryColumns()){
      this.selectionForSummaryColumns.clear();
    }else{
      this.summaryColumnData.forEach(row => { this.selectionForSummaryColumns.select(row) });
    }
  }

  isAllSelectedForSummaryColumns(){
    const numSelected = this.selectionForSummaryColumns.selected.length;
    const numRows = this.summaryColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForSummaryColumns(row?: any){

  }

  onCancel(){

  }

  onReset(){

  }

  onConfirm(){

  }

}

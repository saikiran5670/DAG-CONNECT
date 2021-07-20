import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-fuel-deviation-preferences',
  templateUrl: './fuel-deviation-preferences.component.html',
  styleUrls: ['./fuel-deviation-preferences.component.less']
})
export class FuelDeviationPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setFuelDeviationReportFlag = new EventEmitter<any>();
  reportId: any;
  initData: any = [];
  summaryData:any = [];
  chartsData:any = [];
  detailsData:any = [];
  selectionForSummary = new SelectionModel(true, []);
  selectionForCharts = new SelectionModel(true, []);
  selectionForDetails = new SelectionModel(true, []);

  constructor(private reportService: ReportService, private router: Router) { }

  ngOnInit() {
    let repoId: any = this.reportListData.filter(i => i.name == 'Fuel Deviation Report');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 7; //- hard coded for Fuel Deviation Report
    }
    this.translationUpdate();
    this.loadFuelDeviationReportPreferences();
  }

  translationUpdate(){
    this.translationData = {

    }
  }

  loadFuelDeviationReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.initData = [];
    });
  }

  resetColumnData(){
    this.summaryData = [];
    this.chartsData = [];
    this.detailsData = [];
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.key.includes('rp_fd_summary_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 13);   
              }
              this.summaryData.push(_data);
            }else if(item.key.includes('rp_fd_chart_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 18);   
              }
              this.chartsData.push(_data);
            }else if(item.key.includes('rp_fd_details_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 16);   
              }
              this.detailsData.push(_data);
            }
          });
        }
      });
      this.setColumnCheckbox();
    }
  }

  getName(name: any, index: any) {
    let updatedName = name.slice(index);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForSummary.clear();
    this.selectionForCharts.clear();
    this.selectionForDetails.clear();
    
    this.summaryData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForSummary.select(element);
      }
    });
    
    this.chartsData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForCharts.select(element);
      }
    });
    
    this.detailsData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForDetails.select(element);
      }
    });
  }

  onCancel(){
    this.setFuelDeviationReportFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm(){

  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
  }

}

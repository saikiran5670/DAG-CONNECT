import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

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
  fuelDeviationReportForm: FormGroup;
  chartsData:any = [];
  detailsData:any = [];
  selectionForSummary = new SelectionModel(true, []);
  selectionForCharts = new SelectionModel(true, []);
  selectionForDetails = new SelectionModel(true, []);
  chartIndex: any = {};
  lineBarDD: any = [{
    type: 'L',
    name: 'Line Chart'
  },
  {
    type: 'B',
    name: 'Bar Chart'
  }];
  
  donutPieDD: any = [{
    type: 'D',
    name: 'Donut Chart'
  },
  {
    type: 'P',
    name: 'Pie Chart'
  }];

  constructor(private reportService: ReportService, private router: Router, private _formBuilder: FormBuilder) { }

  ngOnInit() {
    let repoId: any = this.reportListData.filter(i => i.name == 'Fuel Deviation Report');
    this.fuelDeviationReportForm = this._formBuilder.group({
      increaseEventChart: [],
      decreaseEventChart: [],
      deviationEventChart: []
    });
    
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
      rp_fd_reportsummary: 'Summary',
      rp_fd_summary_fuelincreaseevents: 'Fuel Increase Events',
      rp_fd_summary_fueldecreaseevents: 'Fuel Decrease Events',
      rp_fd_summary_vehiclewithfuelevents: 'Vehicle With Fuel Events',
      rp_fd_reportchart: 'Charts',
      rp_fd_chart_fuelincreaseevents: 'Fuel Increase Events',
      rp_fd_chart_fueldecreaseevents: 'Fuel Decrease Events',
      rp_fd_chart_fueldeviationevent: 'Fuel Deviation Event',
      rp_fd_report_details: 'Details',
      rp_fd_details_averageweight: 'Average Weight',
      rp_fd_details_enddate: 'End Date',
      rp_fd_details_fuelconsumed: 'Fuel Consumed',
      rp_fd_details_startdate: 'Start Date',
      rp_fd_details_drivingtime: 'Driving Time',
      rp_fd_details_startposition: 'Start Position',
      rp_fd_details_odometer: 'Odometer',
      rp_fd_details_vehiclename: 'Vehicle Name',
      rp_fd_details_vin: 'VIN',
      rp_fd_details_type: 'Type',
      rp_fd_details_date: 'Date',
      rp_fd_details_distance: 'Distance',
      rp_fd_details_averagespeed: 'Average Speed',
      rp_fd_details_regplatenumber: 'Reg. Plate Number',
      rp_fd_details_endposition: 'End Position',
      rp_fd_details_idleduration: 'Idle Duration',
      rp_fd_details_alerts: 'Alerts',
      rp_fd_details_difference: 'Difference'
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
                _data.translatedName = this.getName(item.name, 15);   
              }
              this.summaryData.push(_data);
            }else if(item.key.includes('rp_fd_chart_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 13);   
              }
              let index: any;
              switch(item.key){
                case 'rp_fd_chart_fuelincreaseevents':{
                  index = this.chartIndex.increaseEventIndex = 0;
                  break;
                }
                case 'rp_fd_chart_fueldecreaseevents':{
                  index = this.chartIndex.decreaseEventIndex = 1;
                  break;
                }
                case 'rp_fd_chart_fueldeviationevent':{
                  index = this.chartIndex.deviationEventIndex = 2;
                  break;
                }
              }
              this.chartsData[index] = _data;
            }else if(item.key.includes('rp_fd_details_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 15);   
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

    if(this.chartsData.length > 0){
      this.setDefaultFormValues();
    }
  }

  setDefaultFormValues(){
    this.fuelDeviationReportForm.get('increaseEventChart').setValue(this.chartsData[0].chartType != '' ? this.chartsData[0].chartType : 'L');
    this.fuelDeviationReportForm.get('decreaseEventChart').setValue(this.chartsData[1].chartType != '' ? this.chartsData[1].chartType : 'L');
    this.fuelDeviationReportForm.get('deviationEventChart').setValue(this.chartsData[2].chartType != '' ? this.chartsData[2].chartType : 'D');
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

  masterToggleForSummaryColumns(){
    if(this.isAllSelectedForSummaryColumns()){
      this.selectionForSummary.clear();
    }else{
      this.summaryData.forEach(row => { this.selectionForSummary.select(row) });
    }
  }

  isAllSelectedForSummaryColumns(){
    const numSelected = this.selectionForSummary.selected.length;
    const numRows = this.summaryData.length;
    return numSelected === numRows;
  }

  masterToggleForDetailColumns(){
    if(this.isAllSelectedForSummaryColumns()){
      this.selectionForDetails.clear();
    }else{
      this.detailsData.forEach(row => { this.selectionForDetails.select(row) });
    }
  }

  isAllSelectedForDetailColumns(){
    const numSelected = this.selectionForDetails.selected.length;
    const numRows = this.detailsData.length;
    return numSelected === numRows;
  }

  masterToggleForChartsColumns(){
    if(this.isAllSelectedForChartsColumns()){
      this.selectionForCharts.clear();
    }else{
      this.chartsData.forEach(row => { this.selectionForCharts.select(row) });
    }
  }

  isAllSelectedForChartsColumns(){
    const numSelected = this.selectionForCharts.selected.length;
    const numRows = this.chartsData.length;
    return numSelected === numRows;
  }

  checkboxLabelForColumns(rowData?: any){

  }

  summaryCheckboxClicked(event: any, rowData: any){

  }

  detailsCheckboxClicked(event: any, rowData: any){

  }

  onlineBarDDChange(event: any){

  }

  onDonutPieDDChange(event: any){

  }

}

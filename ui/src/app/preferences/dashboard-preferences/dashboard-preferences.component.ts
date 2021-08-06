import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { SelectionModel } from '@angular/cdk/collections';


@Component({
  selector: 'app-dashboard-preferences',
  templateUrl: './dashboard-preferences.component.html',
  styleUrls: ['./dashboard-preferences.component.less']
})

export class DashboardPreferencesComponent implements OnInit {

  @Input() translationData: any;
  @Input() reportListData: any;

  //dashboard preferences
  DashboardPreferenceForm = new FormGroup({});
  editDashboardFlag: boolean = false;
  displayMessage: any = '';
  updateMsgVisible: boolean = false;
  showDashboardReport: boolean = false;
  reportId: any;
  initData: any = [];
  getDashboardPreferenceResponse: any = [];
  selectionForFleetKPIColumns = new SelectionModel(true, []);
  selectionForTodayLiveVehicleColumns = new SelectionModel(true, []);
  selectionForVehicleUtilizationColumns = new SelectionModel(true, []);
  selectionForAlertLast24HoursColumns = new SelectionModel(true, []);
  fleetKPIColumnData = [];
  vehicleUtilizationColumnData = [];
  alertLast24HrsColumnData = [];
  todayLiveVehicleColumnData = [];

  

  constructor(private reportService: ReportService) {
    this.loadReportData();
   }

  ngOnInit() {
  
    
  }

  upperLowerDD: any = [
    {
      type: 'U',
      name: 'Upper'
    },
    {
      type: 'L',
      name: 'Lower'
    }  
  ];

  donutPieDD: any = [
    {
      type: 'D',
      name: 'Donut Chart'
    },
    {
      type: 'P',
      name: 'Pie Chart'
    }
  ];

  chartSelectionType: any = [
    {
      type: 'B',
      name: 'Bar Chart'
    },
    {
      type: 'L',
      name: 'Line Chart'
    },
    {
      type: 'P',
      name: 'Pie Chart'
    },
    {
      type: 'D',
      name: 'Donut Chart'
    }
  ] 

  onClose(){
    this.updateMsgVisible = false;
  }

  editDashboardPreferences(){
    this.editDashboardFlag = true;
    this.showDashboardReport = false;
  }

  updateEditFleetUtilFlag(retObj: any){
    if(retObj){
      this.editDashboardFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editDashboardFlag = false; // hard coded
    }
  }

  successMsgBlink(msg: any){
    this.updateMsgVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.updateMsgVisible = false;
    }, 5000);
  }

  getSuccessMsg() {
    if (this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details saved successfully");
  }

  loadReportData(){
   
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
     
      this.reportListData = reportList.reportDetails;
      let repoId: any = this.reportListData.filter(i => i.name == 'Dashboard');
    
      if (repoId.length > 0) {
        this.reportId = repoId[0].id;
      } else {
        this.reportId = 18; 
      }//- hard coded for Dashboard
  
      this.loadDashboardPreferences();
    }, (error)=>{
      console.log('Report not found...', error);
      
      this.reportListData = [];
    });
  }


  loadDashboardPreferences() {

    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData: any) => {
      this.initData = prefData['userPreferences'];
      this.getDashboardPreferenceResponse = this.initData;  
      console.log("dataaaaaaa--->",this.getDashboardPreferenceResponse);  
      this.resetColumnData();
      this.prepareDataDashboardPref();
        
    }, (error) => {
      this.initData = [];
      
    });
  }

  resetColumnData() {
    this.fleetKPIColumnData = [];
    this.vehicleUtilizationColumnData = [];
    this.alertLast24HrsColumnData = [];
    this.todayLiveVehicleColumnData = [];    
  }

  setColumnCheckbox() {
  this.selectionForFleetKPIColumns.clear();
  this.selectionForTodayLiveVehicleColumns.clear(); 
  this.selectionForVehicleUtilizationColumns.clear(); 
  this.selectionForAlertLast24HoursColumns.clear();

  this.fleetKPIColumnData.forEach(element => {
    if (element.state == 'A') {
      this.selectionForFleetKPIColumns.select(element);
    }
  });

  this.todayLiveVehicleColumnData.forEach(element => {
    if (element.state == 'A') {
      this.selectionForTodayLiveVehicleColumns.select(element);
    }
  });

  this.alertLast24HrsColumnData.forEach(element => {
    if (element.state == 'A') {
      this.selectionForAlertLast24HoursColumns.select(element);
    }
  });

  this.vehicleUtilizationColumnData.forEach(element => {
    if (element.state == 'A') {
      this.selectionForVehicleUtilizationColumns.select(element);
    }
  });

  }

  prepareDataDashboardPref()
  {
    this.getDashboardPreferenceResponse.subReportUserPreferences.forEach(section => {
     
      section.subReportUserPreferences.forEach(element => {   
        let _data: any;
        if (section.name.includes('Dashboard.FleetKPI')) 
        {
          _data = element;
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          //   console.log("translated name....",_data.translatedName);
          // } else {
          //   _data.translatedName = this.getName(element.name);
          //   console.log("translated name1....",_data.translatedName);
          // }
          // this.fleetKPIColumnData.push(_data);
          console.log("translated name2....",_data.translatedName);
                 
        }
        else if(section.name.includes('Dashboard.TodayLiveVehicle'))
        {
          _data = element;
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          // } else {
          //   _data.translatedName = this.getName(element.name);
          // }
          
          this.todayLiveVehicleColumnData.push(_data);
         
        }
        else if(section.name.includes('Dashboard.VehicleUtilization'))
        {
          _data = element;
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          // } else {
          //   _data.translatedName = this.getName(element.name);
          // }
          _data.translatedName = this.getName(element.name);
          this.vehicleUtilizationColumnData.push(_data);
          
        }
        else if(section.name.includes('Dashboard.AlertLast24Hours'))
        {
          _data = element;
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          // } else {
          //   _data.translatedName = this.getName(element.name);
          // }
          _data.translatedName = this.getName(element.name);
          this.alertLast24HrsColumnData.push(_data);         
        }
      });
      });

      console.log("data...", this.fleetKPIColumnData);
      console.log("data......",this.todayLiveVehicleColumnData);
      console.log("data....",this.vehicleUtilizationColumnData);
      console.log("data....",this.alertLast24HrsColumnData);
        
      this.setColumnCheckbox();
  }

  getName(name: any) {
    let updatedName = name.split('.').splice(-1).join('');
    return updatedName;
  }

  masterToggle(section){
    if(this.isAllSelected(section)){
      this["selectionFor"+section+"Columns"].clear();
    }else{
      let lowerCaseSection = section.charAt(0).toLowerCase() + section.substring(1);
      this[lowerCaseSection+"ColumnData"].forEach(row => { this["selectionFor"+section+"Columns"].select(row) });
    }
  }

  isAllSelected(section){
    const numSelected = this["selectionFor"+section+"Columns"].selected.length;
    let lowerCaseSection = section.charAt(0).toLowerCase() + section.substring(1);
    const numRows = this[lowerCaseSection+"ColumnData"].length;
    return numSelected === numRows;
  }


}
    


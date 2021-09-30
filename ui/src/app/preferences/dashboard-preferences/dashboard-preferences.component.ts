import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { SelectionModel } from '@angular/cdk/collections';
import { TranslationService } from 'src/app/services/translation.service';
import { ReportMapService } from '../../report/report-map.service';
import { DashboardService } from '../../services/dashboard.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard-preferences',
  templateUrl: './dashboard-preferences.component.html',
  styleUrls: ['./dashboard-preferences.component.less']
})

export class DashboardPreferencesComponent implements OnInit {

  @Input() translationData: any;
  @Input() reportListData: any;

  //dashboard preferences
  responseFlag:boolean;
  showLoadingIndicator: any = false;
  dashboardPreferenceForm = new FormGroup({});
  editDashboardFlag: boolean = false;
  displayMessage: any = '';
  updateMsgVisible: boolean = false;
  showDashboardReport: boolean = false;
  reportId: any;
  unitId: any;
  prefUnit: any;
  prefUnitFormat: any;
  generalPreferences: any;
  initData: any = [];
  getDashboardPreferenceResponse: any = [];
  selectionForFleetKPIColumns = new SelectionModel(true, []);
  selectionForTodayLiveVehicleColumns = new SelectionModel(true, []);
  selectionForVehicleUtilizationColumns = new SelectionModel(true, []);
  selectionForAlertLast24HoursColumns = new SelectionModel(true, []);
  fleetKPIColumnData = [];
  vehicleUtilizationColumnData = [];
  alertLast24HoursColumnData = [];
  todayLiveVehicleColumnData = [];
 
  constructor(private dashboardService: DashboardService, private reportService: ReportService, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportMapService: ReportMapService, private router: Router) {
    this.loadReportData();
  }

  ngOnInit() {

    let accountPreference = JSON.parse(localStorage.getItem('accountInfo')).accountPreference;
    this.unitId = accountPreference.unitId
    let languageCode = JSON.parse(localStorage.getItem('language')).code;
    this.translationService.getPreferences(languageCode).subscribe((res) => { this.generalPreferences = res; this.getUnits() }
    )
    this.translationUpdate();
  }

  translationUpdate(){
    this.translationData = {
      rp_db_dashboard_fleetkpi_co2emission: 'CO2 Emission',
      rp_db_dashboard_fleetkpi_totaldistance: 'Total Distance',
      rp_db_dashboard_fleetkpi_drivingtime: 'Driving Time',
      rp_db_dashboard_fleetkpi_fuelconsumption: 'Fuel Consumption',
      rp_db_dashboard_fleetkpi_fuelusedidling: 'Fuel Used Idling',
      rp_db_dashboard_fleetkpi_idlingtime: 'Idling Time',
      rp_db_dashboard_fleetkpi_fuelconsumed: 'Fuel Consumed',
      rp_db_dashboard_todaylivevehicle_distance: 'Distance',
      rp_db_dashboard_todaylivevehicle_drivingtime: 'Driving Time',
      rp_db_dashboard_todaylivevehicle_drivers: 'Drivers',
      rp_db_dashboard_todaylivevehicle_criticalalerts: 'Critical Alerts',
      rp_db_dashboard_todaylivevehicle_timebasedutilizationrate: 'Time Based Utilization Rate',
      rp_db_dashboard_todaylivevehicle_distancebasedutilizationrate: 'Distance Based Utilization Rate',
      rp_db_dashboard_todaylivevehicle_activevehicles: 'Active Vehicles',
      rp_db_dashboard_vehicleutilization_distancebasedutilizationrate: 'Distance Based Utilization Rate',
      rp_db_dashboard_vehicleutilization_timebasedutilizationrate: 'Time Based Utilization Rate',
      rp_db_dashboard_vehicleutilization_distanceperday: 'Distance Per Day',
      rp_db_dashboard_vehicleutilization_activevehiclesperday: 'Active Vehicles Per Day',
      rp_db_dashboard_alertlast24hours_levelalerts: 'Level Alerts',
      rp_db_dashboard_alertlast24hours_totalalerts: 'Total Alerts',
      rp_db_dashboard_alertlast24hours_logisticalert: 'Logistic Alert',
      rp_db_dashboard_alertlast24hours_fueldriveralerts: 'Fuel & Driver Alerts',
      rp_db_dashboard_alertlast24hours_repairmaintenancealerts: 'Repair & Maintenance Alerts',
    }
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

  lineBarDD: any = [
    {
      type: 'B',
      name: 'Bar Chart'
    },
    {
      type: 'L',
      name: 'Line Chart'
    }

  ];

  onClose() {
    this.updateMsgVisible = false;
  }

  editDashboardPreferences() {
    this.editDashboardFlag = true;
    this.showDashboardReport = false;
  }



  successMsgBlink(msg: any) {
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

  loadReportData() {
    this.showLoadingIndicator = true;
    this.reportService.getReportDetails().subscribe((reportList: any) => {

      this.reportListData = reportList.reportDetails;
      let repoId: any = this.reportListData.filter(i => i.name == 'Dashboard');

      if (repoId.length > 0) {
        this.reportId = repoId[0].id;
        this.loadDashboardPreferences();
      } else {
        console.error("No report id found!")
      }//- hard coded for Dashboard

      
    }, (error) => {
      console.log('Report not found...', error);
      this.hideloader();
      this.reportListData = [];
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }
  loadDashboardPreferences() {

    this.dashboardService.getDashboardPreferences(this.reportId).subscribe((prefData: any) => {
      this.hideloader();
      this.initData = prefData['userPreferences'];
      this.getDashboardPreferenceResponse = this.initData;
      //console.log("dataaaaaaa--->", this.getDashboardPreferenceResponse);
      this.getUnits();
      this.resetColumnData();
      this.prepareDataDashboardPref();

    }, (error) => {
      this.resetColumnData();
      this.initData = [];
      this.hideloader();
    });
  }

  resetColumnData() {
    this.fleetKPIColumnData = [];
    this.vehicleUtilizationColumnData = [];
    this.alertLast24HoursColumnData = [];
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

    this.alertLast24HoursColumnData.forEach(element => {
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

  prepareDataDashboardPref() {
    this.getDashboardPreferenceResponse.subReportUserPreferences.forEach(section => {

      section.subReportUserPreferences.forEach(element => {
        let _data: any;
        if (section.name.includes('Dashboard.FleetKPI')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
            //console.log("translated name....", _data.translatedName);
          } else {
            _data.translatedName = this.getName(element.name);
            //console.log("translated name1....", _data.translatedName);
          }
          // _data.translatedName = this.getName(element.name);
          this.fleetKPIColumnData.push(_data);
          this.dashboardPreferenceForm.addControl(element.key + 'thresholdType', new FormControl(element.thresholdType != '' ? element.thresholdType : 'L'));
          if (element.key.includes('fleetkpi_drivingtime') || element.key.includes('fleetkpi_idlingtime')) {
            let secondscalc = (element.thresholdValue); // removed as time is in seconds
            let hms = this.secondsToHms(secondscalc);
            let hrs = '';
            let mins = '';
            if (hms) {
              hrs = hms.split(',')[0];
              mins = hms.split(',')[1];
            }
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(hrs,[Validators.min(0),Validators.max(23)]));
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValuemin', new FormControl(mins,[Validators.min(0),Validators.max(60)]));
          } else if (element.key.includes('fleetkpi_totaldistance')) {
            let thresholdValueKm = this.reportMapService.getDistance(element.thresholdValue, this.prefUnitFormat);
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(thresholdValueKm,[Validators.min(0),Validators.max(10000),Validators.pattern('^\-?[0-9]+(?:\.[0-9]{1,2})?$')]));
          }
          else if (element.key.includes('fleetkpi_fuelconsumption'))
          {
            let thresholdValue_L_100Km = this.reportMapService.getFuelConsumedUnits(element.thresholdValue, this.prefUnitFormat, true);
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(thresholdValue_L_100Km,[Validators.min(0),Validators.max(10000),Validators.pattern('^\-?[0-9]+(?:\.[0-9]{1,2})?$')]));
          }
          else if (element.key.includes('fleetkpi_fuelusedidling') || element.key.includes('fleetkpi_fuelconsumed')) {
            let thresholdValueL = this.reportMapService.getFuelConsumedUnits(element.thresholdValue, this.prefUnitFormat, false);
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(thresholdValueL,[Validators.min(0),element.key.includes('fleetkpi_fuelusedidling') ? Validators.max(10000) :  Validators.max(1000), Validators.pattern('^\-?[0-9]+(?:\.[0-9]{1,2})?$')]));
          }
          else {
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(element.thresholdValue, [Validators.min(0),Validators.max(1000),Validators.pattern('^\-?[0-9]+(?:\.[0-9]{1,2})?$')]));
          }
        } else if (section.name.includes('Dashboard.TodayLiveVehicle')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name);
          }
          // _data.translatedName = this.getName(element.name);
          this.todayLiveVehicleColumnData.push(_data);
          this.dashboardPreferenceForm.addControl(element.key + 'thresholdType', new FormControl(element.thresholdType != '' ? element.thresholdType : 'L'));
          if (element.key.includes('todaylivevehicle_timebasedutilizationrate')) {
            let secondscalc = (element.thresholdValue)/1000;          
            let hms = this.secondsToHms(secondscalc);
            let hrs = '';
            let mins = '';
            if (hms) {
              hrs = hms.split(',')[0];
              mins = hms.split(',')[1];
            }
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(hrs,[Validators.min(0),Validators.max(23)]));
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValuemin', new FormControl(mins,[Validators.min(0),Validators.max(60)]));
          }
          else if (element.key.includes('todaylivevehicle_distancebasedutilizationrate')) {
            let thresholdValueKm = this.reportMapService.getDistance(element.thresholdValue, this.prefUnitFormat);
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(thresholdValueKm, [Validators.min(0),Validators.max(10000),Validators.pattern('^\-?[0-9]+(?:\.[0-9]{1,2})?$')]));
          }        
          else {
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(element.thresholdValue,[Validators.min(0),Validators.max(10000)]));
          }

        } else if (section.name.includes('Dashboard.VehicleUtilization')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name);
          }
          // _data.translatedName = this.getName(element.name);
          this.vehicleUtilizationColumnData.push(_data);
          
          if (element.key.includes('vehicleutilization_timebasedutilizationrate')) {          
            let secondscalc = (element.thresholdValue)/1000;          
            let hms = this.secondsToHms(secondscalc);
            let hrs = '';
            let mins = '';
            if (hms) {
              hrs = hms.split(',')[0];
              mins = hms.split(',')[1];
            }
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(hrs,[Validators.min(0),Validators.max(23)]));
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValuemin', new FormControl(mins,[Validators.min(0),Validators.max(60)]));
            this.dashboardPreferenceForm.addControl(element.key + 'chartType', new FormControl(element.chartType != '' ? element.chartType : 'D'));
          }
          else if (element.key.includes('vehicleutilization_distancebasedutilizationrate')) {
            let thresholdValueKm = this.reportMapService.getDistance(element.thresholdValue, this.prefUnitFormat);
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(thresholdValueKm, [Validators.min(0),Validators.max(10000),
              Validators.pattern('^\-?[0-9]+(?:\.[0-9]{1,2})?$')]));
            this.dashboardPreferenceForm.addControl(element.key + 'chartType', new FormControl(element.chartType != '' ? element.chartType : 'D'));
          }
          else {
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(element.thresholdValue));
            this.dashboardPreferenceForm.addControl(element.key + 'chartType', new FormControl(element.chartType != '' ? element.chartType : 'L'));
          }

        } else if (section.name.includes('Dashboard.AlertLast24Hours')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name);
          }
          // _data.translatedName = this.getName(element.name);
          this.alertLast24HoursColumnData.push(_data);
        }
      });
    });

    this.setColumnCheckbox();
  }

  secondsToHms(d) {
    d = Number(d);
    if(d== 0)
    {
      return "0,0";
    }
    var h = Math.floor(d / 3600);
    var m = Math.floor(d % 3600 / 60);
    var s = Math.floor(d % 3600 % 60);
    var hDisplay = h > 0 ? h + "," : "";
    var mDisplay = m > 0 ? m + "," : "";
    var sDisplay = s > 0 ? s + "," : "";
    return hDisplay + mDisplay + sDisplay;
  }

  getName(name: any) {
    let updatedName = name.split('.').splice(-1).join('');
    return updatedName;
  }

  masterToggle(section) {
    console.log(!this.dashboardPreferenceForm.valid ||(this.selectionForFleetKPIColumns.selected.length == 0 && this.fleetKPIColumnData.length > 0) || (this.selectionForVehicleUtilizationColumns.selected.length == 0 && this.vehicleUtilizationColumnData.length > 0) || (this.selectionForTodayLiveVehicleColumns.selected.length == 0 && this.todayLiveVehicleColumnData.length > 0) || (this.selectionForAlertLast24HoursColumns.selected.length == 0 && this.alertLast24HoursColumnData.length > 0))
    if (this.isAllSelected(section)) {
      this["selectionFor" + section + "Columns"].clear();
    } else {
      let lowerCaseSection = section.charAt(0).toLowerCase() + section.substring(1);
      this[lowerCaseSection + "ColumnData"].forEach(row => { this["selectionFor" + section + "Columns"].select(row) });
    }
  }

  isAllSelected(section) {
    const numSelected = this["selectionFor" + section + "Columns"].selected.length;
    let lowerCaseSection = section.charAt(0).toLowerCase() + section.substring(1);
    const numRows = this[lowerCaseSection + "ColumnData"].length;
    return numSelected === numRows;
  }

  getSaveObject(columnData, selectionData) {
    // console.log("selcted data", selectionData);
    // console.log("coloumn dataaaa", columnData);
    let saveArr = [];
    this[columnData].forEach(element => {
      let sSearch = this[selectionData].selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      let chartType = this.dashboardPreferenceForm.get([element.key + 'chartType'])?.value || '';
      let thresholdType = this.dashboardPreferenceForm.get([element.key + 'thresholdType'])?.value || '';
      let thresholdValue = this.dashboardPreferenceForm.get([element.key + 'thresholdValue'])?.value || 0;
      if (element.key.includes('_fleetkpi_drivingtime') || element.key.includes('_fleetkpi_idlingtime') ) { // separated as time is in seconds
        let thresholdValuehrs = thresholdValue * 3600;
        let thresholdValuemin = this.dashboardPreferenceForm.get([element.key + 'thresholdValuemin'])?.value * 60;
        let totalsecs: number = thresholdValuehrs + thresholdValuemin;
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType ? chartType : '', thresholdType: thresholdType, thresholdValue: totalsecs, reportId: element.reportId  });
      }
      else if(element.key.includes('_todaylivevehicle_timebasedutilizationrate') || element.key.includes('_vehicleutilization_timebasedutilizationrate')){
        let thresholdValuehrs = thresholdValue * 3600;
        let thresholdValuemin = this.dashboardPreferenceForm.get([element.key + 'thresholdValuemin'])?.value * 60;
        let totalsecs: number = thresholdValuehrs + thresholdValuemin;
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType ? chartType : '', thresholdType: thresholdType, thresholdValue: totalsecs * 1000, reportId: element.reportId });
      }
      else if (element.key.includes('fleetkpi_totaldistance') || element.key.includes('todaylivevehicle_distancebasedutilizationrate') || element.key.includes('vehicleutilization_distancebasedutilizationrate')) {
        
        let totalmilimeters = this.prefUnitFormat == 'dunit_Imperial' ? thresholdValue * 1609.344 : thresholdValue * 1000;
       
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType ? chartType : '', thresholdType: thresholdType, thresholdValue: totalmilimeters, reportId: element.reportId  });

      }
      else if(element.key.includes('fleetkpi_fuelconsumption'))
      {
        let totalmililitres = this.prefUnitFormat == 'dunit_Imperial' ? this.reportMapService.convertFuelConsumptionMpgToMlm(thresholdValue): this.reportMapService.convertFuelConsumptionLtr100kmToMlm(thresholdValue);
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType ? chartType : '', thresholdType: thresholdType, thresholdValue: parseFloat(totalmililitres), reportId: element.reportId  });
      }
      else if (element.key.includes('fleetkpi_fuelusedidling') || element.key.includes('fleetkpi_fuelconsumed')) {
        let totalmililitres = this.prefUnitFormat == 'dunit_Imperial' ? thresholdValue * 3785.41 : thresholdValue * 1000;

        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType ? chartType : '', thresholdType: thresholdType, thresholdValue: totalmililitres, reportId: element.reportId  });
      }    
      else {
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType ? chartType : '', thresholdType: thresholdType, thresholdValue: parseInt(thresholdValue), reportId: element.reportId  });
      }

    });
    return saveArr;
  }

  onCancel() {
    this.editDashboardFlag = false;
    this.showDashboardReport = true;
    this.setColumnCheckbox();
  }


  onReset() {
    this.dashboardPreferenceForm = new FormGroup({});
    this.setColumnCheckbox();
    this.resetColumnData();
    this.prepareDataDashboardPref();
  }

  onConfirm() {

    let _fleetKPIArr: any = [];
    let _vehicleUtilizationArr: any = [];
    let _todayLiveVehicleArr: any = [];
    let _alertLast24HoursArr: any = [];
    let _parentArr: any = [];

    _fleetKPIArr = this.getSaveObject('fleetKPIColumnData', 'selectionForFleetKPIColumns');
    _todayLiveVehicleArr = this.getSaveObject('todayLiveVehicleColumnData', 'selectionForTodayLiveVehicleColumns');
    _vehicleUtilizationArr = this.getSaveObject('vehicleUtilizationColumnData', 'selectionForVehicleUtilizationColumns');

    _alertLast24HoursArr = this.getSaveObject('alertLast24HoursColumnData', 'selectionForAlertLast24HoursColumns');
    
    _parentArr.push({ dataAttributeId:  this.getDashboardPreferenceResponse.dataAttributeId, state: "A", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: this.getDashboardPreferenceResponse.reportId  })
 
     this.getDashboardPreferenceResponse.subReportUserPreferences.forEach(element => {     
      if(element.name.includes("Dashboard.FleetKPI")){
        if(this.selectionForFleetKPIColumns.selected.length == this.fleetKPIColumnData.length)
        {
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "A", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }
        else{
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "I", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }       
      }else if(element.name.includes("Dashboard.TodayLiveVehicle")){
        if(this.selectionForTodayLiveVehicleColumns.selected.length == this.todayLiveVehicleColumnData.length)
        {
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "A", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }
        else{
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "I", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }  
       }else if(element.name.includes("Dashboard.VehicleUtilization")){
        if(this.selectionForVehicleUtilizationColumns.selected.length == this.vehicleUtilizationColumnData.length)
        {
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "A", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }
        else{
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "I", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }  
      
      }else if(element.name.includes("Dashboard.AlertLast24Hours")){
        if(this.selectionForAlertLast24HoursColumns.selected.length == this.alertLast24HoursColumnData.length)
        {
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "A", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }
        else{
          _parentArr.push({ dataAttributeId:  element.dataAttributeId, state: "I", preferenceType: "D",  chartType:'', thresholdType:'', thresholdValue:0,  reportId: element.reportId  })
        }  
      }

    });

    //console.log("save Object", [..._fleetKPIArr, ..._vehicleUtilizationArr, ..._todayLiveVehicleArr, ..._alertLast24HoursArr])
    // return [..._fleetKPIArr, ..._vehicleUtilizationArr, ..._todayLiveVehicleArr, ..._alertLast24HoursArr];


    let objData: any = {
      reportId: this.reportId,
      attributes: [..._fleetKPIArr, ..._vehicleUtilizationArr, ..._todayLiveVehicleArr, ..._alertLast24HoursArr, ..._parentArr] //-- merge data
    }
    if(!this.responseFlag)
    {
    this.responseFlag=true;
    this.dashboardService.createDashboardPreferences(objData).subscribe((prefData: any) => {
      this.loadDashboardPreferences();
      this.successMsgBlink('Dashboard Preferences Updated Successfully');
      //this.setDashboardFlag.emit({ flag: false, msg: this.getSuccessMsg() });
      //this.reloadCurrentComponent();
      this.responseFlag=false;
      if((this.router.url).includes("dashboard")){
       this.reloadCurrentComponent();
      }
    });
    }
  }

  reloadCurrentComponent() {
    window.location.reload(); //-- reload screen
  }

  getUnits() {
    let unitObj = this.generalPreferences?.unit.filter(item => item.id == this.unitId);
    if (unitObj && unitObj.length != 0) {
      this.prefUnit = unitObj[0].value;
      if (this.prefUnit == 'Imperial') {
        this.prefUnitFormat = 'dunit_Imperial';
      } else {
        this.prefUnitFormat = 'dunit_Metric';
      }
    }
  }

}



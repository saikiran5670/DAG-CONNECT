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
  dashboardPreferenceForm = new FormGroup({});
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
  alertLast24HoursColumnData = [];
  todayLiveVehicleColumnData = [];




  constructor(private reportService: ReportService, private _formBuilder: FormBuilder) {
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

  updateEditFleetUtilFlag(retObj: any) {
    if (retObj) {
      this.editDashboardFlag = retObj.flag;
      if (retObj.msg && retObj.msg != '') {
        this.successMsgBlink(retObj.msg);
      }
    } else {
      this.editDashboardFlag = false; // hard coded
    }
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

    this.reportService.getReportDetails().subscribe((reportList: any) => {

      this.reportListData = reportList.reportDetails;
      let repoId: any = this.reportListData.filter(i => i.name == 'Dashboard');

      if (repoId.length > 0) {
        this.reportId = repoId[0].id;
      } else {
        this.reportId = 18;
      }//- hard coded for Dashboard

      this.loadDashboardPreferences();
    }, (error) => {
      console.log('Report not found...', error);

      this.reportListData = [];
    });
  }


  loadDashboardPreferences() {

    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData: any) => {
      this.initData = prefData['userPreferences'];
      this.getDashboardPreferenceResponse = this.initData;
      console.log("dataaaaaaa--->", this.getDashboardPreferenceResponse);
      this.resetColumnData();
      this.prepareDataDashboardPref();

    }, (error) => {
      this.resetColumnData();
      this.initData = [];

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
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          //   console.log("translated name....",_data.translatedName);
          // } else {
          //   _data.translatedName = this.getName(element.name);
          //   console.log("translated name1....",_data.translatedName);
          // }
          _data.translatedName = this.getName(element.name);
          this.fleetKPIColumnData.push(_data);
          this.dashboardPreferenceForm.addControl(element.key + 'thresholdType', new FormControl(element.thresholdType));
          if (element.key.includes('fleetkpi_drivingtime') || element.key.includes('fleetkpi_idlingtime')) {
            let hms = this.secondsToHms(element.thresholdValue);
            let hrs = '';
            let mins = '';
            if (hms) {
              hrs = hms.split(',')[0];
              mins = hms.split(',')[1];
            }
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(hrs));
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValuemin', new FormControl(mins));
          } else {
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(element.thresholdValue));
          }
        } else if (section.name.includes('Dashboard.TodayLiveVehicle')) {
          _data = element;
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          // } else {
          //   _data.translatedName = this.getName(element.name);
          // }
          _data.translatedName = this.getName(element.name);
          this.todayLiveVehicleColumnData.push(_data);
          this.dashboardPreferenceForm.addControl(element.key + 'thresholdType', new FormControl(element.thresholdType));
          if (element.key.includes('todaylivevehicle_timebasedutilizationrate')) {
            let hms = this.secondsToHms(element.thresholdValue);
            let hrs = '';
            let mins = '';
            if (hms) {
              hrs = hms.split(',')[0];
              mins = hms.split(',')[1];
            }
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(hrs));
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValuemin', new FormControl(mins));
          } else {
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(element.thresholdValue));
          }

        } else if (section.name.includes('Dashboard.VehicleUtilization')) {
          _data = element;
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          // } else {
          //   _data.translatedName = this.getName(element.name);
          // }
          _data.translatedName = this.getName(element.name);
          this.vehicleUtilizationColumnData.push(_data);
          this.dashboardPreferenceForm.addControl(element.key + 'chartType', new FormControl(element.chartType));
          if (element.key.includes('vehicleutilization_timebasedutilizationrate')) {
            let hms = this.secondsToHms(element.thresholdValue);
            let hrs = '';
            let mins = '';
            if (hms) {
              hrs = hms.split(',')[0];
              mins = hms.split(',')[1];
            }
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(hrs));
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValuemin', new FormControl(mins));
          } else {
            this.dashboardPreferenceForm.addControl(element.key + 'thresholdValue', new FormControl(element.thresholdValue));
          }

        } else if (section.name.includes('Dashboard.AlertLast24Hours')) {
          _data = element;
          // if (this.translationData[element.key]) {
          //   _data.translatedName = this.translationData[element.key];
          // } else {
          //   _data.translatedName = this.getName(element.name);
          // }
          _data.translatedName = this.getName(element.name);
          this.alertLast24HoursColumnData.push(_data);
        }
      });
    });

    this.setColumnCheckbox();
  }

  secondsToHms(d) {
    d = Number(d);
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
    console.log("selcted data", selectionData);
    console.log("coloumn dataaaa", columnData);
    let saveArr = [];
    this[columnData].forEach(element => {
      let sSearch = this[selectionData].selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      let thresholdType = this.dashboardPreferenceForm.get([element.key + 'thresholdType']).value;     
      let thresholdValue = this.dashboardPreferenceForm.get([element.key + 'thresholdValue']).value;
      if (element.key.includes('_fleetkpi_drivingtime') || element.key.includes('_fleetkpi_idlingtime') || element.key.includes('_todaylivevehicle_timebasedutilizationrate') || element.key.includes('_vehicleutilization_timebasedutilizationrate')) {
        let thresholdValuehrs = thresholdValue * 3600;
        let thresholdValuemin = this.dashboardPreferenceForm.get([element.key + 'thresholdValuemin']).value * 60;
        let totalsecs : number = thresholdValuehrs + thresholdValuemin;
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: "", thresholdType: thresholdType, thresholdValue: totalsecs });
      } else if(element.key.includes('_vehicleutilization_distancebasedutilizationrate') || element.key.includes('_vehicleutilization_distanceperday') || element.key.includes('_vehicleutilization_activevehiclesperday')) {
        let chartType = this.dashboardPreferenceForm.get([element.key + 'chartType']) ? this.dashboardPreferenceForm.get([element.key + 'chartType']).value : "";
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType, thresholdType: thresholdType, thresholdValue: 0 });
      }
      else if(element.key.includes('_vehicleutilization_timebasedutilizationrate') )
      {
        let chartType = this.dashboardPreferenceForm.get([element.key + 'chartType']) ? this.dashboardPreferenceForm.get([element.key + 'chartType']).value : "";
        let thresholdValuehrs = thresholdValue * 3600;
        let thresholdValuemin = this.dashboardPreferenceForm.get([element.key + 'thresholdValuemin']).value * 60;
        let totalsecs : number = thresholdValuehrs + thresholdValuemin;
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: chartType, thresholdType: thresholdType, thresholdValue: totalsecs });
      }
      else {
        let thresholdValuehrs = thresholdValue * 3600;
        let thresholdValuemin = this.dashboardPreferenceForm.get([element.key + 'thresholdValuemin']).value * 60;
        let totalsecs : number = thresholdValuehrs + thresholdValuemin;
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: sSearch.length > 0 ? "A" : "I", preferenceType: "V", chartType: "", thresholdType: thresholdType, thresholdValue: parseInt(thresholdValue) });
      }
    });
    return saveArr;
  }

  onCancel() {
    this.editDashboardFlag = false;
    this.showDashboardReport = true;
    this.setColumnCheckbox();
  }

  // onReset(){
  //   this.setColumnCheckbox();
  // }

  onConfirm() {

    let _fleetKPIArr: any = [];
    let _vehicleUtilizationArr: any = [];
    let _todayLiveVehicleArr: any = [];
    let _alertLast24HoursArr: any = [];

    _fleetKPIArr = this.getSaveObject('fleetKPIColumnData', 'selectionForFleetKPIColumns');
     _vehicleUtilizationArr = this.getSaveObject('vehicleUtilizationColumnData', 'selectionForVehicleUtilizationColumns');
     _todayLiveVehicleArr = this.getSaveObject('todayLiveVehicleColumnData', 'selectionForTodayLiveVehicleColumns');
    // _alertLast24HoursArr = this.getSaveObject('alertLast24HoursColumnData', 'selectionForAlertLast24HoursColumns');

    console.log("save Object", [..._fleetKPIArr, ..._vehicleUtilizationArr, ..._todayLiveVehicleArr, ..._alertLast24HoursArr])
    // return [..._fleetKPIArr, ..._vehicleUtilizationArr, ..._todayLiveVehicleArr, ..._alertLast24HoursArr];


    let objData: any = {
      reportId: this.reportId,
      attributes: [..._fleetKPIArr, ..._vehicleUtilizationArr, ..._todayLiveVehicleArr, ..._alertLast24HoursArr] //-- merge data
    }
    this.reportService.updateReportUserPreference(objData).subscribe((prefData: any) => {
      this.loadDashboardPreferences();
      //this.setDashboardFlag.emit({ flag: false, msg: this.getSuccessMsg() });
      //this.reloadCurrentComponent();
      // if((this.router.url).includes("fleetfuelreport")){
      //  this.reloadCurrentComponent();
      // }
    });
  }

  reloadCurrentComponent() {
    window.location.reload(); //-- reload screen
  }



}



import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { ReplaySubject } from 'rxjs';
import { Util } from 'src/app/shared/util';
import { ReportMapService } from '../../report/report-map.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { MeasurementRuleService } from '../../services/measurementrule.service';
import * as moment from 'moment';

@Component({
  selector: 'app-measurement-campaign-rule',
  templateUrl: './measurement-campaign-rule.component.html',
  styleUrls: ['./measurement-campaign-rule.component.less']
})
export class MeasurementCampaignRuleComponent implements OnInit {
  displayedColumns: string[] = ['seqNumber', 'startTime', 'endTime', 'status', 'OEM', 'vehicleGeneration', 'tcuBrand', 'connectState', 'dataFrequency', 'description', 'action'];
  grpTitleVisible: boolean = false;
  errorMsgVisible: boolean = false;
  //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; 
  prefTimeZone: any = 'dtimezone_Asia/Kolkata';
  prefDateFormat: any = 'ddateformat_dd-mm-yyyy';
  prefTimeFormat: any = 24;
  displayMessage: any;
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  accountRoleId: any;
  showLoadingIndicator: any = false;
  translationData: any = {};
  dataSource: any;
  initData: any = [];
  filterValue: any;
  accountPrefObj: any;
  finalVehicleGroupList: any = [];
  prefDetail: any = {};
  createViewEditStatus: boolean = false;selectedElementData: any;
;
  actionType: any;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  vehicleDisplayPreference: any = 'dvehicledisplay_VehicleIdentificationNumber';
  titleVisible : boolean = false;
  ruleCreatedMsg : any = '';
  public filteredVehicles: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
 
  constructor(private translationService: TranslationService,
    private dialog: MatDialog,
    private reportMapService: ReportMapService,
    private dialogService: ConfirmDialogService,
    private organizationService: OrganizationService,
    private measurementRuleService: MeasurementRuleService) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    if (localStorage.getItem('contextOrgId')) {
      this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    }
    else {
      this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    }
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 17 //-- for alerts
    }
    this.showLoadingIndicator = true;
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.hideloader();
      // this.loadRuleTableData();// uncommnet for API integration
    });
    if(this.prefDetail){
      if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){
        this.proceedStep(this.accountPrefObj.accountPreference); 
      }else{ 
        this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
          this.proceedStep(orgPref);
        }, (error) => {
          this.proceedStep({});
        });
      }
    }
    
    this.initData = [
      {
        SeqNo: 1,
        StartTimestampt : 1648549396000,
        EndTimestamp: 1680085396000,
        Enabled: true,
        OEM: 'DAF',
        VehicleGeneration: '26-BSF-8',
        TCUBrand: 'Bosch',
        ConnectedState: 'Connected',
        Datafrequency: 'High',
        Description: 'Description',
        TargetSystem: "B",
        Action: "H",
        ActionArgument: "H",
        Created_at: "1648549396000",
        Modified_at: "1648549396000",
        Created_by: "2",
      },
      {
        SeqNo: 2,
        StartTimestampt: 1648549396000,
        EndTimestamp: 1680085396000,
        Enabled: false,
        OEM: 'DAF1',
        VehicleGeneration: '26-BSF-8',
        TCUBrand: 'Conti',
        ConnectedState: 'Connected OTA',
        Datafrequency: 'High',
        Description: 'Description',
        TargetSystem: "B",
        Action: "H",
        ActionArgument: "H",
        Created_at: "1648549396000",
        Modified_at: "1648549396000",
        Created_by: "2",
      },
      {
        SeqNo: 3,
        StartTimestampt: 1648549396000,
        EndTimestamp: 1680085396000,
        Enabled: true,
        OEM: 'DAF2',
        VehicleGeneration: '26-BSF-8',
        TCUBrand: 'Bosch',
        ConnectedState: 'Terminated',
        Datafrequency: 'Low',
        Description: 'Description',
        TargetSystem: "B",
        Action: "H",
        ActionArgument: "H",
        Created_at: "1648549396000",
        Modified_at: "1648549396000",
        Created_by: "2",
      }]
      this.initData.forEach(item =>{
        item.StartTimestampt = this.getStarttime(item.StartTimestampt, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone,true);
        item.EndTimestamp = this.getendtime(item.EndTimestamp, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone,true);
        item.Enabled = item.Enabled ? 'Active': 'Inactive';
        // item.StartTimestampt = this.formStartDate(xtempDate, this.prefTimeFormat, this.prefDateFormat);
        // item.startTime = this.timeConversion(item.startTime);
        // item.endTime =  this.timeConversion(item.endTime)
      })
    this.updateDataSource(this.initData);
  }

  proceedStep(preference: any){ 
    let _search = this.prefDetail?.timeformat.filter(i => i.id == preference.timeFormatId);
    this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
    this.prefTimeZone = this.prefDetail?.timezone.filter(i => i.id == preference.timezoneId)[0].name;
    this.prefDateFormat = this.prefDetail?.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
    this.prefUnitFormat = this.prefDetail?.unit.filter(i => i.id == preference.unitId)[0].name; 
    let vehicleDisplayId = preference.vehicleDisplayId;
    console.log(this.prefTimeZone, 'this.prefTimeZone');
    if(vehicleDisplayId) {
      let vehicledisplay = this.prefDetail?.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
      if(vehicledisplay.length != 0) {
        this.vehicleDisplayPreference = vehicledisplay[0].name;
      }
    } 
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  
  loadRuleTableData(){
    this.showLoadingIndicator = true;
    this.measurementRuleService.getMeasurementRules().subscribe((data: any) => {
      if(data){
        this.hideloader();
        data.forEach(element =>{
          element.StartTimestampt = this.getStarttime(element.startTimeStamp, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone,true);
          element.EndTimestamp = this.getendtime(element.EndTimestamp, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone,true);
        })
        this.initData = data;
        this.updateDataSource(this.initData);
      }
    })

  }
  getStarttime(startTime: any, dateFormat: any, timeFormat: any, timeZone: any, addTime?:boolean, onlyTime?: boolean){
    let sTime: any = 0;
    if(startTime != 0){
      sTime = this.formStartendDate(Util.convertUtcToDate(startTime, timeZone), dateFormat, timeFormat, addTime, onlyTime);
    }
    return sTime;
  }
  getendtime(startTime: any, dateFormat: any, timeFormat: any, timeZone: any, addTime?:boolean,onlyTime?: boolean){
    let sTime: any = 0;
    if(startTime != 0){
      sTime = this.formStartendDate(Util.convertUtcToDate(startTime, timeZone), dateFormat, timeFormat, addTime, onlyTime);
    }
    return sTime;
  }
  formStartendDate(date: any, dateFormat: any, timeFormat: any, addTime?:boolean, onlyTime?:boolean){
    // let h = (date.getHours() < 10) ? ('0'+date.getHours()) : date.getHours(); 
    // let m = (date.getMinutes() < 10) ? ('0'+date.getMinutes()) : date.getMinutes(); 
    // let s = (date.getSeconds() < 10) ? ('0'+date.getSeconds()) : date.getSeconds(); 
    // let _d = (date.getDate() < 10) ? ('0'+date.getDate()): date.getDate();
    // let _m = ((date.getMonth()+1) < 10) ? ('0'+(date.getMonth()+1)): (date.getMonth()+1);
    // let _y = (date.getFullYear() < 10) ? ('0'+date.getFullYear()): date.getFullYear();
    let date1 = date.split(" ")[0];
    let time1 = date.split(" ")[1];
    let h = time1.split(":")[0];
    let m = time1.split(":")[1];
    let s = time1.split(":")[2];
    let _d = date1.split("/")[2];
    let _m = date1.split("/")[1];
    let _y = date1.split("/")[0];
    let _date: any;
    let _time: any;
    if(timeFormat == 12){
      _time = (h > 12 || (h == 12 && m > 0 && s>0)) ? `${h == 12 ? 12 : h-12}:${m}:${s} PM` : `${(h == 0) ? 12 : h}:${m}:${s} AM`;
    }else{
      _time = `${h}:${m}:${s}`;
    }
    if(onlyTime){
      return _time; // returns only time
    }
    switch(dateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        if(addTime)
        _date = `${_d}/${_m}/${_y} ${_time}`;
        else
        _date = `${_d}/${_m}/${_y}`;

        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        if(addTime)
        _date = `${_m}/${_d}/${_y} ${_time}`;
        else
        _date = `${_m}/${_d}/${_y}`;
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        if(addTime)
        _date = `${_d}-${_m}-${_y} ${_time}`;
        else
        _date = `${_d}-${_m}-${_y}`;

        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        if(addTime)
        _date = `${_m}-${_d}-${_y} ${_time}`;
        else
        _date = `${_m}-${_d}-${_y}`;

        break;
      }
      default:{
        if(addTime)
        _date = `${_m}/${_d}/${_y} ${_time}`;
        else
        _date = `${_m}/${_d}/${_y}`;

      }
    }
    return _date; //returns dateTime if addTime is true or date if addTime is false
  }


  onClose() {
    this.grpTitleVisible = false;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  
  addNewRule() {
    this.actionType = 'create';
    this.createViewEditStatus = true;
  }
  
  ruleInterpreter() {
    const obj = {
      title: 'Rule Interpreter',
      message: 'Are you sure you want to interpret rule for all Vehicles?',
      cancelText: 'Cancel' ,
      confirmText: 'Interpret'
    };
    this.dialogService.DeleteModelOpen(obj);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res) { //--- delete rule
        // this.driverService.deleteDriver(row.organizationId, row.id).subscribe((deleteDrv) => {
          let msg = 'Interpreted all Rules has been submitted Successfully! ';
          this.successMsgBlink(msg);
        //   this.selectedConsentType = 'All';
        //   this.loadDriverData(); //-- load driver list
        //   this.setConsentDropdown();
        // });
      }
   });
  }

  updateDataSource(tableData: any) {
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
    });
    Util.applySearchFilter(this.dataSource, this.displayedColumns, this.filterValue);
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName: any) {
    if(columnName == "name" || columnName =="category"){
    if (!(a instanceof Number)) a = a.toUpperCase();
    if (!(b instanceof Number)) b = b.toUpperCase();
  }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  timeConversion(time: any){
    let newDate = ''
    if(time){
      newDate = this.reportMapService.getStartTime(time, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true);
    }
    else{
      newDate = '';
    }
    return newDate;
  }

  swapRow(direction, index, tableData){
    console.log(direction, index, tableData,  'swap');
    let tempRowData: any;
    if(direction == 'down'){
      tempRowData = tableData[index+1];
      tableData[index+1] = tableData[index];
      tableData[index] = tempRowData;
      this.updateDataSource(tableData);
    }
    if(direction == 'up'){
      tempRowData = tableData[index-1];
      tableData[index-1] = tableData[index];
      tableData[index] = tempRowData;
      this.updateDataSource(tableData);
    }
  }

  onDelete(row: any){
    const options = {
      title: 'Delete Rule',
      message: 'Are you sure you want to delete rule from the rule list?',
      cancelText: 'Cancel' ,
      confirmText: 'Delete'
    };

    let name = `${row.firstName} ${row.lastName}`;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if(res) { //--- delete rule
        alert('rule deleted');
        // this.driverService.deleteDriver(row.organizationId, row.id).subscribe((deleteDrv) => {
        //   this.successMsgBlink(this.getDeletMsg(name));
        //   this.selectedConsentType = 'All';
        //   this.loadDriverData(); //-- load driver list
        //   this.setConsentDropdown();
        // });
      }
   });
  }

  onViewRule(rowData: any, actiontype: any){
    this.createViewEditStatus= true;
    this.actionType = actiontype;
    this.selectedElementData = rowData;
  }
  onEditRule(rowData: any, actiontype: any){
    this.createViewEditStatus= true;
    this.actionType = actiontype;
    this.selectedElementData = rowData;
    // this.titleText = this.actionType == 'duplicate' ? this.translationData.lblCreateNewAlert  : this.translationData.lblEditAlertDetails;
    // this.rowsData = this.originalAlertData.filter(element => element.id == row.id)[0];

  }

  getRuleDetails(){

  }
  onDuplicateRule(rowData: any, actiontype: any){
    this.createViewEditStatus= true;
    this.actionType = actiontype;
    this.selectedElementData = rowData;
  }

  checkCreationForRule(item: any){
    this.createViewEditStatus = item.stepFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.initData = item.tableData;
    }
    this.loadRuleTableData();
  }
  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.ruleCreatedMsg = msg;
    setTimeout(() => {
      this.titleVisible = false;
    }, 5000);
  }
}

import { Component, Input, OnInit,ViewChild } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { AccountService } from '../../services/account.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { UserDetailTableComponent } from '../../admin/user-management/new-user-step/user-detail-table/user-detail-table.component';
import { MatSort } from '@angular/material/sort';
import { VehicleService } from '../../services/vehicle.service';
import { PackageService } from 'src/app/services/package.service';
import { AlertService } from 'src/app/services/alert.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { ReportMapService } from '../../report/report-map.service';
import { Util } from 'src/app/shared/util';
import { ReplaySubject } from 'rxjs';
import { OrganizationService } from '../../services/organization.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.less']
})

export class AlertsComponent implements OnInit {
  displayedColumns: string[] = ['highUrgencyLevel','name','category','type','thresholdValue','vehicleGroupName','state','action'];
  grpTitleVisible : boolean = false;
  errorMsgVisible: boolean = false;
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  prefTimeFormat: any = 24;
  prefTimeZone: any = 'dtimezone_Asia/Kolkata';
  prefDateFormat: any = 'ddateformat_dd-mm-yyyy';
  highThresholdUnitType: any;
  displayMessage: any;
  createViewEditStatus: boolean = false;
  showLoadingIndicator: any = false;
  actionType: any = '';
  UnitTypeVal: any;
  selectedRowData: any= [];
  singleVehicle: any =[];
  titleText: string;
  translationData: any= {};
  vehicleByVehGroupList: any= [];
  localStLanguage: any;
  dataSource: any;
  initData: any = [];
  originalAlertData: any= [];
  rowsData: any;
  //createStatus: boolean;
  editFlag: boolean = false;
  duplicateFlag: boolean = false;
  accountOrganizationId: any;
  accountId: any;
  accountRoleId: any;
  EmployeeDataService : any= [];
  packageCreatedMsg : any = '';
  titleVisible : boolean = false;
  alertCategoryList: any= [];
  alertTypeList: any= [];
  vehicleGroupList: any= [];
  vehicleList: any= [];
  alertCriticalityList: any= [];
  alertStatusList: any= [];
  alertTypeListBasedOnPrivilege: any= [];
  vehicleGroupListBasedOnPrivilege: any= [];
  vehicleListBasedOnPrivilege: any= [];
  stringifiedData: any;
  parsedJson: any;
  filterValues = {};
  prefUnit: any;
  unitId: any;
  alertCategoryTypeMasterData: any= [];
  alertCategoryTypeFilterData: any= [];
  associatedVehicleData: any= [];
  //generalPreferences: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  dialogVeh: MatDialogRef<UserDetailTableComponent>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  filterValue: any;
  accountPrefObj: any;
  prefData: any;
  vehicleDisplayPreference: any= 'dvehicledisplay_VehicleIdentificationNumber';
  finalVehicleGroupList: any = [];

  public filteredVehicles: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(
    private translationService: TranslationService,
    private accountService: AccountService,
    private packageService: PackageService,
    private dialog: MatDialog,
    private vehicleService: VehicleService,
    private alertService: AlertService,
    private dialogService: ConfirmDialogService,
    private reportMapService: ReportMapService,
    private organizationService: OrganizationService) { }

    ngOnInit() {
      this.localStLanguage = JSON.parse(localStorage.getItem("language"));
      this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
      //this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
      if(localStorage.getItem('contextOrgId')){
        this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
      }
      else{
        this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
      }
      // let accountPreference = JSON.parse(localStorage.getItem('accountInfo')).accountPreference;
      // this.unitId = accountPreference.unitId;
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
        // this.loadFiltersData();
        this.loadDataBasedOnPrivileges();
      });
      this.translationService.getPreferences(this.localStLanguage).subscribe((res: any) => {
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(res, this.accountPrefObj.accountPreference);
          //this.showLoadingIndicator = false;
        }else{ //-- org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.proceedStep(res, orgPref);
            //this.showLoadingIndicator = false;
          }, (error) => { // failed org API
            //this.showLoadingIndicator = false;
            let pref: any = {};
            this.proceedStep(res, pref);
          });
        }

      });
    }

    proceedStep(_prefData: any, preference: any){
      //this.generalPreferences = _prefData; 
      //this.getUnits();
      this.prefData = _prefData;
      let _search = _prefData?.timeformat.filter(i => i.id == preference.timeFormatId);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = _prefData?.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = _prefData?.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = _prefData?.unit.filter(i => i.id == preference.unitId)[0].name; 
      let vehicleDisplayId = preference.vehicleDisplayId;
      if(vehicleDisplayId) {
        let vehicledisplay = _prefData?.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        if(vehicledisplay.length != 0) {
          this.vehicleDisplayPreference = vehicledisplay[0].name;
        }
      } 
    }

    // getUnits() {
    //   let unitObj = this.generalPreferences?.unit.filter(item => item.id == this.unitId);
    //   if (unitObj && unitObj.length != 0) {
    //     this.prefUnit = unitObj[0].value;
    //     if (this.prefUnit == 'Imperial') {
    //       this.prefUnitFormat = 'dunit_Imperial';
    //     } else {
    //       this.prefUnitFormat = 'dunit_Metric';
    //     }
    //   }
    // }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    ////console.log("process translationData:: ", this.translationData)
  }

  loadFiltersData(){
    this.showLoadingIndicator = true;
    this.alertService.getAlertFilterData(this.accountId, this.accountOrganizationId).subscribe((data) => {
      let filterData = data["enumTranslation"];
      filterData.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });
      this.alertCategoryList= filterData.filter(item => item.type == 'C');
      this.alertTypeList= filterData.filter(item => item.type == 'T');
      // //console.log("alertTypeList=" +this.alertTypeList);
      // //console.log("filterData=" +filterData);
      this.alertCriticalityList= filterData.filter(item => item.type == 'U');
      this.vehicleList= data["vehicleGroup"].filter(item => item.vehicleName != '');
      this.vehicleList = this.removeDuplicates(this.vehicleList, "vehicleName");

      this.alertStatusList=[{
       id: 1,
       value:"A",
       key:this.translationData.lblActive
      },{
        id: 2,
        value:"I",
        key:this.translationData.lblSuspended
       }
    ]
    this.loadDataBasedOnPrivileges();

    }, (error) => {

    })
  }

  loadDataBasedOnPrivileges(){
    this.showLoadingIndicator = true;
    this.alertService.getAlertFilterDataBasedOnPrivileges(this.accountId, this.accountRoleId).subscribe((data) => {
      this.alertCategoryTypeMasterData = data["enumTranslation"];
      this.alertCategoryTypeFilterData = data["alertCategoryFilterRequest"];
      this.associatedVehicleData = data["associatedVehicleRequest"];

      let alertTypeMap = new Map();
      this.alertCategoryTypeFilterData.forEach(element => {
        alertTypeMap.set(element.featureKey, element.featureKey);
      });

      this.alertCriticalityList= this.alertCategoryTypeMasterData.filter(item => item.type == 'U');
      this.alertCriticalityList.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });

      if(alertTypeMap != undefined){
        this.alertCategoryTypeMasterData.forEach(element => {
          if(alertTypeMap.get(element.key)){
            element["value"]= this.translationData[element["key"]];
            this.alertTypeListBasedOnPrivilege.push(element);
          }
        });
      }

      this.alertTypeList = this.alertTypeListBasedOnPrivilege;

      if(this.alertTypeList.length != 0){
        this.alertCategoryTypeMasterData.forEach(element => {
          this.alertTypeList.forEach(item => {
            if(item.parentEnum == element.enum && element.parentEnum == ""){
              element["value"]= this.translationData[element["key"]];
              this.alertCategoryList.push(element);
            }
          });
        });
        this.alertCategoryList = this.getUnique(this.alertCategoryList, "enum")
      }

      this.associatedVehicleData.forEach(element => {
        if(element.vehicleGroupDetails && element.vehicleGroupDetails != ""){
          let vehicleGroupDetails= element.vehicleGroupDetails.split(",");
          vehicleGroupDetails.forEach(item => {
            let itemSplit = item.split("~");
            if(itemSplit[2] != 'S') {
            let vehicleGroupObj= {
              "vehicleGroupId" : (itemSplit[0] && itemSplit[0] != '') ? parseInt(itemSplit[0]) : 0,
              "vehicleGroupName" : itemSplit[1],
              "vehicleId" : parseInt(element.vehicleId)
            }
            this.vehicleGroupListBasedOnPrivilege.push(vehicleGroupObj);
            this.vehicleList.push(vehicleGroupObj);
           }
           else{
            this.singleVehicle.push(element);
           }
          });
        }
        if(element.vehicleId && element.vehicleId != ''){
          this.vehicleListBasedOnPrivilege.push({"vehicleId" : parseInt(element.vehicleId)});
        }
      });

      this.vehicleGroupListBasedOnPrivilege = this.removeDuplicates(this.vehicleGroupListBasedOnPrivilege, "vehicleGroupId");
      // this.vehicleList = this.vehicleGroupListBasedOnPrivilege;
      this.finalVehicleGroupList = this.vehicleGroupListBasedOnPrivilege.filter(i => i.vehicleGroupId >= 0); // NaN removed

      this.vehicleByVehGroupList = this.getUniqueVINs([...this.associatedVehicleData, ...this.singleVehicle]);
      // this.resetVehiclesFilter();
      this.alertStatusList = [{
        id: 1,
        value: "A",
        key:this.translationData.lblActive
      }, {
        id: 2,
        value: "I",
        key:this.translationData.lblSuspended
      }
      ]

      this.loadAlertsData();
    })
  }

  getUniqueVINs(vinList: any){
    let uniqueVINList = [];
    for(let vin of vinList){
      let vinPresent = uniqueVINList.map(element => element.vin).indexOf(vin.vin);
      if(vinPresent == -1) {
        uniqueVINList.push(vin);
      }
    }
    return uniqueVINList;
  }

  getUnique(arr, comp) {

    // store the comparison  values in array
    const unique =  arr.map(e => e[comp])
      // store the indexes of the unique objects
      .map((e, i, final) => final.indexOf(e) === i && i)
      // eliminate the false indexes & return unique objects
    .filter((e) => arr[e]).map(e => arr[e]);

    return unique;
  }

  removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject  = {};
    for(var i in originalArray) {
       lookupObject[originalArray[i][prop]] = originalArray[i];
    }
    for(i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
     return newArray;
  }
  onClickNewAlert(){
    //this.titleText = this.translationData.lblAddNewGroup || "Add New Alert";
    this.actionType = 'create';
    this.createViewEditStatus = true;
  }
  onClose(){
    this.grpTitleVisible = false;
  }

  onBackToPage(objData){
    this.createViewEditStatus = objData.actionFlag;
    if(objData.successMsg && objData.successMsg != ''){
      this.successMsgBlink(objData.successMsg);
    }
    this.loadAlertsData();
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

  makeRoleAccountGrpList(initdata: any){
    let accountId =  localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    initdata = initdata.filter(item => item.id != accountId);
    initdata.forEach((element, index) => {
      let roleTxt: any = '';
      let accGrpTxt: any = '';
      element.roles.forEach(resp => {
        roleTxt += resp.name + ', ';
      });
      element.accountGroups.forEach(resp => {
        accGrpTxt += resp.name + ', ';
      });

      if(roleTxt != ''){
        roleTxt = roleTxt.slice(0, -2);
      }
      if(accGrpTxt != ''){
        accGrpTxt = accGrpTxt.slice(0, -2);
      }

      initdata[index].roleList = roleTxt;
      initdata[index].accountGroupList = accGrpTxt;
    });

    return initdata;
  }

  loadAlertsData(){
    this.initData = [];
    let obj: any = {
      accountId: 0,
      organizationId: this.accountOrganizationId,
      accountGroupId: 0,
      vehicleGroupGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.showLoadingIndicator = true;
    this.alertService.getAlertData(this.accountId, this.accountOrganizationId).subscribe((data) => {
      this.hideloader();
      let initDataBasedOnPrivilege = data;
      // this.initData =data;
      initDataBasedOnPrivilege.forEach(item => {
        let hasPrivilege = (this.alertTypeListBasedOnPrivilege.filter(element => element.enum == item.type).length > 0 && (this.vehicleGroupListBasedOnPrivilege.filter(element => element.vehicleGroupId == item.vehicleGroupId).length > 0 || this.vehicleListBasedOnPrivilege.filter(element => element.vehicleId == item.vehicleGroupId).length > 0));
        if(hasPrivilege){
          this.initData.push(item);
        }
      })

      this.originalAlertData= JSON.parse(JSON.stringify(data)); //Clone array of objects
      this.initData.forEach(item => {
        this.setUnitOfThreshold(item);

      let catVal = this.alertCategoryList.filter(cat => cat.enum == item.category);
      catVal.forEach(obj => {
        item["category"]=obj.value;
      });
      let typeVal = this.alertTypeList.filter(type => type.enum == item.type);
      typeVal.forEach(obj => {
        item["keyType"] = obj.key; // add feature key
        item["type"] = obj.value;
      });

      let alertUrgency=({
      alertFilterRefs: [],
      alertId: 42,
      createdAt: 1621588594280,
      dayType: [],
      id: 38,
      modifiedAt: 0,
      periodType: "A",
      state: "A",
      thresholdValue: 25,
      unitType: "H",
      urgencyLevelType: "W",
      urgencylevelEndDate: 0,
      urgencylevelStartDate: 0
    })
      // let alertUrgency =({
      // thresholdValue: 300,
      // urgencyLevelType: "A"
      // })

     // item.alertUrgencyLevelRefs.push(alertUrgency);
      //item.alertUrgencyLevelRefs.push(alertUrgency);

      let critical  = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'C');
      let warning   = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'W');
      let advisory   = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'A');

      if(critical.length > 0){
      critical.forEach(obj => {
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});
      if(obj.unitType !='N') {
      item.highThresholdValue = this.getConvertedThresholdValues(item.highThresholdValue, obj.unitType);
      }
      });
      }
      else if(warning.length > 0){
      warning.forEach(obj => {
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});
      if(obj.unitType !='N') {
      item.highThresholdValue = this.getConvertedThresholdValues(item.highThresholdValue, obj.unitType);
      }
    });
      }
      else {
      advisory.forEach(obj => {
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});
      if(obj.unitType !='N') {
      item.highThresholdValue = this.getConvertedThresholdValues(item.highThresholdValue, obj.unitType);
      }
    });
      }
      if(item.vehicleGroupName!=''){
        this.vehicleGroupList.push({value:item.vehicleGroupName, key:item.vehicleGroupName});
        this.vehicleGroupList = this.vehicleGroupList.filter((test, index, array) =>
          index === array.findIndex((findTest) =>
          findTest.value === test.value
          )
       );
      } else {//according to pref
        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if(vehicleDisplayId) {
          let vehicledisplay = this.prefData?.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if(vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }
        if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName'){
          item.vehicleGroupName = item.vehicleName;
        }
        else if(this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber'){
          item.vehicleGroupName = item.vin;
        }
        else{
          item.vehicleGroupName = item.regNo;
        }
        // item.vehicleGroupName = item.vehicleName;

      }
     });
      this.updateDatasource(this.initData);
    }, (error) => {
      this.hideloader();
    });
 }

 getConvertedThresholdValues(originalThreshold,unitType){
   let threshold;
   if(unitType == 'H' || unitType == 'T' ||unitType == 'S'){
    threshold =this.reportMapService.getConvertedTime(originalThreshold,unitType);
   }
   else if(unitType == 'K' || unitType == 'L'){
    threshold =this.reportMapService.getConvertedDistance(originalThreshold,unitType);
    }
   else if(unitType == 'A' || unitType == 'B'){
    threshold =this.reportMapService.getConvertedSpeed(originalThreshold,unitType);
   }
   else if(unitType == 'P'){
     threshold=originalThreshold;
   }

   return threshold;
 }

 setUnitOfThreshold(item){
  let unitTypeEnum, unitType;
  switch(item.category +item.type){
    case 'LH': unitTypeEnum= "H";
    item.UnitTypeVal =  this.translationData.lblHours;
    break;

    case 'LD':
    if(this.prefUnitFormat == 'dunit_Metric'){
      unitTypeEnum= "K";
      item.UnitTypeVal = this.translationData.lblkm;
     }
      else{
      unitTypeEnum= "L";
      item.UnitTypeVal = this.translationData.lblmile;
      }
      break;

    case 'LU': unitTypeEnum= "H";
    // item.UnitTypeVal =  this.translationData.lblHours;
     unitType = item.alertUrgencyLevelRefs? item.alertUrgencyLevelRefs[0].unitType : 'S';
    if(unitType == 'H'){
      item.UnitTypeVal = this.translationData.lblHours;
    }
    else if(unitType == 'T'){
      item.UnitTypeVal = this.translationData.lblMinutes;
    }
    else{
      item.UnitTypeVal = this.translationData.lblSeconds;
    }
     break;

    case 'LG':
    if(this.prefUnitFormat == 'dunit_Metric'){
      unitTypeEnum= "K";
      item.UnitTypeVal = this.translationData.lblkm;}
      else{
      unitTypeEnum= "L";
      item.UnitTypeVal = this.translationData.lblmile;
      }
     break;

    case 'FP': unitTypeEnum= "P";
    item.UnitTypeVal =  "%"
    break;

    case 'FL': unitTypeEnum= "P";
    item.UnitTypeVal =  "%"
    break;

    case 'FT': unitTypeEnum= "P";
    item.UnitTypeVal =  "%"
    break;

    case 'FI': unitTypeEnum= "S";
    // item.UnitTypeVal = this.translationData.lblSeconds;
    unitType = item.alertUrgencyLevelRefs? item.alertUrgencyLevelRefs[0].unitType : 'S';
    if(unitType == 'H'){
      item.UnitTypeVal = this.translationData.lblHours;
    }
    else if(unitType == 'T'){
      item.UnitTypeVal = this.translationData.lblMinutes;
    }
    else{
      item.UnitTypeVal = this.translationData.lblSeconds;
    }
    break;

    case 'FA':
    if(this.prefUnitFormat == 'dunit_Metric'){
      unitTypeEnum= "A";
      item.UnitTypeVal = this.translationData.lblkilometerperhour;
    }
      else{
        unitTypeEnum= "B";
        item.UnitTypeVal = this.translationData.lblMilesPerHour
      }
      break;

    case 'FF': unitTypeEnum= "P";
    item.UnitTypeVal =  "%"
    break;

    return item.UnitTypeVal;
  }
 }

 getFilteredValues(dataSource){
  this.dataSource = dataSource;
  this.dataSource.paginator = this.paginator;
  this.dataSource.sort = this.sort;
}
  updateDatasource(data){
    if(data && data.length > 0){
      this.initData = this.getNewTagData(data);
    }
    this.initData = data;
    this.dataSource = new MatTableDataSource(this.initData);
    this.initData.forEach((ele,index) => {
      if(ele.state == 'A'){
        this.initData[index]["status"] = this.translationData.lblActive;
      }
      if(ele.state == 'I'){
        this.initData[index]["status"] =this.translationData.lblSuspended;
      }
    });
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      // this.dataSource.sortData = (data: String[], sort: MatSort) => {
      //   const isAsc = sort.direction === 'asc';
      //   return data.sort((a: any, b: any) => {
      //     var a1;
      //     var b1;
      //     if(sort.active && sort.active === 'thresholdValue'){
      //       a1 = a.highThresholdValue;
      //       b1 = b.highThresholdValue;
      //     } else {
      //       a1 = a[sort.active];
      //       b1 = b[sort.active];
      //     }
      //     return this.compare(a1, b1, isAsc);
      //   });
      //  }
      this.dataSource.filterPredicate = function(data: any, filter: string): boolean {
        return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.category.toString().toLowerCase().includes(filter) ||
         data.type.toString().toLowerCase().includes(filter) ||
         data.vehicleGroupName.toString().toLowerCase().includes(filter) ||
        data.status.toString().toLowerCase().includes(filter)
       // data.highThresholdValue.toString().includes(filter)
      );
    };
      this.dataSource.sortData = (data : String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
    });
    // Util.applySearchFilter(this.dataSource, this.displayedColumns , this.filterValue );
  }
  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName: any) {
    if(columnName == "name" || columnName =="category"){
      if(!(a instanceof Number)) a = a.toString().toUpperCase();
      if(!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }


  // compare(a: any, b: any, isAsc: boolean) {
  //   if(!(a instanceof Number) && isNaN(a)) a = a.toUpperCase();
  //   if(!(b instanceof Number) && isNaN(b)) b = b.toUpperCase();
  //   return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  // }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      if(row.createdAt){
        let createdDate = parseInt(row.createdAt);
        let nextDate = createdDate + 86400000;
        if(currentDate >= createdDate && currentDate < nextDate){
          row.newTag = true;
        }
        else{
          row.newTag = false;
        }
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData, newFalseData);
    return newTrueData;
  }

  onDeleteAlert(item: any) {
    const options = {
      title: this.translationData.lblDeleteAlert,
      message: this.translationData.lblAreousureyouwanttodeleteAlert,
      cancelText: this.translationData.lblCancel,
      confirmText: this.translationData.lblDelete
    };
    let name = item.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.alertService.deleteAlert(item.id).subscribe((res) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadAlertsData();
        }, error => {
          if(error.status == 409){
            this.errorMsgBlink(this.getDeletMsg(name, true));
          }
        });
    }
   });
  }

  getDeletMsg(alertName: any, isError? :boolean){
    if(!isError){
      if(this.translationData.lblAlertDelete)
        return this.translationData.lblAlertDelete.replace('$', alertName);
      else
        return ("Alert '$' was successfully deleted").replace('$', alertName);
    }
    else{
      if(this.translationData.lblAlertDeleteError)
        return this.translationData.lblAlertDeleteError.replace('$', alertName);
      else
        return ("Alert '$' cannot be deleted as notification is associated with this").replace('$', alertName);
    }
  }

  onViewAlert(row: any, action: any) {
    this.createViewEditStatus= true;
    this.actionType = action;
    this.rowsData = this.originalAlertData.filter(element => element.id == row.id)[0];
  }

  onEditDuplicateAlert(row: any, action : string) {
    this.createViewEditStatus= true;
    this.actionType = 'edit';
    if(action == 'duplicate'){
      this.actionType = 'duplicate';
    }
    this.titleText = this.actionType == 'duplicate' ? this.translationData.lblCreateNewAlert  : this.translationData.lblEditAlertDetails;
    this.rowsData = this.originalAlertData.filter(element => element.id == row.id)[0];
    //this.rowsData.push(row);
    //this.editFlag = true;
    //this.createStatus = false;
  }

   successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {
      this.grpTitleVisible = false;
    }, 5000);
  }

  errorMsgBlink(errorMsg: any){
    this.errorMsgVisible = true;
    this.displayMessage = errorMsg;
    setTimeout(() => {
      this.errorMsgVisible = false;
    }, 5000);
  }

  onChangeAlertStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert,
      message:  (rowData.state == 'A') ?  this.translationData.lblYouwanttoDeActivate  : this.translationData.lblYouwanttoActivate,
      cancelText: this.translationData.lblCancel,
      confirmText: (rowData.state == 'A') ? this.translationData.lblDeactivate : this.translationData.lblActivate,
      status: rowData.state == 'A' ? 'Suspend' : 'Activate' ,
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){
       if(rowData.state == 'A'){
          this.alertService.suspendAlert(rowData.id).subscribe((data) => {
            this.loadAlertsData();
            // let successMsg = "Updated Successfully!";
            // this.successMsgBlink(successMsg);
          }, error => {
            this.loadAlertsData();
          });
       }
       else{
        this.alertService.activateAlert(rowData.id).subscribe((data) => {
          this.loadAlertsData();
          // let successMsg = "Updated Successfully!";
          // this.successMsgBlink(successMsg);
        }, error => {
          this.loadAlertsData();
        });

       }

      }else {
        this.loadAlertsData();
      }
    });
  }

  onVehicleGroupClick(data: any) {
    const colsList = ['name','vin','licensePlateNumber', 'status'];
    const colsName =[this.translationData.lblVehicleName, this.translationData.lblVIN, this.translationData.lblRegistrationNumber, this.translationData.lblStatus];
    const tableTitle =`${data.vehicleGroupName} - ${this.translationData.lblVehicles}`;
   let objData = {
      groupId: data.vehicleGroupId,
      groupType: 'G',
      functionEnum: 'A',
      organizationId: data.organizationId
      // groupType: data.groupType,
      // functionEnum: data.functionEnum
    }
    this.showLoadingIndicator = true;
    this.vehicleService.getVehiclesDetails(objData).subscribe((vehList: any) => {
      this.hideloader();
      let _vehList: any = this.addSubscribrNonSubscribeStatus(vehList, data); // #22342
      this.callToCommonTable(_vehList, colsList, colsName, tableTitle);
    }, (error) => {
      this.hideloader();
    });
  }

  addSubscribrNonSubscribeStatus(_vehList: any, rowData: any){
    if(rowData && rowData.keyType && rowData.keyType != ''){
      let featuresData: any = this.alertCategoryTypeFilterData.filter(i => i.featureKey == rowData.keyType);
      if(featuresData.length == 1 && featuresData[0].subscriptionType == 'O'){ // org based - all subscribe
        _vehList.forEach(element => {
          element.status = "Subscribed";
        });
      }else{
        if(featuresData.length > 0){
          let _orgSub: any = featuresData.filter(i => i.subscriptionType == 'O'); // find org based subscription
          if(_orgSub && _orgSub.length > 0){ // all vehicle subscribe
            _vehList.forEach(element => {
              element.status = "Subscribed";
            });
          }else{ // all vehicle subscribe
            _vehList.forEach(_el => {
              let _find: any = featuresData.filter(j => j.vehicleId == _el.id && j.subscriptionType == 'V');
              if(_find && _find.length > 0){ // find 'V'
                _el.status = "Subscribed";
              }else{ // non-subscribe
                _el.status = "Non-Subscribed";
              }
            });
          }
        }
      }
    }
    return _vehList;
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: colsList,
      colsName: colsName,
      tableTitle: tableTitle,
      translationData: this.translationData,
      popupWidth: true
    }
    this.dialogVeh = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }
}

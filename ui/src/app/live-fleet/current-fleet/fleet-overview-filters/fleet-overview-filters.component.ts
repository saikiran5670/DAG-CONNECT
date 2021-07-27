import { Component, Input, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { validateBasis } from '@angular/flex-layout';
import { TranslationService } from '../../../services/translation.service';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { DataInterchangeService } from '../../../services/data-interchange.service';
import { MessageService } from 'src/app/services/message.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-fleet-overview-filters',
  templateUrl: './fleet-overview-filters.component.html',
  styleUrls: ['./fleet-overview-filters.component.less']
})
export class FleetOverviewFiltersComponent implements OnInit {
@Input() translationData: any;
@Input() detailsData: any;
@Input() fromVehicleHealth: any;
@Input() vehInfoPrefData: any;
tabVisibilityStatus: boolean = true;
selectedIndex: number = 0;
filterData: any;
filterValue: any;
selection1: any;
selection2: any;
selection3: any;
filterVehicleForm:FormGroup;
todayFlagClicked: boolean = true;
driverFlagClicked: boolean = true;
isVehicleDetails: boolean = false;
isVehicleListOpen: boolean = true;
noRecordFlag: boolean = false;
groupList : any= [];
driverList: any = [];
categoryList : any= [];
vehicleListData: any = [];
driverListData: any = [];
levelList : any= [];
healthList : any= [];
otherList : any= [];
driverVehicleForm: FormGroup;
showLoadingIndicator: any = false;
dataSource: any = new MatTableDataSource([]);
initData: any = [];
objData: any;
localStLanguage: any;
accountOrganizationId: any;
translationAlertData: any = {};
svgIcon:any;
displayedColumns: string[] = ['icon','vin','driverName','drivingStatus','healthStatus'];
@ViewChild('dataContainer') dataContainer: ElementRef;
messages: any[] = [];
subscription: Subscription;

  constructor(private messageService: MessageService, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private sanitizer: DomSanitizer,
    private dataInterchangeService: DataInterchangeService) { 
      this.subscription = this.messageService.getMessage().subscribe(message => {
        if (message.key.indexOf("refreshData") !== -1) {
          this.loadVehicleData();
        }
      });
    }

  ngOnInit(): void {
    
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 17 
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);    
    });
   
    this.selection1 = ['all'];
    this.selection2 = ['all'];
    this.selection3 = ['all'];
    this.filterVehicleForm = this._formBuilder.group({
      group: ['all'],
      level: ['all'],
      category: ['all'],
      status: ['all'],
      otherFilter: ['all']
    })
    this.driverVehicleForm = this._formBuilder.group({
      driver: ['all'],
    })   
    if(this.selectedIndex == 0){
    this.getFilterData();
    }
    if(this.selectedIndex == 1){
    this.getDriverData();
    }
    this.drawIcons(this.detailsData);
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }
  
  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }
  
  processTranslation(transData: any) {
    this.translationAlertData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }


  getDriverData(){
    this.reportService.getFilterDetails().subscribe((data: any) => {
      this.filterData = data;
      this.driverList = [];
      if(!this.driverFlagClicked  && this.selectedIndex == 1){
        this.filterData["driverList"].forEach(item=>{
          this.driverList.push(item) });
          // this.groupList = this.removeDuplicates(this.groupList, "vehicleGroupName");
          this.loadDriverData();
      }   
      else{
        this.loadDriverData(); 
        this.detailsData.forEach(element => {
  
          let currentDate = new Date().getTime();
            let createdDate = parseInt(element.latestProcessedMessageTimeStamp); 
            let nextDate = createdDate + 86400000;
            if(currentDate > createdDate && currentDate < nextDate){
            let driverData =this.filterData["driverList"].filter(item => item.driverId == element.driver1Id);
            driverData.forEach(item=>
              this.driverList.push(item));
            }
          })
          this.loadDriverData();
      }

    })
  }

  loadDriverData(){  
    let newAlertCat=[];
    if(!this.driverFlagClicked && this.selectedIndex == 1)
    {
      this.objData = {
        "groupId": ['all'],
        "alertLevel": ['all'],
        "alertCategory": ['all'],
        "healthStatus": ['all'],
        "otherFilter": ['all'],
        "driverId": ['all'],
        "days": 90,
        "languagecode":"cs-CZ"
    }}
    if(this.driverFlagClicked && this.selectedIndex == 1)
    {
      this.objData = {
        "groupId": ['all'],
        "alertLevel": ['all'],
        "alertCategory": ['all'],
        "healthStatus": ['all'],
        "otherFilter": ['all'],
        "driverId": [this.driverVehicleForm.controls.driver.value.toString()],
        "days": 0,
        "languagecode":"cs-CZ"
      }
    }
    let driverSelected = this.driverList.filter((elem)=> elem.driver1Id === this.driverVehicleForm.get("driver").value);
    this.reportService.getFleetOverviewDetails(this.objData).subscribe((data:any) => {
      let val = [{driver : driverSelected.vehicleGroupName, data : data}];
      this.messageService.sendMessage(val);
      this.messageService.sendMessage("refreshTimer");
      this.drawIcons(data);
      data.forEach(item => {
        this.filterData["healthStatus"].forEach(e => {
         if(item.vehicleHealthStatusType==e.value)
         {         
          item.vehicleHealthStatusType = this.translationData[e.name];
         }
        });
        this.filterData["otherFilter"].forEach(element => {
          if(item.vehicleDrivingStatusType==element.value)
          {         
           item.vehicleDrivingStatusType = this.translationData[element.name];
          }
         });         
      //    if(this.categoryList.length>0){
      //    item.fleetOverviewAlert.forEach(e => {
      //    let alertCategory = this.categoryList.filter((ele)=> ele.value == e.categoryType);
      //    if(alertCategory.length>0){
      //    newAlertCat.push(alertCategory[0]);
      //    }          
      //   });  
      //  }
      });    
    //  this.categoryList = this.removeDuplicates(newAlertCat, "value");
    //  console.log(newAlertCat);    
      this.driverListData = data;     
      let _dataObj ={
        vehicleDetailsFlag : this.isVehicleDetails,
        data:data
      }
      this.dataInterchangeService.getVehicleData(_dataObj);//change as per filter data
          
    }, (error) => {
      let val = [{vehicleGroup : driverSelected.driver, data : error}];
      this.messageService.sendMessage(val);
      this.messageService.sendMessage("refreshTimer");
      if (error.status == 404) {
        this.noRecordFlag = true;
        let _dataObj ={
          vehicleDetailsFlag : this.isVehicleDetails,
          data:null
        }
        this.dataInterchangeService.getVehicleData(_dataObj);
      }
    });
    this.noRecordFlag = false;
 } 

getFilterData(){
  this.reportService.getFilterDetails().subscribe((data: any) => {
    this.filterData = data;
    this.groupList = [];
    this.categoryList = [];
    this.levelList = [];
    this.healthList = [];
    this.otherList = [];
    if(!this.todayFlagClicked && this.selectedIndex == 0){
        this.filterData["vehicleGroups"].forEach(item=>{
        this.groupList.push(item) });
        this.groupList = this.removeDuplicates(this.groupList, "vehicleGroupName");
    
        this.filterData["alertCategory"].forEach(item=>{
        let catName =  this.translationAlertData[item.name];
        this.categoryList.push({'name':catName, 'value': item.value})});     
       
        this.filterData["alertLevel"].forEach(item=>{
        let levelName =  this.translationAlertData[item.name];
        this.levelList.push({'name':levelName, 'value': item.value})}); 
      
        this.filterData["healthStatus"].forEach(item=>{
        let statusName = this.translationData[item.name];
        this.healthList.push({'name':statusName, 'value': item.value})});

        this.filterData["otherFilter"].forEach(item=>{
        if(item.value=='N'){
        let statusName = this.translationData[item.name];           
        this.otherList.push({'name':statusName, 'value': item.value})
        }});
       
       this.detailsData.forEach(item => {
       this.filterData["healthStatus"].forEach(e => {
         if(item.vehicleHealthStatusType==e.value)
         {         
          item.vehicleHealthStatusType = this.translationData[e.name];
         }
        });
        this.filterData["otherFilter"].forEach(element => {
          if(item.vehicleDrivingStatusType==element.value)
          {         
           item.vehicleDrivingStatusType = this.translationData[element.name];
          }
         });        
      }); 
      this.vehicleListData = this.detailsData; 
      this.loadVehicleData();
    }
    if(this.todayFlagClicked  && this.selectedIndex == 0){
      this.loadVehicleData(); 
      this.detailsData.forEach(element => {

        let currentDate = new Date().getTime();
          let createdDate = parseInt(element.latestProcessedMessageTimeStamp); 
          let nextDate = createdDate + 86400000;
          if(currentDate > createdDate && currentDate < nextDate){
          let vehicleData =this.filterData["vehicleGroups"].filter(item => item.vin == element.vin);
          vehicleData.forEach(item=>
            this.groupList.push(item));
          }

    })
    let currentDate = new Date().getTime();
        let categoryData =this.filterData["fleetOverviewAlerts"].forEach(element => {
          let createdDate = parseInt(element.alertTime); 
          let nextDate = createdDate + 86400000;
          if(currentDate > createdDate && currentDate < nextDate){
            this.categoryList.push(element);
            this.healthList.push(element);
          }
        });
 
        this.filterData["healthStatus"].forEach(item=>{
        let statusName = this.translationData[item.name];
        this.healthList.push({'name':statusName, 'value': item.value})});

        this.filterData["otherFilter"].forEach(item=>{
        if(item.value=='N'){
        let statusName = this.translationData[item.name];           
        this.otherList.push({'name':statusName, 'value': item.value})
        }});

    }
  })
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

  applyFilter(filterValue: string) {
    this.vehicleListData = this.detailsData;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();

    const filteredData = this.detailsData.filter(value => {​​​​​​​​
      const searchStr = filterValue.toLowerCase();
      const vin = value.vin.toLowerCase().toString().includes(searchStr);
      const driver = value.driverName.toLowerCase().toString().includes(searchStr);
      const drivingStatus = value.vehicleDrivingStatusType.toLowerCase().toString().includes(searchStr);
      const healthStatus = value.vehicleHealthStatusType.toLowerCase().toString().includes(searchStr);
      return vin || driver || drivingStatus ||healthStatus;
    }​​​​​​​​);
  
  
    this.vehicleListData = filteredData;
    
  }

  applyFilterDriver(filterValue: string) {
    this.driverListData = this.detailsData;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();

    const filteredData = this.detailsData.filter(value => {​​​​​​​​
      const searchStr = filterValue.toLowerCase();
      const vin = value.vin.toLowerCase().toString().includes(searchStr);
      const driver = value.driverName.toLowerCase().toString().includes(searchStr);
      const drivingStatus = value.vehicleDrivingStatusType.toLowerCase().toString().includes(searchStr);
      const healthStatus = value.vehicleHealthStatusType.toLowerCase().toString().includes(searchStr);
      return vin || driver || drivingStatus ||healthStatus;
    }​​​​​​​​);

    this.driverListData = filteredData;
  }

  onChangeGroup(id: any){
    this.filterVehicleForm.get("group").setValue(id);
    this.loadVehicleData();
  }

  onChangeLevel(id: any){
    this.filterVehicleForm.get("level").setValue(id);
    this.loadVehicleData();
  }

  onChangeCategory(id: any){
    this.filterVehicleForm.get("category").setValue(id);
    this.loadVehicleData();
  }

  onChangHealthStatus(id: any){
    this.filterVehicleForm.get("status").setValue(id);
    this.loadVehicleData();
  }

  onChangeOtherFilter(id: any){
    this.filterVehicleForm.get("otherFilter").setValue(id);
    this.loadVehicleData();
  }

  onChangeDriver(id: any){
    this.driverVehicleForm.get("driver").setValue(id);
    this.loadDriverData();
  }
  
  loadVehicleData(){  
    this.initData =this.detailsData;
    let newAlertCat=[];
    if(!this.todayFlagClicked  && this.selectedIndex == 0)
    {
      this.objData = {
        "groupId": [this.filterVehicleForm.controls.group.value.toString()],
        "alertLevel": [this.filterVehicleForm.controls.level.value.toString()],
        "alertCategory": this.filterVehicleForm.controls.category.value,
        "healthStatus": this.filterVehicleForm.controls.status.value,
        "otherFilter": [this.filterVehicleForm.controls.otherFilter.value.toString()],
        "driverId": ["all"],
        "days": 90,
        "languagecode":"cs-CZ"
    }}
    if(this.todayFlagClicked  && this.selectedIndex == 0)
    {
      this.objData = {
        "groupId": [this.filterVehicleForm.controls.group.value.toString()],
        "alertLevel": [this.filterVehicleForm.controls.level.value.toString()],
        "alertCategory": this.filterVehicleForm.controls.category.value,
        "healthStatus": this.filterVehicleForm.controls.status.value,
        "otherFilter": [this.filterVehicleForm.controls.otherFilter.value.toString()],
        "driverId": ["all"],
        "days": 0,
        "languagecode":"cs-CZ"
      }
    }
    let vehicleGroupSel = this.groupList.filter((elem)=> elem.vehicleId === this.filterVehicleForm.get("group").value);
    this.reportService.getFleetOverviewDetails(this.objData).subscribe((data:any) => {
      let val = [{vehicleGroup : vehicleGroupSel.vehicleGroupName, data : data}];
      this.messageService.sendMessage(val);
      this.messageService.sendMessage("refreshTimer");
      this.drawIcons(data);
      data.forEach(item => {
        this.filterData["healthStatus"].forEach(e => {
         if(item.vehicleHealthStatusType==e.value)
         {         
          item.vehicleHealthStatusType = this.translationData[e.name];
         }
        });
        this.filterData["otherFilter"].forEach(element => {
          if(item.vehicleDrivingStatusType==element.value)
          {         
           item.vehicleDrivingStatusType = this.translationData[element.name];
          }
         });         
         if(this.categoryList.length>0){
         item.fleetOverviewAlert.forEach(e => {
         let alertCategory = this.categoryList.filter((ele)=> ele.value == e.categoryType);
         if(alertCategory.length>0){
         newAlertCat.push(alertCategory[0]);
         }          
        });  
       }
      });    
     this.categoryList = this.removeDuplicates(newAlertCat, "value");
     console.log(newAlertCat);    
      this.vehicleListData = data;     
      let _dataObj ={
        vehicleDetailsFlag : this.isVehicleDetails,
        data:data
      }
      this.dataInterchangeService.getVehicleData(_dataObj);//change as per filter data
          
    }, (error) => {
      let val = [{vehicleGroup : vehicleGroupSel.vehicleGroupName, data : error}];
      this.messageService.sendMessage(val);
      this.messageService.sendMessage("refreshTimer");
      if (error.status == 404) {
        this.noRecordFlag = true;
        let _dataObj ={
          vehicleDetailsFlag : this.isVehicleDetails,
          data:null
        }
        this.dataInterchangeService.getVehicleData(_dataObj);
      }
    });
    this.noRecordFlag = false;
 } 

  checkToHideFilter(item:any){
    this.isVehicleDetails  = item.vehicleDetailsFlag;
  }

 checkCreationForVehicle(item: any){
  this.todayFlagClicked = item.todayFlagClicked;
  // this.isVehicleDetails  = item.vehicleDetailsFlag;
  // this.driverFlagClicked = true;
  this.getFilterData();
  // this.loadDriverData();
  this.loadVehicleData();
}

checkCreationForDriver(item:any){
  this.driverFlagClicked = item.driverFlagClicked;
  // this.todayFlagClicked  = true;
  this.getDriverData();
  // this.loadVehicleData();
  this.loadDriverData();
}


drawIcons(_selectedRoutes){
  _selectedRoutes.forEach(elem => {
   
    let _vehicleMarkerDetails = this.setIconsOnMap(elem);
    let _vehicleMarker = _vehicleMarkerDetails['icon'];
    let _alertConfig = _vehicleMarkerDetails['alertConfig'];
    let _type = 'No Warning';
    if(_alertConfig){
      _type = _alertConfig.type;
    }
    let markerSize = { w: 40, h: 49 };
    let _healthStatus = '',_drivingStatus = '';
    switch (elem.vehicleHealthStatusType) {
      case 'T': // stop now;
        _healthStatus = 'Stop Now';
        break;
      case 'V': // service now;
        _healthStatus = 'Service Now';
        break;
      case 'N': // no action;
        _healthStatus = 'No Action';
        break
      default:
        break;
    }
    switch (elem.vehicleDrivingStatusType) {
      case 'N': 
        _drivingStatus = 'Never Moved';
        break;
      case 'D':
        _drivingStatus = 'Driving';
        break;
      case 'I': // no action;
        _drivingStatus = 'Idle';
        break;
      case 'U': // no action;
        _drivingStatus = 'Unknown';
        break;
      case 'S': // no action;
        _drivingStatus = 'Stopped';
        break
      
      default:
        break;
    }

    this.svgIcon = this.sanitizer.bypassSecurityTrustHtml(_vehicleMarkerDetails.icon);
    elem =  Object.defineProperty(elem, "icon", {value : this.svgIcon,
    writable : true,enumerable : true, configurable : true});  

  });
  
    
 }

setIconsOnMap(element) {
  let _drivingStatus = false;
  let healthColor = '#606060';
  let _alertConfig = undefined;
  if (element.vehicleDrivingStatusType === 'D') {
    _drivingStatus = true
  }
  switch (element.vehicleHealthStatusType) {
    case 'T': // stop now;
      healthColor = '#D50017'; //red
      break;
    case 'V': // service now;
      healthColor = '#FC5F01'; //orange
      break;
    case 'N': // no action;
      healthColor = '#606060'; //grey
      if (_drivingStatus) {
        healthColor = '#00AE10'; //green
      }
      break
    default:
      break;
  }
  let _vehicleIcon : any; 
  if(_drivingStatus){

  }
  else{
    let _alertFound = undefined ;    
    // if(element.fleetOverviewAlert.length > 0){
    //   _alertFound = element.fleetOverviewAlert.find(item=>item.latitude == element.latestReceivedPositionLattitude && item.longitude == element.latestReceivedPositionLongitude)

    // } 
    if(element.fleetOverviewAlert.length > 0){      
      let critical  = element.fleetOverviewAlert.filter(lvl=> lvl.level == 'C');
      let warning   = element.fleetOverviewAlert.filter(lvl=> lvl.level == 'W');
      let advisory   = element.fleetOverviewAlert.filter(lvl=> lvl.level == 'A');
     
      if(critical.length > 0){
      _alertFound = critical[0];
     } 
     else if(warning.length > 0){
      _alertFound = warning[0];
     } else{
      _alertFound = advisory[0];
     }
     let alertName ='';
     if(_alertFound.level == 'C')
     {
      alertName = 'Critical';
     }
     else if(_alertFound.level == 'W')
     {
      alertName = 'Warning';
     }
     else{
      alertName = 'Advisory';
     }
     element =  Object.defineProperty(element, "alertName", {value : alertName,
      writable : true,enumerable : true, configurable : true}); 
    }
       
    if(_alertFound){
      _alertConfig = this.getAlertConfig(_alertFound);     
      _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M32.5 24.75C32.5 37 16.75 47.5 16.75 47.5C16.75 47.5 1 37 1 24.75C1 20.5728 2.65937 16.5668 5.61307 13.6131C8.56677 10.6594 12.5728 9 16.75 9C20.9272 9 24.9332 10.6594 27.8869 13.6131C30.8406 16.5668 32.5 20.5728 32.5 24.75Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 46.625C24.1875 40.5 31.625 32.9652 31.625 24.75C31.625 16.5348 24.9652 9.875 16.75 9.875C8.53477 9.875 1.875 16.5348 1.875 24.75C1.875 32.9652 9.75 40.9375 16.75 46.625Z" fill="#D50017"/>
      <path d="M16.75 37.4375C23.9987 37.4375 29.875 31.8551 29.875 24.9688C29.875 18.0824 23.9987 12.5 16.75 12.5C9.50126 12.5 3.625 18.0824 3.625 24.9688C3.625 31.8551 9.50126 37.4375 16.75 37.4375Z" fill="white"/>
      <g clip-path="url(#clip0)">
      <path d="M11.7041 30.1148C10.8917 30.1148 10.2307 29.4539 10.2307 28.6415C10.2307 27.8291 10.8917 27.1682 11.7041 27.1682C12.5164 27.1682 13.1773 27.8291 13.1773 28.6415C13.1773 29.4539 12.5164 30.1148 11.7041 30.1148ZM11.7041 27.974C11.3359 27.974 11.0365 28.2735 11.0365 28.6416C11.0365 29.0096 11.3359 29.3091 11.7041 29.3091C12.0721 29.3091 12.3715 29.0096 12.3715 28.6416C12.3715 28.2735 12.0721 27.974 11.7041 27.974Z" fill="#D50017"/>
      <path d="M21.7961 30.1148C20.9838 30.1148 20.3228 29.4539 20.3228 28.6415C20.3228 27.8291 20.9838 27.1682 21.7961 27.1682C22.6085 27.1682 23.2694 27.8291 23.2694 28.6415C23.2694 29.4539 22.6085 30.1148 21.7961 30.1148ZM21.7961 27.974C21.4281 27.974 21.1285 28.2735 21.1285 28.6416C21.1285 29.0096 21.4281 29.3091 21.7961 29.3091C22.1642 29.3091 22.4637 29.0096 22.4637 28.6416C22.4637 28.2735 22.1642 27.974 21.7961 27.974Z" fill="#D50017"/>
      <path d="M18.819 18.5846H14.6812C14.4587 18.5846 14.2783 18.4043 14.2783 18.1817C14.2783 17.9592 14.4587 17.7788 14.6812 17.7788H18.819C19.0415 17.7788 19.2219 17.9592 19.2219 18.1817C19.2219 18.4042 19.0415 18.5846 18.819 18.5846Z" fill="#D50017"/>
      <path d="M19.6206 30.2772H13.8795C13.6569 30.2772 13.4766 30.0969 13.4766 29.8743C13.4766 29.6518 13.6569 29.4714 13.8795 29.4714H19.6206C19.8431 29.4714 20.0235 29.6518 20.0235 29.8743C20.0235 30.0968 19.8431 30.2772 19.6206 30.2772Z" fill="#D50017"/>
      <path d="M19.6206 27.8119H13.8795C13.6569 27.8119 13.4766 27.6315 13.4766 27.409C13.4766 27.1864 13.6569 27.0061 13.8795 27.0061H19.6206C19.8431 27.0061 20.0235 27.1864 20.0235 27.409C20.0235 27.6315 19.8431 27.8119 19.6206 27.8119Z" fill="#D50017"/>
      <path d="M19.6206 29.0445H13.8795C13.6569 29.0445 13.4766 28.8642 13.4766 28.6417C13.4766 28.4191 13.6569 28.2388 13.8795 28.2388H19.6206C19.8431 28.2388 20.0235 28.4191 20.0235 28.6417C20.0235 28.8642 19.8431 29.0445 19.6206 29.0445Z" fill="#D50017"/>
      <path d="M25.5346 22.0678H23.552C23.2742 22.0678 23.0491 22.2929 23.0491 22.5707V23.6681L22.7635 23.9697V18.1753C22.7635 17.2023 21.9722 16.411 20.9993 16.411H12.5009C11.528 16.411 10.7365 17.2023 10.7365 18.1753V23.9696L10.451 23.6681V22.5707C10.451 22.2929 10.2259 22.0678 9.94814 22.0678H7.96539C7.68767 22.0678 7.4625 22.2929 7.4625 22.5707V23.8683C7.4625 24.1461 7.68767 24.3712 7.96539 24.3712H9.73176L10.1695 24.8335C9.49853 25.0833 9.01905 25.73 9.01905 26.4873V31.7339C9.01905 32.0117 9.24416 32.2368 9.52194 32.2368H10.1291V33.4026C10.1291 34.1947 10.7734 34.839 11.5655 34.839C12.3575 34.839 13.0018 34.1947 13.0018 33.4026V32.2368H20.4981V33.4026C20.4981 34.1947 21.1424 34.839 21.9345 34.839C22.7266 34.839 23.3709 34.1947 23.3709 33.4026V32.2368H23.9781C24.2558 32.2368 24.481 32.0117 24.481 31.7339V26.4873C24.481 25.73 24.0015 25.0834 23.3306 24.8336L23.7683 24.3712H25.5346C25.8124 24.3712 26.0375 24.1461 26.0375 23.8683V22.5707C26.0375 22.2929 25.8123 22.0678 25.5346 22.0678ZM9.4452 23.3655H8.46828V23.0736H9.4452V23.3655ZM11.7422 18.1753C11.7422 17.7571 12.0826 17.4168 12.5009 17.4168H20.9992C21.4173 17.4168 21.7576 17.7571 21.7576 18.1753V18.9469H11.7422V18.1753ZM21.7577 19.9526V24.723H17.2529V19.9526H21.7577ZM11.7422 19.9526H16.2471V24.723H11.7422V19.9526ZM11.996 33.4025C11.996 33.6399 11.8027 33.8331 11.5655 33.8331C11.3281 33.8331 11.1349 33.6399 11.1349 33.4025V32.2368H11.996V33.4025ZM22.3651 33.4025C22.3651 33.6399 22.1718 33.8331 21.9345 33.8331C21.6972 33.8331 21.5039 33.6399 21.5039 33.4025V32.2368H22.3651V33.4025ZM23.4752 26.4873V31.231H10.0248V26.4873C10.0248 26.0692 10.3652 25.7288 10.7834 25.7288H22.7166C23.1348 25.7288 23.4752 26.0692 23.4752 26.4873ZM25.0317 23.3655H24.0549V23.0736H25.0317V23.3655Z" fill="#D50017" stroke="#D50017" stroke-width="0.2"/>
      </g>
      <mask id="path-11-outside-1" maskUnits="userSpaceOnUse" x="17.6667" y="0.666748" width="23" height="19" fill="black">
      <rect fill="white" x="17.6667" y="0.666748" width="23" height="19"/>
      <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z"/>
      </mask>
      <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z" fill="${_alertConfig.color}"/>
      <path d="M29.0001 4.66675L30.7309 3.66468L29.0001 0.675021L27.2692 3.66468L29.0001 4.66675ZM21.6667 17.3334L19.9359 16.3313L18.1979 19.3334H21.6667V17.3334ZM36.3334 17.3334V19.3334H39.8023L38.0643 16.3313L36.3334 17.3334ZM27.2692 3.66468L19.9359 16.3313L23.3976 18.3355L30.7309 5.66882L27.2692 3.66468ZM21.6667 19.3334H36.3334V15.3334H21.6667V19.3334ZM38.0643 16.3313L30.7309 3.66468L27.2692 5.66882L34.6026 18.3355L38.0643 16.3313Z" fill="white" mask="url(#path-11-outside-1)"/>
      <path d="M29.6666 14H28.3333V15.3333H29.6666V14Z" fill="white"/>
      <path d="M29.6666 10H28.3333V12.6667H29.6666V10Z" fill="white"/>
      <defs>
      <clipPath id="clip0">
      <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 16.4375)"/>
      </clipPath>
      </defs>
      </svg>`;
    }
    else{
      _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M32.5 17.5C32.5 29.75 16.75 40.25 16.75 40.25C16.75 40.25 1 29.75 1 17.5C1 13.3228 2.65937 9.31677 5.61307 6.36307C8.56677 3.40937 12.5728 1.75 16.75 1.75C20.9272 1.75 24.9332 3.40937 27.8869 6.36307C30.8406 9.31677 32.5 13.3228 32.5 17.5Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 39.375C24.1875 33.25 31.625 25.7152 31.625 17.5C31.625 9.28475 24.9652 2.62498 16.75 2.62498C8.53477 2.62498 1.875 9.28475 1.875 17.5C1.875 25.7152 9.75 33.6875 16.75 39.375Z" fill="${healthColor}"/>
      <path d="M16.75 30.1875C23.9987 30.1875 29.875 24.605 29.875 17.7187C29.875 10.8324 23.9987 5.24998 16.75 5.24998C9.50126 5.24998 3.625 10.8324 3.625 17.7187C3.625 24.605 9.50126 30.1875 16.75 30.1875Z" fill="white"/>
      <g clip-path="url(#clip0)">
      <path d="M11.7041 22.8649C10.8917 22.8649 10.2307 22.2039 10.2307 21.3916C10.2307 20.5792 10.8917 19.9183 11.7041 19.9183C12.5164 19.9183 13.1773 20.5792 13.1773 21.3916C13.1773 22.204 12.5164 22.8649 11.7041 22.8649ZM11.7041 20.7241C11.3359 20.7241 11.0365 21.0235 11.0365 21.3916C11.0365 21.7597 11.3359 22.0591 11.7041 22.0591C12.0721 22.0591 12.3715 21.7597 12.3715 21.3916C12.3715 21.0235 12.0721 20.7241 11.7041 20.7241Z" fill="${healthColor}"/>
      <path d="M21.7961 22.8649C20.9838 22.8649 20.3228 22.2039 20.3228 21.3916C20.3228 20.5792 20.9838 19.9183 21.7961 19.9183C22.6085 19.9183 23.2694 20.5792 23.2694 21.3916C23.2694 22.204 22.6085 22.8649 21.7961 22.8649ZM21.7961 20.7241C21.4281 20.7241 21.1285 21.0235 21.1285 21.3916C21.1285 21.7597 21.4281 22.0591 21.7961 22.0591C22.1642 22.0591 22.4637 21.7597 22.4637 21.3916C22.4637 21.0235 22.1642 20.7241 21.7961 20.7241Z" fill="${healthColor}"/>
      <path d="M18.819 11.3345H14.6812C14.4587 11.3345 14.2783 11.1542 14.2783 10.9317C14.2783 10.7092 14.4587 10.5288 14.6812 10.5288H18.819C19.0415 10.5288 19.2219 10.7092 19.2219 10.9317C19.2219 11.1542 19.0415 11.3345 18.819 11.3345Z" fill="${healthColor}"/>
      <path d="M19.6206 23.0272H13.8795C13.6569 23.0272 13.4766 22.8468 13.4766 22.6243C13.4766 22.4018 13.6569 22.2214 13.8795 22.2214H19.6206C19.8431 22.2214 20.0235 22.4018 20.0235 22.6243C20.0235 22.8468 19.8431 23.0272 19.6206 23.0272Z" fill="${healthColor}"/>
      <path d="M19.6206 20.5619H13.8795C13.6569 20.5619 13.4766 20.3815 13.4766 20.159C13.4766 19.9364 13.6569 19.7561 13.8795 19.7561H19.6206C19.8431 19.7561 20.0235 19.9364 20.0235 20.159C20.0235 20.3815 19.8431 20.5619 19.6206 20.5619Z" fill="${healthColor}"/>
      <path d="M19.6206 21.7945H13.8795C13.6569 21.7945 13.4766 21.6142 13.4766 21.3916C13.4766 21.1691 13.6569 20.9887 13.8795 20.9887H19.6206C19.8431 20.9887 20.0235 21.1691 20.0235 21.3916C20.0235 21.6142 19.8431 21.7945 19.6206 21.7945Z" fill="${healthColor}"/>
      <path d="M25.5346 14.8178H23.552C23.2742 14.8178 23.0491 15.0429 23.0491 15.3207V16.4181L22.7635 16.7197V10.9253C22.7635 9.95231 21.9722 9.16096 20.9993 9.16096H12.5009C11.528 9.16096 10.7365 9.9523 10.7365 10.9253V16.7196L10.451 16.4181V15.3207C10.451 15.0429 10.2259 14.8178 9.94814 14.8178H7.96539C7.68767 14.8178 7.4625 15.0429 7.4625 15.3207V16.6183C7.4625 16.8961 7.68767 17.1212 7.96539 17.1212H9.73176L10.1695 17.5835C9.49853 17.8333 9.01905 18.48 9.01905 19.2373V24.4839C9.01905 24.7617 9.24416 24.9868 9.52194 24.9868H10.1291V26.1526C10.1291 26.9447 10.7734 27.5889 11.5655 27.5889C12.3575 27.5889 13.0018 26.9447 13.0018 26.1526V24.9868H20.4981V26.1526C20.4981 26.9447 21.1424 27.5889 21.9345 27.5889C22.7266 27.5889 23.3709 26.9447 23.3709 26.1526V24.9868H23.9781C24.2558 24.9868 24.481 24.7617 24.481 24.4839V19.2373C24.481 18.48 24.0015 17.8333 23.3306 17.5835L23.7683 17.1212H25.5346C25.8124 17.1212 26.0375 16.8961 26.0375 16.6183V15.3207C26.0375 15.0429 25.8123 14.8178 25.5346 14.8178ZM9.4452 16.1154H8.46828V15.8236H9.4452V16.1154ZM11.7422 10.9253C11.7422 10.5071 12.0826 10.1667 12.5009 10.1667H20.9992C21.4173 10.1667 21.7576 10.5071 21.7576 10.9253V11.6969H11.7422V10.9253ZM21.7577 12.7026V17.4729H17.2529V12.7026H21.7577ZM11.7422 12.7026H16.2471V17.4729H11.7422V12.7026ZM11.996 26.1525C11.996 26.3898 11.8027 26.5831 11.5655 26.5831C11.3281 26.5831 11.1349 26.3898 11.1349 26.1525V24.9867H11.996V26.1525ZM22.3651 26.1525C22.3651 26.3898 22.1718 26.5831 21.9345 26.5831C21.6972 26.5831 21.5039 26.3898 21.5039 26.1525V24.9867H22.3651V26.1525ZM23.4752 19.2373V23.981H10.0248V19.2373C10.0248 18.8191 10.3652 18.4788 10.7834 18.4788H22.7166C23.1348 18.4788 23.4752 18.8191 23.4752 19.2373ZM25.0317 16.1154H24.0549V15.8236H25.0317V16.1154Z" fill="${healthColor}" stroke="${healthColor}" stroke-width="0.2"/>
      </g>
      <defs>
      <clipPath id="clip0">
      <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 9.18748)"/>
      </clipPath>
      </defs>
      </svg>`
    }
  
  }
  return {icon: _vehicleIcon,alertConfig:_alertConfig};
}

getAlertConfig(_currentAlert){
  let _alertConfig = {color : '#D50017' , level :'Critical', type : ''};
  let _fillColor = '#D50017';
  let _level = 'Critical';
  let _type = '';
    switch (_currentAlert.level) {
      case 'C':{
        _fillColor = '#D50017';
        _level = 'Critical'
      }
      break;
      case 'W':{
        _fillColor = '#FC5F01';
        _level = 'Warning'
      }
      break;
      case 'A':{
        _fillColor = '#FFD80D';
        _level = 'Advisory'
      }
      break;
      default:
        break;
    }
    switch (_currentAlert.categoryType) {
      case 'L':{
        _type = 'Logistics Alerts'
      }
      break;
      case 'F':{
        _type='Fuel and Driver Performance'
      }
      break;
      case 'R':{
        _type='Repair and Maintenance'

      }
      break;
      default:
        break;
    }
    return {color : _fillColor , level : _level, type : _type};
}

}
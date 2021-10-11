import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { ReportService } from '../services/report.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReplaySubject } from 'rxjs';
import { FormControl } from '@angular/forms';
@Component({
  selector: 'app-vehicle-updates',
  templateUrl: './vehicle-updates.component.html',
  styleUrls: ['./vehicle-updates.component.less']
})
export class VehicleUpdatesComponent implements OnInit {
  displayedColumns: string[] = ['vehicleName','registrationNo','vehicleGroupNames','modelYear','type','softwareStatus','action'];
  grpTitleVisible : boolean = false;
  errorMsgVisible: boolean = false;
  displayMessage: any;

  translationData: any= {};
  localStLanguage: any;
  dataSource: any; 
  initData: any = [];
  accountOrganizationId: any;
  accountId: any;
  accountRoleId: any;
  showLoadingIndicator: any = false;
  showVehicalDetails: boolean = false;
  vehicleGroup: any = [];
  vehicleName: any = [];
  vehicleListArrany =[];
  vehicleFilterList:any=[];
  vehicleUpdatesForm: FormGroup;
  vehicleSoftwareStatus:any=[];
  vehicleGroupArr:any=[]; 
  vehicleNameArr:any=[];
  filterListValues = {};
  searchFilter= new FormControl();
  filteredValues = {
    search: ''
  };
  ngVehicleName = ''; 
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  public filteredSoftwareStatus: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  constructor(private translationService: TranslationService, private reportService:ReportService, private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 45 //-- for vehicle updates
    }
    this.vehicleUpdatesForm = this._formBuilder.group({
      vehicleGroup: ['', []],
      vehicle: ['', []],
      softStatus: ['', []]  
    });

    let vehicleSoftStatusArr= [
      {
        "id": 79,
        "type": "S",
        "enum": "F",
        "parentEnum": "",
        "key": "enumvehiclesoftwarestatus_updatefailed",
        "featureId": 0
      },
      {
        "id": 80,
        "type": "S",
        "enum": "A",
        "parentEnum": "",
        "key": "enumvehiclesoftwarestatus_updateavailable",
        "featureId": 0
      },
      {
        "id": 81,
        "type": "S",
        "enum": "R",
        "parentEnum": "",
        "key": "enumvehiclesoftwarestatus_updaterunning",
        "featureId": 0
      },
      {
        "id": 82,
        "type": "S",
        "enum": "U",
        "parentEnum": "",
        "key": "enumvehiclesoftwarestatus_updateuptodate",
        "featureId": 0
      }
    ]       
     this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data); 
      if(this.translationData != undefined){
        vehicleSoftStatusArr.forEach(element => {      
            element["value"]= this.translationData[element["key"]];
            this.vehicleSoftwareStatus.push(element);
        });
      }      
        this.resetSoftStatusFilter();       
        this.loadVehicleStatusData(); 
        this.searchAllDataFilter();       
    });  
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  } 
 
 searchAllDataFilter(){
  this.dataSource.filterPredicate = this.createFilter();  
  this.searchFilter.valueChanges.subscribe(filterValue => {
  this.filteredValues['search'] = filterValue.trim().toLowerCase(); 
  this.dataSource.filter = JSON.stringify(this.filteredValues);  
  this.vehicleUpdatesForm.get('vehicle').setValue("all");
  this.vehicleUpdatesForm.get('vehicleGroup').setValue("all"); 
  this.vehicleUpdatesForm.get('softStatus').setValue("all");   
  }); 
}

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
     });
  }
  
  loadVehicleStatusData(){
    let vehicleStatusList:any  = [
      {
      vin: "XPsa43434343",
      vehicleName: "test 73.0",
      registrationNo: "MH 213213",
      vehicleGroupNames: "DefaultVehicleGroup",
      modelYear: "2011",
      type: "XF",
      softwareStatus:"Update Failed",
      isAdminRight:true
      },
      {
        vin: "XPsa43434343",
        vehicleName: "ACB 31",
        registrationNo: "MH 213213",
        vehicleGroupNames: "group1, group2",
        modelYear: "2012",
        type: "LG",
        softwareStatus:"Update Available",
        isAdminRight:false
        }, {
          vin: "XPsa43434343",
          vehicleName: "demo",
          registrationNo: "MH 213213",
          vehicleGroupNames: "DefaultVehicleGroup, Fleet",
          modelYear: "2013",
          type: "XG",
          softwareStatus:"Update Running",
          isAdminRight:true
          },
          {
            vin: "XPsa43434343",
            vehicleName: "ACB 31",
            registrationNo: "MH 213213",
            vehicleGroupNames: "group2",
            modelYear: "2014",
            type: "XF",
            softwareStatus:"Up-To Date",
            isAdminRight:false
            }, {
              vin: "XPsa43434343",
              vehicleName: "ACB 31",
              registrationNo: "MH 213213",
              vehicleGroupNames: "group2",
              modelYear: "2015",
              type: "XF",
              softwareStatus:"Update Available",
              isAdminRight:true
              },
              {
                vin: "XPsa43434343",
                vehicleName: "test 73.0",
                registrationNo: "MH 213213",
                vehicleGroupNames: "Fleet",
                modelYear: "2016",
                type: "XF",
                softwareStatus:"Update Failed",
                isAdminRight:false
                }
      ] ;
       
    //   this.vehicleUpdatesService.getVeicleStatusListData().subscribe((data) => {
    //   let filterData = data["enumTranslation"];
    //   filterData.forEach(element => {
    //     element["value"]= this.translationData[element["key"]];
    //   });    
    // }, (error) => {

    // })
  
    vehicleStatusList.filter((element) =>{
    this.vehicleGroupArr.push(element.vehicleGroupNames);    
    this.vehicleNameArr.push({'vehicleName': element.vehicleName.trim(),'vehicleGroup': element.vehicleGroupNames.trim()});    
    });
  
    let vehGrp:any = [];
    this.vehicleGroupArr.forEach(element => {
     let vehGrpTemp = element.split(',');    
     vehGrpTemp.forEach((ele:any )=> {
       vehGrp.push({'vehicleGroup': ele.trim()});
     })
    });
    this.vehicleGroup = this.removeDuplicates(vehGrp, "vehicleGroup");
    this.vehicleName = this.removeDuplicates(this.vehicleNameArr, "vehicleName");
    
    this.initData= vehicleStatusList;
    this.showLoadingIndicator = false;      
    this.updateDataSource(this.initData); 
    
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

onVehicleGroupChange(filter, event){
   this.vehicleName=[];
   this.ngVehicleName='all'
   let event_val;    
  
   if(event == 'all'){
    this.vehicleName =  this.removeDuplicates(this.vehicleNameArr, "vehicleName");
    event_val = '';  
  }
  else{
    let vehicle_group_selected = event.vehicleGroup;
    let vehicle= this.vehicleNameArr.filter(item => item.vehicleGroup.includes(vehicle_group_selected+","));
    console.log('vehicle',vehicle);
    this.vehicleNameArr.forEach(element => {
    if(element.vehicleGroup.includes(vehicle_group_selected)){
      this.vehicleName.push(element);
    }
    });
    this.vehicleName = this.removeDuplicates(this.vehicleName, "vehicleName");    
    event_val = event.vehicleGroup.trim(); 
  }  
    this.filterListValues['vehicleName']='';
    this.filterListValues[filter] =event_val;
    
    this.dataSource.filter = JSON.stringify(this.filterListValues);  
}


  onViewVehicleList(row:any, type:any){
   this.getVehicleUpdateDetails();
  }
filterVehicleSoft(softStatus) { 
  if (!this.vehicleSoftwareStatus) {
    return;
  }
  if (!softStatus) {
    this.resetSoftStatusFilter();
    return;
  } else {
    softStatus = softStatus.toLowerCase();
  }
  this.filteredSoftwareStatus.next(
    this.vehicleSoftwareStatus.filter(item => item.value.toLowerCase().indexOf(softStatus) > -1)
  );  
 }

 resetSoftStatusFilter() {
  this.filteredSoftwareStatus.next(this.vehicleSoftwareStatus.slice());
}

  onClose(){
    this.grpTitleVisible = false;
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
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

  
  getVehicleUpdateDetails(){
    // this.showLoadingIndicator = true;
    this.showVehicalDetails = true;
    alert('component loaded');
    // let accountStatus: any = this.isViewListDisabled ? true : false; 
    // this.accountService.getAccessRelationshipDetails(this.accountOrganizationId, accountStatus).subscribe((data: any) => {
    //   this.hideloader();
    //   this.accountGrpAccountDetails = data.account;
    //   this.vehicleGrpVehicleDetails = data.vehicle;
    //   this.associationTypeId = this.isViewListDisabled ? 2 : 1; // 1-> vehicle 2-> account
    //   this.createVehicleAccountAccessRelation = true;
    // }, (error) => {
    //   this.hideloader();
    //   console.log("error:: ", error)
    // });
  }
  

  onVehicleChange(filter, event) {     
    let event_val;      
       if(filter == "vehicleName" || filter == "softwareStatus"){
          if(event.value == 'all'){          
            event_val = '';  
          }else
          {
            event_val = event.value.trim();  
          }
       }   
       else{
        event_val = '';  
       }
       this.filterListValues[filter] =event_val;
      this.dataSource.filter = JSON.stringify(this.filterListValues);
       
      // this.filterValues.emit(this.dataSource); 
  }
  createFilter() {
    let filterFunction = function (data: any, filter: string): boolean {
      let searchTerms = JSON.parse(filter);
      let isFilterSet = false;
      for (const col in searchTerms) {
        if (searchTerms[col].toString() !== '') {
          isFilterSet = true;
        } else {  
          delete searchTerms[col];
        }
      }
      let nameSearch = () => {
        let found = false;
        if (isFilterSet) {          
          if(searchTerms.vehicleName){
            let vehName = '';
            vehName = data.vehicleName;
            if(vehName.includes(searchTerms.vehicleName)){
              found = true;    
            }     
          else{
            return false;
          }
        }
        if(searchTerms.vehicleGroupNames){          
          let vehGrpName = '';
          vehGrpName = data.vehicleGroupNames;
            if(vehGrpName.includes(searchTerms.vehicleGroupNames)){
              found = true;    
          }
        else{
          return false;
        }
      }
        if(searchTerms.softwareStatus){          
          let softStatus = '';
          softStatus = data.softwareStatus;
            if(softStatus.includes(searchTerms.softwareStatus)){
              found = true;    
          }
        else{
          return false;
        }
      }
      if(searchTerms.search){  
           
        let searchData = '';
        searchData = data;
          if(searchData["softwareStatus"].toLowerCase().includes(searchTerms.search)){
            found = true;    
        }else if(searchData["vehicleGroupNames"].toLowerCase().includes(searchTerms.search)){
          found = true;    
            }else if(searchData["vehicleName"].toLowerCase().includes(searchTerms.search)){
              found = true;    
          }
          else if(searchData["registrationNo"].toLowerCase().includes(searchTerms.search)){
            found = true;    
        }
        else if(searchData["modelYear"].toLowerCase().includes(searchTerms.search)){
          found = true;    
      }
      else if(searchData["type"].toLowerCase().includes(searchTerms.search)){
        found = true;    
      }
      else{
        return false;
      }
       
    }
      return found
        } else {
          return true;
        }
      }
      return nameSearch()
    }
    return filterFunction
  }
}

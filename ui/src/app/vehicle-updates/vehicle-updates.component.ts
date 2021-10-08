import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
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
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  constructor(private translationService: TranslationService) { }

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
      menuId: 17 //-- for alerts
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);      
      this.loadFiltersData();  
    }); 
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadFiltersData(){   
    let vehicleStatusList:any  = [
      {
      vin: "XPsa43434343",
      vehicleName: "ACB 31",
      registrationNo: "MH 213213",
      vehicleGroupNames: "group1",
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
        softwareStatus:"Update Avaiable",
        isAdminRight:false
        }, {
          vin: "XPsa43434343",
          vehicleName: "ACB 31",
          registrationNo: "MH 213213",
          vehicleGroupNames: "group1, group2, group3, group4",
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
            softwareStatus:"UP-To-Date",
            isAdminRight:false
            }, {
              vin: "XPsa43434343",
              vehicleName: "ACB 31",
              registrationNo: "MH 213213",
              vehicleGroupNames: "group2",
              modelYear: "2015",
              type: "XF",
              softwareStatus:"Update Avaiable",
              isAdminRight:true
              },
              {
                vin: "XPsa43434343",
                vehicleName: "ACB 31",
                registrationNo: "MH 213213",
                vehicleGroupNames: "group2",
                modelYear: "2016",
                type: "XF",
                softwareStatus:"Update Failed",
                isAdminRight:false
                }
      ] ;
       
    //  this.alertService.getAlertFilterData(this.accountId, this.accountOrganizationId).subscribe((data) => {
    //   let filterData = data["enumTranslation"];
    //   filterData.forEach(element => {
    //     element["value"]= this.translationData[element["key"]];
    //   });    
    // }, (error) => {

    // })
    this.initData= vehicleStatusList;
    this.showLoadingIndicator = false;  
    this.updateDataSource(this.initData);  
  }
  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      // this.dataSource.sortData = (data: String[], sort: MatSort) => {
      //   const isAsc = sort.direction === 'asc';
      //   return data.sort((a: any, b: any) => {
      //     return this.compare(a[sort.active], b[sort.active], isAsc);
      //   });
      //  }
    });
  }

  // compare(a: Number | String, b: Number | String, isAsc: boolean) {
  //   if(!(a instanceof Number)) a = a.toUpperCase();
  //   if(!(b instanceof Number)) b = b.toUpperCase();
  //   return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  // }
  onViewVehicleList(row:any, type:any){
   this.getVehicleUpdateDetails();
  }
  filterVehicleGroupChange(type:any, event){

  }
  onClose(){
    this.grpTitleVisible = false;
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
  

}

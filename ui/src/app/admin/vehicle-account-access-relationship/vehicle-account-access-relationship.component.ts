import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AccountService } from '../../services/account.service';
import { TranslationService } from '../../services/translation.service';
import { VehicleService } from '../../services/vehicle.service';

@Component({
  selector: 'app-vehicle-account-access-relationship',
  templateUrl: './vehicle-account-access-relationship.component.html',
  styleUrls: ['./vehicle-account-access-relationship.component.less']
})

export class VehicleAccountAccessRelationshipComponent implements OnInit {
  accountGrpList: any = [];
  vehicleGrpList: any = [];
  accessRelationCreatedMsg : any;
  titleVisible: boolean = false;
  translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  selectedViewType: any = '';
  selectedColumnType: any = '';
  createVehicleAccessRelation: boolean = false;
  createAccountAccessRelation: boolean = false;
  displayedColumns: string[] = ['firstName','emailId','roles','accountGroups','action'];
  dataSource: any;
  initData: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  showLoadingIndicator: any;
  isViewListDisabled: boolean = false;
  
  constructor(private translationService: TranslationService, private accountService: AccountService, private vehicleService: VehicleService) { 
    this.defaultTranslation();
  }

  defaultTranslation() {
    this.translationData = {
      lblSearch: "Search",
      lblAllAccessRelationshipDetails: "All Access Relationship Details",
      lblNewAssociation: "New Association"
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 //-- for user mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadAccountData();
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc: any, cur: any) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  createNewAssociation(){
      let accountGrpObj: any = {
        accountGroupId: 0,
        organizationId: this.accountOrganizationId,
        accountId: 0,
        accounts: true,
        accountCount: true,
      }
      let vehicleGrpObj: any = {
        id: 0,
        organizationID: this.accountOrganizationId,
        vehicles: true,
        vehiclesGroup: true,
        groupIds: [0],
      };
      this.accountService.getAccountGroupDetails(accountGrpObj).subscribe((accountGrpList) => {
        this.accountGrpList = accountGrpList;
        this.vehicleService.getVehicleGroup(vehicleGrpObj).subscribe((vehGrpList) => {
          this.vehicleGrpList = vehGrpList;
          if(!this.isViewListDisabled){
            this.createVehicleAccessRelation = true;
          }
          else{
            this.createAccountAccessRelation = true;
          }
        });
      });
  }

  loadAccountData(){
    this.showLoadingIndicator = true;
    let obj: any = {
      accountId: 0,
      organizationId: this.accountOrganizationId,
      accountGroupId: 0,
      vehicleGroupGroupId: 0,
      roleId: 0,
      name: ""
    }
    this.accountService.getAccountDetails(obj).subscribe((usrlist) => {
      this.hideloader();
      this.initData = this.makeRoleAccountGrpList(usrlist);
      this.dataSource = new MatTableDataSource(this.initData);
      setTimeout(()=>{
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
    });
    this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
    this.selectedColumnType = this.selectedColumnType == '' ? 'vehicle' : this.selectedColumnType;
    if(this.selectedColumnType == 'account'){
      this.isViewListDisabled = true;
    }
    else{
      this.isViewListDisabled = false;
    }
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

  editViewAccessRelationship(element: any, type: any) {
    this.createNewAssociation();
  }

  deleteAccessRelationship(element: any){

  }

  onClose(){
    this.titleVisible = false;
  }

  onListChange(event: any){
    this.selectedViewType = event.value;
  }

  onColumnChange(event: any){
    if(event.value == 'account'){
      this.isViewListDisabled = true;
    }
    else{
      this.isViewListDisabled = false;
    }
  }

  checkCreationForVehicle(item: any){
    this.createVehicleAccessRelation = !this.createVehicleAccessRelation;
    if(this.isViewListDisabled){
      this.selectedColumnType = 'account';
    }
    else{
      this.selectedColumnType = 'vehicle';
    }
  }

  checkCreationForAccount(item: any){
    this.createAccountAccessRelation = !this.createAccountAccessRelation;
    if(this.isViewListDisabled){
      this.selectedColumnType = 'account';
    }
    else{
      this.selectedColumnType = 'vehicle';
    }
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

}
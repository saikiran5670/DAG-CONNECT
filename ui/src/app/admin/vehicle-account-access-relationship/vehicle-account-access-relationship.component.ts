import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AccountService } from '../../services/account.service';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-vehicle-account-access-relationship',
  templateUrl: './vehicle-account-access-relationship.component.html',
  styleUrls: ['./vehicle-account-access-relationship.component.less']
})

export class VehicleAccountAccessRelationshipComponent implements OnInit {
  userCreatedMsg : any;
  titleVisible: boolean = false;
  translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  selectedViewType: any = '';
  selectedColumnType: any = '';
  createAccessRelation: boolean = false;
  displayedColumns: string[] = ['firstName','emailId','roles','accountGroups','action'];
  dataSource: any;
  initData: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  showLoadingIndicator: any;
  isViewListDisabled: boolean = false;
  
  constructor(private translationService: TranslationService, private accountService: AccountService) { 
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
    this.createAccessRelation = true;
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

  }

  deleteAccessRelationship(element: any){

  }

  onClose(){
    this.titleVisible = false;
  }

  onListChange(event: any){

  }

  onColumnChange(event: any){
    if(event.value == 'account'){
      this.isViewListDisabled = true;
    }
    else{
      this.isViewListDisabled = false;
    }
  }

  checkCreation(item: any){
    this.createAccessRelation = !this.createAccessRelation;
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

}

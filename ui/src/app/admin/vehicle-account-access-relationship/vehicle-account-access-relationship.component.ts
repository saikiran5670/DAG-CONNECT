import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { AccountService } from '../../services/account.service';
import { TranslationService } from '../../services/translation.service';
import { VehicleService } from '../../services/vehicle.service';

@Component({
  selector: 'app-vehicle-account-access-relationship',
  templateUrl: './vehicle-account-access-relationship.component.html',
  styleUrls: ['./vehicle-account-access-relationship.component.less']
})

export class VehicleAccountAccessRelationshipComponent implements OnInit {
  //-------Rest mock variable--------//
  vehicleGrpVehicleAssociationDetails: any = [];
  accountGrpAccountAssociationDetails: any = [];
  vehicleGrpVehicleDetails: any = [];
  accountGrpAccountDetails: any = [];
  //---------------//
  accessRelationCreatedMsg : any = '';
  titleVisible: boolean = false;
  translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  selectedViewType: any = '';
  selectedColumnType: any = '';
  createVehicleAccessRelation: boolean = false;
  createAccountAccessRelation: boolean = false;
  cols: string[] = ['name','accessType','associatedAccount','action'];
  columnNames: string[] = ['Vehicle Group/Vehicle','Access Type','Account Group/Account','Action'];
  dataSource: any;
  initData: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  showLoadingIndicator: any;
  isViewListDisabled: boolean = false;
  actionType: any = '';
  selectedElementData: any;

  constructor(private translationService: TranslationService, private accountService: AccountService, private vehicleService: VehicleService, private dialogService: ConfirmDialogService) { 
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
      this.restMockdata();
      this.selectedViewType = this.selectedViewType == '' ? 'both' : this.selectedViewType;
      this.selectedColumnType = this.selectedColumnType == '' ? 'vehicle' : this.selectedColumnType;
      if(this.selectedColumnType == 'account'){
        this.isViewListDisabled = true;
      }
      else{
        this.isViewListDisabled = false;
      }
      this.updateGridData(this.makeAssociatedAccountGrpList(this.vehicleGrpVehicleAssociationDetails));
    });
  }

  restMockdata(){
      this.vehicleGrpVehicleAssociationDetails = [
          {
              "name": "Vehicle Group 1",
              "id": 1,
              "accessType":{
                  "id": 1,
                  "name":"Full Access"
              },
              "associatedAccount":[
                  {
                      "name": "Account Group 1",
                      "id": 1,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account Group 2",
                      "id": 2,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account 1",
                      "id": 3,
                      "isAccountGroup": false
                  },
                  {
                      "name": "Account 2",
                      "id": 4,
                      "isAccountGroup": false
                  }                
              ],
              "isVehicleGroup": true,
              "vehicleCount": 0
          },
          {
              "name": "Vehicle Group 2",
              "id": 2,
              "accessType":{
                  "id": 1,
                  "name":"Full Access"
              },
              "associatedAccount":[
                  {
                      "name": "Account Group 1",
                      "id": 1,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account Group 2",
                      "id": 2,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account 1",
                      "id": 3,
                      "isAccountGroup": false
                  },
                  {
                      "name": "Account 2",
                      "id": 4,
                      "isAccountGroup": false
                  }                
              ],
              "isVehicleGroup": true,
              "vehicleCount": 2
          },
          {
              "name": "Vehicle 1",
              "id": 3,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedAccount":[
                  {
                      "name": "Account Group 1",
                      "id": 1,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account 1",
                      "id": 3,
                      "isAccountGroup": false
                  }                
              ],
              "isVehicleGroup": false
          },
          {
              "name": "Vehicle 2",
              "id": 4,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedAccount":[
                  {
                      "name": "Account Group 2",
                      "id": 2,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account 2",
                      "id": 4,
                      "isAccountGroup": false
                  }                
              ],
              "isVehicleGroup": false
          },
          {
              "name": "Vehicle 3",
              "id": 5,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedAccount":[
                  {
                      "name": "Account Group 2",
                      "id": 2,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account 2",
                      "id": 4,
                      "isAccountGroup": false
                  }                
              ],
              "isVehicleGroup": false
          },
          {
              "name": "Vehicle 4",
              "id": 6,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedAccount":[
                  {
                      "name": "Account Group 2",
                      "id": 2,
                      "isAccountGroup": true
                  },
                  {
                      "name": "Account 2",
                      "id": 4,
                      "isAccountGroup": false
                  }                
              ],
              "isVehicleGroup": false
          }
      ];
      
      this.accountGrpAccountAssociationDetails = [
          {
              "name": "Account Group 1",
              "id": 1,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedVehicle":[
                  {
                      "name": "Vehicle Group 2",
                      "id": 2,
                      "isVehicleGroup": true
                  },
                  {
                      "name": "Vehicle 2",
                      "id": 4,
                      "isVehicleGroup": false
                  }                
              ],
              "isAccountGroup": true,
              "accountCount": 0
          },
          {
              "name": "Account Group 2",
              "id": 2,
              "accessType":{
                  "id": 1,
                  "name":"Full Access"
              },
              "associatedVehicle":[
                  {
                      "name": "Vehicle Group 2",
                      "id": 2,
                      "isVehicleGroup": true
                  },
                  {
                    "name": "Vehicle 1",
                    "id": 3,
                    "isVehicleGroup": false
                  },
                  {
                      "name": "Vehicle 2",
                      "id": 4,
                      "isVehicleGroup": false
                  }                
              ],
              "isAccountGroup": true,
              "accountCount": 2
          },
          {
              "name": "Account 1",
              "id": 3,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedVehicle":[
                  {
                      "name": "Vehicle Group 2",
                      "id": 2,
                      "isVehicleGroup": true
                  },
                  {
                      "name": "Vehicle 2",
                      "id": 4,
                      "isVehicleGroup": false
                  }                
              ],
              "isAccountGroup": false
          },
          {
              "name": "Account 2",
              "id": 4,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedVehicle":[
                  {
                      "name": "Vehicle Group 2",
                      "id": 2,
                      "isVehicleGroup": true
                  },
                  {
                      "name": "Vehicle 2",
                      "id": 4,
                      "isVehicleGroup": false
                  }                
              ],
              "isAccountGroup": false
          },
          {
              "name": "Account 3",
              "id": 5,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedVehicle":[
                  {
                      "name": "Vehicle Group 2",
                      "id": 2,
                      "isVehicleGroup": true
                  },
                  {
                      "name": "Vehicle 2",
                      "id": 4,
                      "isVehicleGroup": false
                  }                
              ],
              "isAccountGroup": false
          },
          {
              "name": "Account 4",
              "id": 6,
              "accessType":{
                  "id": 2,
                  "name":"View Only"
              },
              "associatedVehicle":[
                  {
                      "name": "Vehicle Group 2",
                      "id": 2,
                      "isVehicleGroup": true
                  },
                  {
                      "name": "Vehicle 2",
                      "id": 4,
                      "isVehicleGroup": false
                  }                
              ],
              "isAccountGroup": false
          }
      ];

      this.accountGrpAccountDetails = [
          {
              "name": "Account Group 1",
              "id": 1,
              "isAccountGroup": true
          },
          {
              "name": "Account Group 2",
              "id": 2,
              "isAccountGroup": true
          },
          {
              "name": "Account 1",
              "id": 3,
              "isAccountGroup": false
          },
          {
              "name": "Account 2",
              "id": 4,
              "isAccountGroup": false
          },
          {
              "name": "Account 3",
              "id": 4,
              "isAccountGroup": false
          },
          {
              "name": "Account 4",
              "id": 5,
              "isAccountGroup": false
          }
      ];

      this.vehicleGrpVehicleDetails = [
          {
              "name": "Vehicle Group 1",
              "id": 1,
              "isVehicleGroup": true
          },
          {
              "name": "Vehicle Group 2",
              "id": 2,
              "isVehicleGroup": true
          },
          {
              "name": "Vehicle 1",
              "id": 3,
              "isVehicleGroup": false
          },
          {
              "name": "Vehicle 2",
              "id": 4,
              "isVehicleGroup": false
          },
          {
              "name": "Vehicle 3",
              "id": 5,
              "isVehicleGroup": false
          },
          {
              "name": "Vehicle 4",
              "id": 6,
              "isVehicleGroup": false
          }
      ];
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
    this.actionType = 'create';
    if(!this.isViewListDisabled){
      this.createVehicleAccessRelation = true;
    }
    else{
      this.createAccountAccessRelation = true;
    }
  }

  updateGridData(tableData: any){
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  makeAssociatedAccountGrpList(initdata: any){
    initdata.forEach((element: any, index: any) => {
      let list: any = '';
      element.associatedAccount.forEach((resp: any) => {
        list += resp.name + ', ';
      });
      if(list != ''){
        list = list.slice(0, -2);
      }
      initdata[index].associatedAccountList = list; 
    });
    return initdata;
  }

  makeAssociatedVehicleGrpList(initdata: any){
    initdata.forEach((element: any, index: any) => {
      let list: any = '';
      element.associatedVehicle.forEach((resp: any) => {
        list += resp.name + ', ';
      });
      if(list != ''){
        list = list.slice(0, -2);
      }
      initdata[index].associatedVehicleList = list; 
    });
    return initdata;
  }

  editViewAccessRelationship(element: any, type: any) {
    this.actionType = type;
    this.selectedElementData = element;
    if(!this.isViewListDisabled){
      this.createVehicleAccessRelation = true;
    }
    else{
      this.createAccountAccessRelation = true;
    }
  }

  deleteAccessRelationship(element: any){
    //console.log("delete item:: ", element);
    const options = {
        title: this.translationData.lblDelete || "Delete",
        message: this.translationData.lblAreyousureyouwanttodeleteAssociationRelationship || "Are you sure you want to delete '$' Association Relationship?",
        cancelText: this.translationData.lblNo || "No",
        confirmText: this.translationData.lblYes || "Yes"
      };
    this.dialogService.DeleteModelOpen(options, element.name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
          this.successMsgBlink(this.getDeletMsg(element.name));
        }
    });
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.accessRelationCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }

  getDeletMsg(userName: any){
    if(this.translationData.lblAssociationRelationshipwassuccessfullydeleted)
      return this.translationData.lblAssociationRelationshipwassuccessfullydeleted.replace('$', userName);
    else
      return ("Association Relationship '$' was successfully deleted").replace('$', userName);
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
      this.cols = ['name','accessType','associatedVehicle','action'];
      this.columnNames = ['Account Group/Account','Access Type','Vehicle Group/Vehicle','Action'];
      this.updateGridData(this.makeAssociatedVehicleGrpList(this.accountGrpAccountAssociationDetails));
    }
    else{
      this.isViewListDisabled = false;
      this.cols = ['name','accessType','associatedAccount','action'];
      this.columnNames = ['Vehicle Group/Vehicle','Access Type','Account Group/Account','Action'];
      this.updateGridData(this.makeAssociatedAccountGrpList(this.vehicleGrpVehicleAssociationDetails));
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
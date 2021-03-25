import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { vehicleGrpGetRequest } from 'src/app/models/vehicle.model';
import { TranslationService } from 'src/app/services/translation.service';
import { VehicleService } from 'src/app/services/vehicle.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-vehicle-management',
  templateUrl: './vehicle-management.component.html',
  styleUrls: ['./vehicle-management.component.less']
})
export class VehicleManagementComponent implements OnInit {
  vehicleDisplayedColumns: string[] = ['vehicleGroupID', 'name', 'isActive', 'createdDate', 'action'];
  //veh: vehicleGetRequest;
  vehGrpRqst: vehicleGrpGetRequest;
  vehSelectionFlag: boolean = false;
  mainTableFlag: boolean = true;
  //vehGC: vehGrpCreation = { groupName: null, groupDesc: null };
  cols: string[];
  dataSource: any = new MatTableDataSource([]);
  selectedType: any = '';
  columnNames: string[];
  userCreatedMsg: any = '';
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  inputText: any;
  editFlag: boolean = true;
  viewFlag: boolean = false;
  viewMode: boolean = false;
  viewGroupMode: boolean = false;
  initData: any;
  rowsData: any;
  createStatus: boolean;
  titleText: string;
  translationData: any;
  vehicleData: any;
  bothData: any;
  vehicleGroupData: any;
  vinData: any;
  groupInfo: any;
  orgId: number = 1;
  grpTitleVisible: boolean = false;
  showLoadingIndicator = true;
  localStLanguage: any;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");

  constructor(private vehService: VehicleService, private dialogService: ConfirmDialogService, private _snackBar: MatSnackBar, private translationService: TranslationService) {
    this.defaultTranslation();
  }
  defaultTranslation() {
    this.translationData = {
      lblConfirm: 'Confirm',
      lblFilter: 'Filter',
      lblVehicleGroup: 'Vehicle Group',
      lblVehicle: 'Vehicle',
      lblVIN: 'VIN',
      lblRegistrationNumber: 'Registration Number',
      lblAction: 'Action',
      lblGroup: 'Group',
      lblBoth: 'Both',
      lblSearch: 'Search',
      lblBack: 'Back',
      lblReset: 'Reset',
      lblCancel: 'Cancel',
      lblNo: 'No',
      lblYes: 'Yes',
      lbl120CharMax: '120 characters max',
      lblVehicleName: 'Vehicle Name',
      lblCreate: 'Create',
      lblNew: 'New',
      lblGroupName: 'Group Name',
      lblAll: 'All',
      lblVehicleManagement: 'Vehicle Management',
      lblAllVehicleGroupVehicleDetails: 'All Vehicle Details',
      lblNewVehicleGroup: 'New Vehicle Group',
      lblModel: 'Model',
      lblDetails: 'Details',
      lblEditVehicleGroupDetails: 'Edit Vehicle Group Details',
      lblVehicleModel: 'Vehicle Model',
      lblAssociatedGroups: 'Associated Groups',
      lblSave: 'Save',
      lblVehicleIdentificationNumber: 'Vehicle Identification Number',
      lblNewVehicleGroupCreatedSuccessfully:
        "New Vehicle Group '$' Created Successfully",
      lblDeleteVehicleGroup: 'Delete Vehicle Group',
      lblAreyousureyouwanttodeletevehiclegroup:
        "Are you sure you want to delete '$' vehicle group?",
      lblCreateNewVehicleGroup: 'Create New Vehicle Group',
      lblNewGroupName: 'New Group Name',
      lblGroupDescriptionOptional: 'Group Description Optional',
      lblEnterVehicleGroupName: 'Enter Vehicle Group Name',
      lblEnterVehicleGroupDescription: 'Enter Vehicle Group Description',
      lblSelectVehicleListOptional: 'Select Vehicle List (Optional)',
      lblVehicleHintMessage:
        'Please select vehicles from below list to associate with this vehicle group if needed.',
      lblConsent: 'Status',
      lblVehicleGroupalreadyexistsPleasechooseadifferentname:
        'Vehicle Group already exists. Please choose a different name.',
      lblPleaseenterthenewvehiclegroupname:
        'Please enter the new vehicle group name',
      lblCreateVehicleGroupAPIFailedMessage:
        "Error encountered in creating new Vehicle Group '$'",
      lblEditHintMessage:
        'You can edit vehicle associations from the list below',
      lblVehicleGroupdetailssuccessfullyupdated:
        "Vehicle Group '$' details successfully updated",
      lblUpdateVehicleGroupAPIFailedMessage:
        "Error encountered in updating Vehicle Group '$'",
      lblConsentStatus: 'Consent Status',
      lblVehicleGroupDeleted: "Vehicle Group '$' was successfully deleted",
      lblDeleteVehicleGroupAPIFailedMessage: "Error deleting Vehicle Group '$'",
      lblVehicleConsent: 'Vehicle Consent',
      lblEnteredvehiclenamealreadyexistsPleasechooseadifferentname:
        'Entered vehicle name already exists. Please choose a different name.',
      lblPleaseenterVehicleRegistrationnumberinthecorrectformat:
        'Please enter Vehicle Registration number in the correct format',
      lblVehiclesettingssuccessfullyupdated:
        "Vehicle '$' settings successfully updated",
      lblUpdateVehicleSettingAPIFailedMessage:
        "Error encountered in updating Vehicle Settings for '$'",
      lblVehicleNameisrequired: 'Vehicle Name is required',
      lblRegistrationNumberisrequired: 'Registration Number is required',
    };
  }
  onClose() {
    this.grpTitleVisible = false;
  }
  ngOnInit(): void {
    if (localStorage.getItem('accountOrganizationId') != null) {
      this.orgId = parseInt(localStorage.getItem('accountOrganizationId'));
    }
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.editFlag = false;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: 'Menu',
      name: '',
      value: '',
      filter: '',
      menuId: 21 //-- for vehicle mgnt
    };
    this.translationService
      .getMenuTranslations(translationObj)
      .subscribe((data) => {
        this.processTranslation(data);
        this.loadVehicleData();
      });
  }
  processTranslation(transData: any) {
    this.translationData = transData.reduce(
      (acc, cur) => ({ ...acc, [cur.name]: cur.value }),
      {}
    );
  }
  loadVehicleData() {
    this.vehGrpRqst = {
      id: 0,
      organizationID: this.orgId ? this.orgId : 1,
      vehicles: true,
      vehiclesGroup: true,
      groupIds: [0],
    };
    let tempData = [];
    let tempDataForGroup = [];
    this.dataSource = new MatTableDataSource(tempData);
    //For testing purpose
    this.vehGrpRqst.organizationID=10;
    //-------------------------------
    this.vehService
      .getVehiclesData(this.vehGrpRqst.organizationID)
      //.pipe(map((data) => data.filter((d) => d.isVehicleGroup == true)))
      .subscribe(
        (_data) => {
          if(_data){
            this.hideloader();
          }
          _data.forEach((res)=>{
            // if(res.isVehicleGroup == false){
               tempData.push(res);
            // }
            // else if(res.isVehicleGroup == true){
            //   tempDataForGroup.push(res);
            // }
          })

          //this.vehicleData = tempData;
          this.vehicleGroupData = tempData;
          console.log(tempData)
          this.initData = this.vehicleGroupData;
          this.loadVehicleGroupDataSource();
          //this.selectedType = this.selectedType == '' ? 'group' : this.selectedType;
          // if (this.selectedType === 'group') {
          //   this.loadVehicleGroupDataSource();
          // } else if (this.selectedType === 'vehicle') {
          //   //this.loadVehicleDataSource();
          // } else if (this.selectedType === 'both') {
          //   //this.loadBothDataSource();
          // }
        },
        (error) => {
          console.error(error);
        }
      );
  }
  editFunc(row: any) {}
  viewFunc(row:any){}
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  loadVehicleGroupDataSource() {
    this.cols = ['name', 'vin', 'licensePlateNumber', 'model', 'relationShip','status', 'action'];
    this.columnNames = ['Vehicle', 'VIN', 'Registration Number', 'Model','RelationShip', 'Status', 'Actions'];
    if (this.vehicleGroupData != null) {
      this.updateDataSource(this.vehicleGroupData);
    } else {
      //this.loadVehGroupTable('group');
    }
  }
  loadVehGroupTable(flag: string) {
    this.vehGrpRqst = {
      id: 0,
      organizationID: this.orgId ? this.orgId : 1,
      vehicles: true,
      vehiclesGroup: true,
      groupIds: [0],
    };

    if(flag == 'group') {
      this.vehGrpRqst.vehicles = false;
    } else{
      this.vehGrpRqst.vehicles = true;
    }

    this.vehService
      .getVehicleGroup(this.vehGrpRqst)
      //.pipe(map((data) => data.filter((d) => d.isVehicleGroup == true)))
      .subscribe((_data) => {
        if(_data){
          this.hideloader();
        }
        this.vehicleGroupData = _data;
        this.initData = this.vehicleGroupData;
      });

    this.updateDataSource(this.vehicleGroupData);
  }
  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
    }
  updateDataSource(data: any) {
    setTimeout(() => {
      this.getNewTagData(data);
      this.dataSource = new MatTableDataSource(data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }
  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      let createdDate = new Date(row.createdAt).getTime(); //  need to check API response.
      let nextDate = createdDate + 86400000;

      if(currentDate > createdDate && currentDate < nextDate){
        row.newTag = true;
      }
      else{
        row.newTag = false;
      }
    });
    let newTrueData = data.filter(item => item.newTag == true);
    newTrueData.sort((userobj1,userobj2) => userobj2.createdAt - userobj1.createdAt);
    let newFalseData = data.filter(item => item.newTag == false);
    Array.prototype.push.apply(newTrueData,newFalseData);
    return newTrueData;
  }

}

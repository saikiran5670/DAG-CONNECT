import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { vehicleGetRequest, vehicleGrpGetRequest } from '../../models/vehicle.model';
import { VehicleService } from '../../services/vehicle.service';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';

export interface vehGrpCreation {
  groupName: null;
  groupDesc: null;
}

@Component({
  selector: 'app-vehicle-management',
  templateUrl: './vehicle-management.component.html',
  styleUrls: ['./vehicle-management.component.less'],
})

export class VehicleManagementComponent implements OnInit {
  vehicleDisplayedColumns: string[] = ['vehicleGroupID', 'name', 'isActive', 'createdDate', 'action'];
  veh: vehicleGetRequest;
  vehGrpRqst: vehicleGrpGetRequest;
  vehSelectionFlag: boolean = false;
  mainTableFlag: boolean = true;
  vehGC: vehGrpCreation = { groupName: null, groupDesc: null };
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
      lblAllVehicleGroupVehicleDetails: 'All Vehicle Group/Vehicle Details',
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

  ngOnInit() {
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

    this.vehService
      .getVehicleGroup(this.vehGrpRqst)
      //.pipe(map((data) => data.filter((d) => d.isVehicleGroup == true)))
      .subscribe(
        (_data) => {
          if(_data){
            this.hideloader();
          }
          _data.forEach((res)=>{
            if(res.isVehicleGroup == false){
              tempData.push(res);
            }
            else if(res.isVehicleGroup == true){
              tempDataForGroup.push(res);
            }
          })

          this.vehicleData = tempData;
          this.vehicleGroupData = tempDataForGroup;
          this.initData = this.vehicleGroupData;
          this.selectedType = this.selectedType == '' ? 'group' : this.selectedType;
          if (this.selectedType === 'group') {
            this.loadVehicleGroupDataSource();
          } else if (this.selectedType === 'vehicle') {
            this.loadVehicleDataSource();
          } else if (this.selectedType === 'both') {
            this.loadBothDataSource();
          }
        },
        (error) => {
          console.error(error);
        }
      );
  }

  hideloader() {
  // Setting display of spinner
    this.showLoadingIndicator=false;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce(
      (acc, cur) => ({ ...acc, [cur.name]: cur.value }),
      {}
    );
  }

  newVehicleGroup() {
    this.titleText = this.translationData.lblCreateNewVehicleGroup || 'Create New Vehicle Group';
    this.rowsData = [];
    this.groupInfo = null;
    this.rowsData = this.vehicleData;
    this.editFlag = true;
    this.createStatus = true;
    this.viewGroupMode = false;
  }

  deleteFunc(row: any) {
    if(this.selectedType === 'group'){
      this.deleteVehicleGroup(row);
    }
    else if(this.selectedType === 'vehicle'){
      this.deleteVehicle(row);
    }
    else if(this.selectedType === 'both'){
      if (row.isVehicleGroup) {
        this.deleteVehicleGroup(row);
      } else this.deleteVehicle(row);
    }
  }

  deleteVehicle(row: any) {
    const options = {
      title: this.translationData.lblDeleteVehicle || 'Delete Vehicle',
      message: this.translationData.lblAreyousureyouwanttodeletevehiclegroup || "Are you sure you want to delete '$' vehicle?",
      cancelText: this.translationData.lblNo || 'No',
      confirmText: this.translationData.lblYes || 'Yes'
    };
    this.OpenDialog(options, 'delete', row);
  }

  deleteVehicleGroup(row: any) {
    const options = {
      title: this.translationData.lblDeleteVehicleGroup || 'Delete Vehicle Group',
      message: this.translationData.lblAreyousureyouwanttodeletevehiclegroup || "Are you sure you want to delete '$' vehicle group?",
      cancelText: this.translationData.lblNo || 'No',
      confirmText: this.translationData.lblYes || 'Yes'
    };
    this.OpenDialog(options, 'delete', row);
  }

  OpenDialog(options: any, flag: any, item: any) {
    if (flag == '') {
      //Model for create
      this.dialogService.open(options);
      this.dialogService.confirmed().subscribe((res) => {
        if (res) {
          //save data here
        }
      });
    } else {
      //Model for delete
      let name = (this.selectedType === 'group' || this.selectedType === 'both') ? item.name : item.vin;
      this.dialogService.DeleteModelOpen(options, name);
      this.dialogService.confirmedDel().subscribe((res) => {
        if (res) {
          if (this.selectedType === 'group' || this.selectedType === 'both') {
            this.vehService.deleteVehicleGroup(item.id).subscribe((d) => {
              this.loadVehicleData();
              let msg = this.getCreateMsg(item.name, 'delete');
              this.openSnackBar(msg, 'dismiss');
            });
          } else if (this.selectedType === 'vehicle') {
            this.vehService.deleteVehicle(item.vehicleID).subscribe((d) => {
              this.loadVehicleData();
              let msg = this.getCreateMsg(item.name, 'delete');
              this.openSnackBar(msg, 'dismiss');
            });
          }
        }
      });
    }
  }

  onClose() {
    this.grpTitleVisible = false;
  }

  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => { });
    snackBarRef.onAction().subscribe(() => { });
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  editFunc(row: any) {
    if(this.selectedType === 'group'){
      this.editVehicleGroupDetails(row);
    }
    else if(this.selectedType === 'vehicle'){
      this.editVehicleDetails(row);
    }
    else if(this.selectedType === 'both') {
      if(row.isVehicleGroup){
        this.editVehicleGroupDetails(row);
      }
      else{
        this.editVehicleDetails(row);
      }
    }
  }

  viewFunc(row: any) {
    if(this.selectedType === 'group'){
      this.viewVehicleGroupDetails(row);
    }
    else if(this.selectedType === 'vehicle'){
      this.viewVehicleDetails(row);
    }
    else if(this.selectedType === 'both'){
      if (row.isVehicleGroup) this.editVehicleGroupDetails(row);
      else this.viewVehicleDetails(row);
    }
  }

  editVehicleDetails(item: any) {
    this.viewFlag = true;
    this.viewMode = false;
    this.vinData = item;
  }

  viewVehicleDetails(item: any) {
    this.viewMode = true;
    this.viewFlag = true;
    this.vinData = item;
  }

  editVehicleGroupDetails(row: any) {
    this.titleText = this.translationData.lblEditVehicleGroupDetails || 'Edit Vehicle Group Details';
    this.rowsData = [];
    this.groupInfo = row;
    this.rowsData = this.vehicleData;
    this.editFlag = true;
    this.createStatus = false;
    this.viewGroupMode = false;
  }

  viewVehicleGroupDetails(row: any) {
    this.titleText = this.translationData.lblViewVehicleGroupDetails || 'View Vehicle Group Details';
    this.rowsData = [];
    this.rowsData = this.vehicleData;
    this.groupInfo = row;
    this.vinData = this.vehicleData;
    this.editFlag = true;
    this.viewGroupMode = true;
    this.createStatus = false;
  }

  editBothDetails(row: any) {
    //check if data belongs to group or specific vehicle
    this.titleText = this.translationData.lblEditVehicleGroupDetails || 'Edit Vehicle Group Details';
    this.rowsData = [];
    this.groupInfo = row;
    this.rowsData = this.bothData;
    this.editFlag = true;
    this.createStatus = false;
  }

  editData(item: any) {
    this.editFlag = item.editFlag;
    if (item.editText == 'create') {
      let msg = this.getCreateMsg(item.gridData.name, 'create');
      this.openSnackBar(msg, 'dismiss');
      this.initData = item.gridData;
    } else if (item.editText == 'edit') {
      let msg = this.getCreateMsg(item.gridData.name, 'edit');
      this.userCreatedMsg = msg;
      this.grpTitleVisible = true;
      setTimeout(() => {
        this.grpTitleVisible = false;
      }, 5000);
    }
    this.loadVehicleData();
  }

  getCreateMsg(Name: any, flagS: string) {
    let returnVal = '';
    if (flagS == 'edit'){
      if (this.translationData.lblVehicleGroupdetailssuccessfullyupdated){
        returnVal = this.translationData.lblVehicleGroupdetailssuccessfullyupdated.replace(
          '$',
          Name
        );
      }
      else{
        returnVal = "Vehicle Group '$' details successfully updated".replace(
          '$',
          Name
        );
      }
    } else if(flagS == 'delete'){
      if (this.translationData.lblVehicleGroupDeleted){
        returnVal = this.translationData.lblVehicleGroupDeleted.replace(
          '$',
          Name
        );
      }
      else{
        returnVal = "Vehicle Group '$' was successfully deleted".replace(
          '$',
          Name
        );
      }
    } else{
      if (this.translationData.lblNewVehicleGroupCreatedSuccessfully){
        returnVal = this.translationData.lblNewVehicleGroupCreatedSuccessfully.replace(
          '$',
          Name
        );
      }
      else{
        returnVal = "New Vehicle Group '$' Created Successfully".replace(
          '$',
          Name
        );
      }
    }
    return returnVal;
  }

  updateDataSource(data: any) {
    setTimeout(() => {
      this.dataSource = new MatTableDataSource(data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  onBackToPage(item: any) {
    this.viewFlag = item.editFlag;
    if(item.editText == 'create'){
      this.selectedType = 'group';
    }
    else{
      this.selectedType = 'vehicle';
    }

    if(item.editText == 'edit'){
      let msg = '';
      if(this.translationData.lblVehiclesettingssuccessfullyupdated){
        msg = this.translationData.lblVehiclesettingssuccessfullyupdated.replace(
          '$',
          item.gridData
        );
      }
      else{
        msg = "Vehicle '$' settings successfully updated".replace(
          '$',
          item.gridData
        );
      }
      this.userCreatedMsg = msg;
      this.grpTitleVisible = true;
      setTimeout(() => {
        this.grpTitleVisible = false;
      }, 5000);
    }
    this.loadVehicleData();
    this.updateDataSource(this.initData);
  }

  onChange(event) {
    this.selectedType = event.value;
    if (event.value === 'group') {
      this.initData = this.vehicleGroupData;
      this.loadVehicleGroupDataSource();
    } else if (event.value === 'vehicle') {
      this.initData = this.vehicleData;
      this.loadVehicleDataSource();
    } else if (event.value === 'both') {
      this.bothData = this.vehicleGroupData.concat(this.vehicleData);
      this.initData = this.bothData;
      this.loadBothDataSource();
    }
  }

  loadVehicleGroupDataSource() {
    this.cols = ['name', 'vin', 'license_Plate_Number', 'model', 'status', 'action'];
    this.columnNames = ['Vehicle Group', 'VIN', 'Registration Number', 'Model', 'Status', 'Actions'];
    if (this.vehicleGroupData != null) {
      this.updateDataSource(this.vehicleGroupData);
    } else {
      this.loadVehGroupTable('group');
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

  loadVehicleDataSource() {
    this.cols = ['name', 'vin', 'license_Plate_Number', 'model', 'status', 'action'];
    this.columnNames = ['Vehicle Name', 'VIN', 'Registration Number', 'Model', 'Status', 'Actions'];

    if(this.vehicleData != null){
      this.updateDataSource(this.vehicleData);
    }
    else{
      this.vehGrpRqst = {
        id: 0,
        organizationID: this.orgId ? this.orgId : 1,
        vehicles: true,
        vehiclesGroup: false,
        groupIds: [0],
      };
      this.vehService
      .getVehicleGroup(this.vehGrpRqst)
      .subscribe((_data) => {
        if(_data){
          this.hideloader();
        }
        this.vehicleData = _data;
        this.initData = this.vehicleData;
      });
      this.updateDataSource(this.vehicleData);
    }
  }

  loadBothDataSource() {
    this.cols = ['name', 'vin', 'license_Plate_Number', 'model', 'status', 'action', 'isVehicleGroup'];
    this.columnNames = ['Vehicle Group/Vehicle', 'VIN', 'Registration Number', 'Model', 'Status', 'Actions', 'isVehicleGroup'];

    // if (this.bothData != null) {
    //   this.updateDataSource(this.bothData);
    // } else {
      this.vehGrpRqst = {
        id: 0,
        organizationID: this.orgId ? this.orgId : 1,
        vehicles: true,
        vehiclesGroup: true,
        groupIds: [0],
      };
      this.vehService.getVehicleGroup(this.vehGrpRqst).subscribe((_data) => {
        this.bothData = _data;
        this.updateDataSource(this.bothData);
      });

    //}
  }

}

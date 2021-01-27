import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { forkJoin, Observable, of } from 'rxjs';
import { map, filter } from 'rxjs/internal/operators';
import {
  vehicleGetRequest,
  VehicleGroup,
  vehicleGrpGetRequest,
} from 'src/app/models/vehicle.model';
import { VehicleService } from 'src/app/services/vehicle.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
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
  vehicleDisplayedColumns: string[] = [
    'vehicleGroupID',
    'name',
    'isActive',
    'createdDate',
    'action',
  ];
  veh: vehicleGetRequest;
  vehGrpRqst: vehicleGrpGetRequest;
  vehSelectionFlag: boolean = false;
  mainTableFlag: boolean = true;
  vehGC: vehGrpCreation = { groupName: null, groupDesc: null };
  cols: string[];
  dataSource: any = new MatTableDataSource([]);
  selectedType: any = '';
  columnNames: string[];

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
  orgId:number

  constructor(
    private vehService: VehicleService,
    private dialogService: ConfirmDialogService,
    private _snackBar: MatSnackBar,
    private _router: Router,
    private translationService: TranslationService
  ) {
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
      lblHintMessage:
        'Please select vehicles to from below list to associate with this vehicle group if needed.',
      lblConsent: 'Consent',
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

  ngAfterViewInit() {}

  ngOnInit() {
    //get OrgId
    if (localStorage.getItem('accountOrganizationId') != null) {
      this.orgId = parseInt(localStorage.getItem('accountOrganizationId'));
    }
    this.editFlag=false;
    let translationObj = {
      id: 0,
      code: 'EN-GB', //-- TODO: Lang code based on account
      type: 'Menu',
      name: '',
      value: '',
      filter: '',
      menuId: 21, //-- for vehicle mgnt
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
    this.veh = {
      vehicleId: 0,
      organizationID:this.orgId ? this.orgId : 1,
      vehicleIdList: '',
      vin: '',
      status: 73,
    };
   
    this.vehService.getVehicle(this.veh).subscribe(
      (_data) => {
        this.vehicleData = _data;
        //console.log('1st call', this.vehicleData);
      },
      (error) => {
        console.log(error);
      }
    );

    // forkJoin(
    //   this.vehService.getVehicleGroup(this.vehGrpRqst),
    //   this.vehService.getVehicle(this.veh)
    // ).subscribe((_data) => {
    //   this.vehicleGroupData = _data[0];
    //     this.vehicleData = _data[1];
    //     this.initData = _data[0];
    //     this.bothData = _data[0].concat(_data[1]);
    //     console.log('2nd call', this.bothData);
    // })

    this.vehService
      .getVehicleGroup(this.vehGrpRqst)
      .pipe(map((data) => data.filter((d) => d.isVehicleGroup == true)))
      .subscribe(
        (_data) => {
          this.vehicleGroupData = _data;
          //console.log('getVehicleGroup call', this.vehicleGroupData);
          //this.vehicleData =of(this.vehicleGroupData).pipe(filter( _data => _data.isVehicleGroup == false));
          //const ob$: Observable<any> = of(this.vehicleGroupData).pipe(filter( _data => _data.isVehicleGroup == true));

          this.initData = _data;
         

          this.selectedType =
            this.selectedType == '' ? 'group' : this.selectedType;
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

  processTranslation(transData: any) {
    this.translationData = transData.reduce(
      (acc, cur) => ({ ...acc, [cur.name]: cur.value }),
      {}
    );
  }

  newVehicleGroup() {
    this.titleText =
      this.translationData.lblCreateNewVehicleGroup ||
      'Create New Vehicle Group';
    this.rowsData = [];
    this.groupInfo = null;
    this.rowsData = this.vehicleData;
    this.editFlag = true;
    this.createStatus = true;
    this.viewGroupMode = false;
  }

  deleteFunc(row: any) {
    if (this.selectedType === 'group') this.deleteVehicleGroup(row);
    else if (this.selectedType === 'vehicle') this.deleteVehicle(row);
    else if (this.selectedType === 'both') {
      if (row.isVehicleGroup) {
        this.deleteVehicleGroup(row);
      } else this.deleteVehicle(row);
    }
  }

  deleteVehicle(row: any) {
    const options = {
      title: this.translationData.lblDeleteVehicle || 'Delete Vehicle',
      message:
        this.translationData.lblAreyousureyouwanttodeletevehiclegroup ||
        "Are you sure you want to delete '$' vehicle?",
      cancelText: this.translationData.lblNo || 'No',
      confirmText: this.translationData.lblYes || 'Yes',
    };
    this.OpenDialog(options, 'delete', row);
  }

  deleteVehicleGroup(row: any) {
    const options = {
      title:
        this.translationData.lblDeleteVehicleGroup || 'Delete Vehicle Group',
      message:
        this.translationData.lblAreyousureyouwanttodeletevehiclegroup ||
        "Are you sure you want to delete '$' vehicle group?",
      cancelText: this.translationData.lblNo || 'No',
      confirmText: this.translationData.lblYes || 'Yes',
    };
    this.OpenDialog(options, 'delete', row);
  }

  OpenDialog(options, flag, item) {
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
      let name =
        this.selectedType === 'group' || this.selectedType === 'both'
          ? item.name
          : item.vin;
      this.dialogService.DeleteModelOpen(options, name);
      this.dialogService.confirmedDel().subscribe((res) => {
        if (res) {
          if (this.selectedType === 'group' || this.selectedType === 'both') {
            this.vehService.deleteVehicleGroup(item.id).subscribe((d) => {
              this.loadVehicleData();
              this.openSnackBar('Item delete', 'dismiss');
            });
          } else if (this.selectedType === 'vehicle') {
            this.vehService.deleteVehicle(item.vehicleID).subscribe((d) => {
              this.loadVehicleData();
              this.openSnackBar('Item delete', 'dismiss');
            });
          }
        }
      });
    }
  }

  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      //console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      //console.log('The snackbar action was triggered!');
    });
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  editFunc(row: any) {
    if (this.selectedType === 'group') this.editVehicleGroupDetails(row);
    else if (this.selectedType === 'vehicle') this.editVehicleDetails(row);
    else if (this.selectedType === 'both') {
      // console.log('is row a group type? ',row.isVehicleGroup);
      if (row.isVehicleGroup) this.editVehicleGroupDetails(row);
      else this.editVehicleDetails(row);
    }
  }
  viewFunc(row: any) {
    if (this.selectedType === 'group') this.viewVehicleGroupDetails(row);
    else if (this.selectedType === 'vehicle') this.viewVehicleDetails(row);
    else if (this.selectedType === 'both') {
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
    this.titleText =
      this.translationData.lblEditVehicleGroupDetails ||
      'Edit Vehicle Group Details';
    this.rowsData = [];
    //this.rowsData.push(row);
    this.groupInfo = row;
    this.rowsData = this.vehicleData;
    this.editFlag = true;
    this.createStatus = false;
    this.viewGroupMode = false;
  }
  viewVehicleGroupDetails(row: any) {
    this.titleText =
      this.translationData.lblViewVehicleGroupDetails ||
      'View Vehicle Group Details';
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
    console.log(row);
    this.titleText =
      this.translationData.lblEditVehicleGroupDetails ||
      'Edit Vehicle Group Details';
    this.rowsData = [];
    //this.rowsData.push(row);
    this.groupInfo = row;
    this.rowsData = this.bothData;
    this.editFlag = true;
    this.createStatus = false;
  }

  editData(item: any) {
    this.editFlag = item.editFlag;
    if (item.editText == 'create') {
      this.openSnackBar('Item created', 'dismiss');
      this.initData = item.gridData;
    } else if (item.editText == 'edit') {
      this.openSnackBar('Item edited', 'dismiss');
      this.initData = item.gridData;
    }
    this.loadVehicleData();
    this.updateDataSource(this.initData);
  }

  updateDataSource(data: any) {
    setTimeout(() => {
      this.dataSource = new MatTableDataSource(data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  onBackToPage(flag: any) {
    this.viewFlag = flag;
    this.selectedType = 'vehicle';
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
    this.cols = [
      'name',
      'vin',
      'license_Plate_Number',
      'model',
      'status',
      'action',
    ];
    this.columnNames = [
      'Vehicle Group',
      'VIN',
      'Registration Number',
      'Model',
      'Status',
      'Actions',
    ];
    if (this.vehicleGroupData != null) {
      this.updateDataSource(this.vehicleGroupData);
    } else {
      this.vehGrpRqst = {
        id: 0,
        organizationID: this.orgId ? this.orgId : 1,
        vehicles: true,
        vehiclesGroup: true,
        groupIds: [0],
      };
      this.vehService.getVehicleGroup(this.vehGrpRqst).subscribe((_data) => {
        this.vehicleGroupData = _data;
        this.initData = this.vehicleGroupData;
      });
      this.updateDataSource(this.vehicleGroupData);
    }
  }

  loadVehicleDataSource() {
    this.cols = [
      'name',
      'vin',
      'license_Plate_Number',
      'model',
      'status',
      'action',
    ];
    this.columnNames = [
      'Vehicle Name',
      'VIN',
      'Registration Number',
      'Model',
      'Status',
      'Actions',
    ];

    if (this.vehicleData != null) {
      this.updateDataSource(this.vehicleData);
    } else {
      this.veh = {
        vehicleId: 0,
        organizationID: this.orgId ? this.orgId : 1,
        vehicleIdList: '',
        vin: '',
        status: 73,
      };
      this.vehService.getVehicle(this.veh).subscribe((_data) => {
        this.vehicleData = _data;
        this.initData = this.vehicleData;
      });
      this.updateDataSource(this.vehicleData);
    }
  }
  loadBothDataSource() {
    this.cols = [
      'name',
      'vin',
      'license_Plate_Number',
      'model',
      'status',
      'action',
    ];
    this.columnNames = [
      'Vehicle Group/Vehicle',
      'VIN',
      'Registration Number',
      'Model',
      'Status',
      'Actions',
    ];

    if (this.bothData != null) {
      this.updateDataSource(this.bothData);
    } else {
      this.vehGrpRqst = {
        id: 0,
        organizationID: this.orgId ? this.orgId : 1,
        vehicles: true,
        vehiclesGroup: true,
        groupIds: [0],
      };
      this.vehService.getVehicleGroup(this.vehGrpRqst).subscribe((_data) => {
        this.bothData = _data;
      });
      this.updateDataSource(this.bothData);
    }
  }
}

import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { POIService } from 'src/app/services/poi.service';
import { GeofenceService } from 'src/app/services/landmarkGeofence.service';
import { element } from 'protractor';
import { LandmarkGroupService } from 'src/app/services/landmarkGroup.service';


@Component({
  selector: 'app-create-edit-view-group',
  templateUrl: './create-edit-view-group.component.html',
  styleUrls: ['./create-edit-view-group.component.less']
})
export class CreateEditViewGroupComponent implements OnInit {
  OrgId: any = 0;
  accountId: any= 0;
  @Output() backToPage = new EventEmitter<any>();
  displayedColumnsPOI: string[] = ['select', 'icon', 'name', 'categoryName', 'subCategoryName', 'address'];
  displayedColumnsGeofence: string[] = ['select', 'geofenceName', 'categoryName', 'subCategoryName']
  selectedPOI = new SelectionModel(true, []);
  selectedGeofence = new SelectionModel(true, []);
  poiDataSource: any = new MatTableDataSource([]);
  geofenceDataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) poiPaginator: MatPaginator;
  @ViewChild(MatPaginator) geofencePaginator: MatPaginator;
  @ViewChild(MatSort) poiSort: MatSort;
  @ViewChild(MatSort) geofenceSort: MatSort;
  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Input() titleText: any;
  groupCreatedMsg: any = '';
  breadcumMsg: any = '';
  landmarkGroupForm: FormGroup;
  duplicateGroupMsg: boolean= false;


  constructor(private _formBuilder: FormBuilder, private poiService: POIService, private geofenceService: GeofenceService, private landmarkGroupService: LandmarkGroupService) { }

  ngOnInit() {
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.landmarkGroupForm = this._formBuilder.group({
      landmarkGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      landmarkGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('landmarkGroupName'),
        CustomValidators.specialCharValidationForNameWithoutRequired('landmarkGroupDescription')
      ]
    });

    if(this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    if(this.actionType == 'view' || this.actionType == 'edit'){
      this.breadcumMsg = this.getBreadcum();
    }
    this.loadPOIData();
    this.loadGeofenceData();
  }

  setDefaultValue(){
    this.landmarkGroupForm.get('landmarkGroupName').setValue(this.selectedRowData.name);
    if(this.selectedRowData.description)
      this.landmarkGroupForm.get('landmarkGroupDescription').setValue(this.selectedRowData.description);
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblLandmarks : "Landmarks"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditGroupDetails ? this.translationData.lblEditGroupDetails : 'Edit Group Details') : (this.translationData.lblViewGroupDetails ? this.translationData.lblViewGroupDetails : 'View Group Details')}`;
  }

  loadPOIData() {
    this.poiService.getPois(this.OrgId).subscribe((poilist: any) => {
      let poiGridData = poilist;
      this.updatePOIDataSource(poiGridData);
      if(this.actionType == 'view' || this.actionType == 'edit')
        this.loadPOISelectedData(poiGridData);
    });
  }

  loadPOISelectedData(tableData: any){
    let selectedPOIList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.id && item.type == "P");
        if (search.length > 0) {
          selectedPOIList.push(row);
        }
      });
      tableData = selectedPOIList;
      this.displayedColumnsPOI= ['icon', 'name', 'categoryName', 'subCategoryName',, 'address'];
      this.updatePOIDataSource(tableData);
    }
    else if(this.actionType == 'edit' ){
      this.selectPOITableRows();
    }
  }

  selectPOITableRows(){
    this.poiDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.id && item.type == "P");
      if (search.length > 0) {
        this.selectedPOI.select(row);
      }
    });
  }


  loadGeofenceData() {
    this.geofenceService.getAllGeofences(this.OrgId).subscribe((geofencelist: any) => {
      let geofenceGridData = geofencelist.geofenceList;
      this.updateGeofenceDataSource(geofenceGridData);
      if(this.actionType == 'view' || this.actionType == 'edit')
        this.loadGeofenceSelectedData(geofenceGridData);
    });
  }

  loadGeofenceSelectedData(tableData: any){
    let selectedGeofenceList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
        if (search.length > 0) {
          selectedGeofenceList.push(row);
        }
      });
      tableData = selectedGeofenceList;
      this.displayedColumnsGeofence= ['geofenceName', 'categoryName', 'subCategoryName'];
      this.updateGeofenceDataSource(tableData);
    }
    else if(this.actionType == 'edit' ){
      this.selectGeofenceTableRows();
    }
  }

  selectGeofenceTableRows(){
    this.geofenceDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
      if (search.length > 0) {
        this.selectedGeofence.select(row);
      }
    });
  }

  updatePOIDataSource(tableData: any){
    this.poiDataSource= new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.poiDataSource.paginator = this.poiPaginator;
      this.poiDataSource.sort = this.poiSort;
    });
  }

  updateGeofenceDataSource(tableData: any){
    this.geofenceDataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.geofenceDataSource.paginator = this.geofencePaginator;
      this.geofenceDataSource.sort = this.geofenceSort;
    });
  }

  onReset(){ //-- Reset
    this.selectedPOI.clear();
    this.selectedGeofence.clear();
    this.selectPOITableRows();
    this.selectGeofenceTableRows();
    this.setDefaultValue();
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onCreateUpdate() {
    this.duplicateGroupMsg = false;
    let landmarkList = [];
    this.selectedPOI.selected.forEach(element => {
      landmarkList.push({"id": element.id, type: "P"  })
    });
    this.selectedGeofence.selected.forEach(element => {
      landmarkList.push({"id": element.geofenceId, type: element.type ? element.type : "O"  }) //"O" for polygon geofence
    })
    if(this.actionType == 'create'){ // create
      let createGrpObj = {
          id: 0,
          organizationId: this.OrgId,
          name: this.landmarkGroupForm.controls.landmarkGroupName.value,
          description: this.landmarkGroupForm.controls.landmarkGroupDescription.value,
          iconId: 0,
          state: "",
          createdAt: 0,
          createdBy: this.accountId,
          modifiedAt: 0,
          modifiedBy: 0,
          poilist: landmarkList
        }
        
        this.landmarkGroupService.createLandmarkGroup(createGrpObj).subscribe((response) => {
          let objData = { 
              organizationid : this.OrgId,
          };
          this.landmarkGroupService.getLandmarkGroups(objData).subscribe((landmarkGrpData: any) => {
            this.groupCreatedMsg = this.getGroupCreatedMessage();
            let emitObj = { actionFlag: false, gridData: landmarkGrpData["groups"], successMsg: this.groupCreatedMsg };
            this.backToPage.emit(emitObj);
          }, (err) => { });
        }, (err) => {
          //console.log(err);
          if (err.status == 409) {
            this.duplicateGroupMsg = true;
          }
        });
    }
    else{ // update
      let updateGrpObj = {
        id: this.selectedRowData.id,
        organizationId: this.OrgId,
        name: this.landmarkGroupForm.controls.landmarkGroupName.value,
        description: this.landmarkGroupForm.controls.landmarkGroupDescription.value,
        iconId: 0,
        state: "",
        createdAt: 0,
        createdBy: this.accountId,
        modifiedAt: 0,
        modifiedBy: 0,
        poilist: landmarkList
      }

      this.landmarkGroupService.updateLandmarkGroup(updateGrpObj).subscribe((d) => {
        let accountGrpObj: any = {
          accountId: 0,
          organizationId: this.OrgId,
          accountGroupId: 0,
          vehicleGroupId: 0,
          roleId: 0,
          name: ""
        }
        let objData = { 
          organizationid : this.OrgId,
        };
        this.landmarkGroupService.getLandmarkGroups(objData).subscribe((landmarkGrpData: any) => {
        this.groupCreatedMsg = this.getGroupCreatedMessage();
        let emitObj = { actionFlag: false, gridData: landmarkGrpData["groups"], successMsg: this.groupCreatedMsg };
        this.backToPage.emit(emitObj);
      }, (err) => { });
      }, (err) => {
        //console.log(err);
        if (err.status == 409) {
          this.duplicateGroupMsg = true;
        }
      });
    }
  }

  getGroupCreatedMessage() {
    let groupName = `${this.landmarkGroupForm.controls.landmarkGroupName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblLandmarkGroupCreatedSuccessfully)
        return this.translationData.lblLandmarkGroupCreatedSuccessfully.replace('$', groupName);
      else
        return ("Landmark Group '$' Created Successfully").replace('$', groupName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblLandmarkGroupUpdatedSuccessfully)
        return this.translationData.lblLandmarkGroupUpdatedSuccessfully.replace('$', groupName);
      else
        return ("Landmark Group '$' Updated Successfully").replace('$', groupName);
    }
    else{
      return '';
    }
  }

  applyFilterForPOI(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.poiDataSource.filter = filterValue;
  }

  applyFilterForGeofence(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.geofenceDataSource.filter = filterValue;
  }

  masterToggleForPOI() {
    this.isAllSelectedForPOI()
      ? this.selectedPOI.clear()
      : this.poiDataSource.data.forEach((row) =>
        this.selectedPOI.select(row)
      );
  }

  isAllSelectedForPOI() {
    const numSelected = this.selectedPOI.selected.length;
    const numRows = this.poiDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForPOI(row?: any): string {
    if (row)
      return `${this.isAllSelectedForPOI() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedPOI.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  masterToggleForGeofence() {
    this.isAllSelectedForGeofence()
      ? this.selectedGeofence.clear()
      : this.geofenceDataSource.data.forEach((row) =>
        this.selectedGeofence.select(row)
      );
  }

  isAllSelectedForGeofence() {
    const numSelected = this.selectedGeofence.selected.length;
    const numRows = this.geofenceDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeofence(row?: any): string {
    if (row)
      return `${this.isAllSelectedForGeofence() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedGeofence.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

}

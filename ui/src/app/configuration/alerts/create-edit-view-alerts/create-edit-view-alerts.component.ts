import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChildren, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DomSanitizer } from '@angular/platform-browser';
import { AlertService } from 'src/app/services/alert.service';
import { GeofenceService } from 'src/app/services/landmarkGeofence.service';
import { LandmarkGroupService } from 'src/app/services/landmarkGroup.service';
import { POIService } from 'src/app/services/poi.service';
import { CommonTableComponent } from 'src/app/shared/common-table/common-table.component';
import { CustomValidators } from 'src/app/shared/custom.validators';

declare var H: any;

@Component({
  selector: 'app-create-edit-view-alerts',
  templateUrl: './create-edit-view-alerts.component.html',
  styleUrls: ['./create-edit-view-alerts.component.less']
})
export class CreateEditViewAlertsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() actionType: any;
  @Input() translationData: any = [];
  @Input() selectedRowData: any;
  @Input() alertCategoryList: any;
  @Input() alertTypeList: any;
  @Input() vehicleGroupList: any;
  @Input() vehicleList: any;
  displayedColumnsPOI: string[] = ['select', 'icon', 'name', 'categoryName', 'subCategoryName', 'address'];
  displayedColumnsGeofence: string[] = ['select', 'geofenceName', 'categoryName', 'subCategoryName'];
  groupDisplayedColumns: string[] = ['select', 'name', 'poiCount', 'geofenceCount'];
  corridorDisplayedColumns: string[] = ['select', 'name', 'poiCount', 'geofenceCount'];
  selectedPOI = new SelectionModel(true, []);
  selectedGeofence = new SelectionModel(true, []);
  selectedGroup = new SelectionModel(true, []);
  selectedCorridor = new SelectionModel(true, []);
  poiDataSource: any = new MatTableDataSource([]);
  geofenceDataSource: any = new MatTableDataSource([]);
  groupDataSource: any = new MatTableDataSource([]);
  corridorDataSource: any = new MatTableDataSource([]);
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  dialogRef: MatDialogRef<CommonTableComponent>;
  alertCreatedMsg: any = '';
  breadcumMsg: any = '';
  alertForm: FormGroup;
  accountOrganizationId: number;
  accountId: number;
  userType: string;
  selectedApplyOn: string;
  openAdvancedFilter: boolean= false;
  poiGridData = [];
  geofenceGridData = [];
  groupGridData = [];
  corridorGridData = [];
  isDuplicateAlert: boolean= false;
  private platform: any;
  map: any;
  alertTypeByCategoryList: any= [];
  vehicleByVehGroupList: any= [];
  typesOfLevel: any= [
                      {
                        levelType : 'C',
                        value: 'Critical'
                      },
                      {
                        levelType : 'W',
                        value: 'Warning'
                      }, 
                      {
                        levelType : 'A',
                        value: 'Advisory'
                      }
                    ];

  @ViewChild("map")
  public mapElement: ElementRef;
  constructor(private _formBuilder: FormBuilder,
              private poiService: POIService,
              private geofenceService: GeofenceService, 
              private landmarkGroupService: LandmarkGroupService, 
              private domSanitizer: DomSanitizer, 
              private dialog: MatDialog,
              private alertService: AlertService) 
  {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(): void {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.alertForm = this._formBuilder.group({
      alertName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      alertCategory: ['', [Validators.required]],
      alertType: ['', [Validators.required]],
      applyOn: ['vehicle_group', [Validators.required]],
      vehicleGroup: ['', [Validators.required]],
      vehicle: [''],
      statusMode: ['active', [Validators.required]],
      alertLevel: ['critical', [Validators.required]],
      levelCheckbox: ['', [Validators.required]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('alertName')  
      ]
    });

    if(this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    if(this.actionType == 'view' || this.actionType == 'edit'){
      this.breadcumMsg = this.getBreadcum();
    }

    this.vehicleGroupList= [
      {
        id: 1,
        value: 'Vehicle Group 001'
      },
      {
        id: 2,
        value: 'Vehicle Group 002'
      },
      {
        id: 3,
        value: 'Vehicle Group 003'
      }
    ];

    this.vehicleList= [
      {
        id: 1,
        value: 'Vehicle 1',
        vehicleGroupId: 1
      },
      {
        id: 2,
        value: 'Vehicle 2',
        vehicleGroupId: 1
      },
      {
        id: 3,
        value: 'Vehicle 3',
        vehicleGroupId: 1
      },
      {
        id: 4,
        value: 'Vehicle 4',
        vehicleGroupId: 2
      },
      {
        id: 5,
        value: 'Vehicle 5',
        vehicleGroupId: 2
      },
      {
        id: 6,
        value: 'Vehicle 6',
        vehicleGroupId: 2
      },
      {
        id: 7,
        value: 'Vehicle 7',
        vehicleGroupId: 3
      },
      {
        id: 8,
        value: 'Vehicle 8',
        vehicleGroupId: 3
      },
      {
        id: 9,
        value: 'Vehicle 9',
        vehicleGroupId: 3
      }
    ]

    this.alertTypeByCategoryList= this.alertTypeList;
    this.vehicleByVehGroupList= this.vehicleList;

    this.loadPOIData();
    this.loadGeofenceData();
    this.loadGroupData();
    this.loadCorridorData();

    if(this.alertCategoryList.length== 0 || this.alertTypeList.length == 0 || this.vehicleList.length == 0)
      this.loadFiltersData();
    if(this.vehicleGroupList.length == 0)
      this.loadVehicleGroupData(); 
  }

  loadFiltersData(){
    this.alertService.getAlertFilterData(this.accountId).subscribe((data) => {
      let filterData = data["enumTranslation"];
      filterData.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });
      this.alertCategoryList= filterData.filter(item => item.type == 'C');
      this.alertTypeList= filterData.filter(item => item.type == 'T');
      this.vehicleList= data["vehicleGroup"];
      this.alertTypeByCategoryList= this.alertTypeList;
      this.vehicleByVehGroupList= this.vehicleList;

    }, (error) => {

    })
  }

  loadVehicleGroupData(){

  }

  onChangeAlertCategory(event){
    this.alertTypeByCategoryList= this.alertTypeList.filter(item => item.parentEnum == event.value);
  }

  public ngAfterViewInit() {
    let defaultLayers = this.platform.createDefaultLayers();
    this.map = new H.Map(
        this.mapElement.nativeElement,
        defaultLayers.vector.normal.map,
        {
          center: { lat: 50, lng: 5 },
          zoom: 4,
          pixelRatio: window.devicePixelRatio || 1
        }
    );
    window.addEventListener('resize', () => this.map.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
    var ui = H.ui.UI.createDefault(this.map, defaultLayers);
}

  checkboxClicked(row) {
    // console.log(this.selectedGroup.isSelected);
    
    console.log("checkbox is clicked");
    console.log(row.latitude);
    console.log(row.longitude);
    let marker = new H.map.Marker({lat:row.latitude, lng:row.longitude});
    this.map.addObject(marker);
    
  }
  
  setDefaultValue(){
    // this.landmarkGroupForm.get('landmarkGroupName').setValue(this.selectedRowData.name);
    // if(this.selectedRowData.description)
    //   this.landmarkGroupForm.get('landmarkGroupDescription').setValue(this.selectedRowData.description);
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblLandmarks : "Landmarks"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditGroupDetails ? this.translationData.lblEditGroupDetails : 'Edit Group Details') : (this.translationData.lblViewGroupDetails ? this.translationData.lblViewGroupDetails : 'View Group Details')}`;
  }

  loadPOIData() {
    this.poiService.getPois(this.accountOrganizationId).subscribe((poilist: any) => {
      if(poilist.length > 0){
        poilist.forEach(element => {
          if(element.icon && element.icon != '' && element.icon.length > 0){
            element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + element.icon);
          }else{
            element.icon = '';
          }
        });
        this.poiGridData = poilist;
        this.updatePOIDataSource(this.poiGridData);
        if(this.actionType == 'view' || this.actionType == 'edit')
        this.loadPOISelectedData(this.poiGridData);
      }
      
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
      this.displayedColumnsPOI= ['icon', 'name', 'categoryName', 'subCategoryName', 'address'];
      this.updatePOIDataSource(tableData);
    }
    else if(this.actionType == 'edit' ){
      this.selectPOITableRows(this.selectedRowData);
    }
  }

  selectPOITableRows(rowData: any){
    this.poiDataSource.data.forEach((row: any) => {
      let search = rowData.landmarks.filter(item => item.landmarkid == row.id && item.type == "P");
      if (search.length > 0) {
        this.selectedPOI.select(row);
      }
    });
  }


  loadGeofenceData() {
    this.geofenceService.getAllGeofences(this.accountOrganizationId).subscribe((geofencelist: any) => {
      this.geofenceGridData = geofencelist.geofenceList;
     this.geofenceGridData = this.geofenceGridData.filter(item => item.type == "C" || item.type == "O");
      this.updateGeofenceDataSource(this.geofenceGridData);
      if(this.actionType == 'view' || this.actionType == 'edit')
        this.loadGeofenceSelectedData(this.geofenceGridData);
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
      this.selectGeofenceTableRows(this.selectedRowData);
    }
  }

  selectGeofenceTableRows(rowData: any){
    this.geofenceDataSource.data.forEach((row: any) => {
      let search = rowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
      if (search.length > 0) {
        this.selectedGeofence.select(row);
      }
    });
  }

  loadGroupData(){
    let objData = { 
      organizationid : this.accountOrganizationId,
   };

    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((data: any) => {
      if(data){
        this.groupGridData = data["groups"];
        this.updateGroupDatasource(this.groupGridData);
      }
    }, (error) => {
      //console.log(error)
    });
  }

  loadGroupSelectedData(tableData: any){
    let selectedGroupList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
        if (search.length > 0) {
          selectedGroupList.push(row);
        }
      });
      tableData = selectedGroupList;
      this.displayedColumnsGeofence= ['name', 'poiCount', 'geofenceCount'];
      this.updateGroupDatasource(tableData);
    }
    else if(this.actionType == 'edit'){
      this.selectGroupTableRows();
    }
  }

  selectGroupTableRows(){
    this.groupDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.landmarks.filter(item => item.groupId == row.id);
      if (search.length > 0) {
        this.selectedGroup.select(row);
      }
    });
  }

  loadCorridorData(){
    let objData = { 
      organizationid : this.accountOrganizationId,
   };

    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((data: any) => {
      if(data){
        this.groupGridData = data["groups"];
        this.updateGroupDatasource(this.groupGridData);
      }
    }, (error) => {
      //console.log(error)
    });
  }

  loadCorridorSelectedData(tableData: any){
    let selectedGroupList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
        if (search.length > 0) {
          selectedGroupList.push(row);
        }
      });
      tableData = selectedGroupList;
      this.corridorDisplayedColumns= ['name', 'poiCount', 'geofenceCount'];
      this.updateCorridorDatasource(tableData);
    }
    else if(this.actionType == 'edit'){
      this.selectCorridorTableRows();
    }
  }

  selectCorridorTableRows(){
    this.corridorDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.landmarks.filter(item => item.groupId == row.id);
      if (search.length > 0) {
        this.selectedCorridor.select(row);
      }
    });
  }

  updatePOIDataSource(tableData: any){
    this.poiDataSource= new MatTableDataSource(tableData);
    this.poiDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.categoryName.toString().toLowerCase().includes(filter) ||
        data.subCategoryName.toString().toLowerCase().includes(filter) || 
        data.address.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.poiDataSource.paginator = this.paginator.toArray()[0];
      this.poiDataSource.sort = this.sort.toArray()[0];
    });
  }

  updateGeofenceDataSource(tableData: any){
    this.geofenceDataSource = new MatTableDataSource(tableData);
    this.geofenceDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.geofenceName.toString().toLowerCase().includes(filter) ||
        data.categoryName.toString().toLowerCase().includes(filter) ||
        data.subCategoryName.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.geofenceDataSource.paginator = this.paginator.toArray()[1];
      this.geofenceDataSource.sort = this.sort.toArray()[1];
    });
  }

  updateGroupDatasource(tableData: any){
    this.groupDataSource = new MatTableDataSource(tableData);
    this.groupDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.poiCount.toString().toLowerCase().includes(filter) ||
        data.geofenceCount.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.groupDataSource.paginator = this.paginator.toArray()[2];
      this.groupDataSource.sort = this.sort.toArray()[2];
    });
  }

  updateCorridorDatasource(tableData: any){
    this.corridorDataSource = new MatTableDataSource(tableData);
    this.corridorDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.poiCount.toString().toLowerCase().includes(filter) ||
        data.geofenceCount.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.corridorDataSource.paginator = this.paginator.toArray()[3];
      this.corridorDataSource.sort = this.sort.toArray()[3];
    });
  }


  onPOIClick(row: any){
    const colsList = ['icon', 'landmarkname', 'categoryname', 'subcategoryname', 'address'];
    const colsName = [this.translationData.lblIcon || 'Icon', this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category', this.translationData.lblAddress || 'Address'];
    const tableTitle = this.translationData.lblPOI || 'POI';
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
    };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => item.type == "P");
      if(this.selectedRowData.length > 0){
        this.selectedRowData.forEach(element => {
          if(element.icon && element.icon != '' && element.icon.length > 0){
            let TYPED_ARRAY = new Uint8Array(element.icon);
            let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
            let base64String = btoa(STRING_CHAR);
            element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + base64String);
          }else{
            element.icon = '';
          }
        });
        this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
      }
    });
  }

  onGeofenceClick(row: any){
    const colsList = ['landmarkname', 'categoryname', 'subcategoryname'];
    const colsName = ['Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category'];
    const tableTitle = this.translationData.lblGeofence || 'Geofence';
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
   };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => (item.type == "C" || item.type == "O"));
      this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
    });
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: colsList,
      colsName: colsName,
      tableTitle: tableTitle
    }
    this.dialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }


  onReset(){ //-- Reset
    this.selectedPOI.clear();
    this.selectedGeofence.clear();
    this.selectPOITableRows(this.selectedRowData);
    this.selectGeofenceTableRows(this.selectedRowData);
    this.setDefaultValue();
  }

  onCancel(){
    let emitObj = {
      actionFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onApplyOnChange(event){
    this.selectedApplyOn = event.value;
  }

  onClickAdvancedFilter(){
    this.openAdvancedFilter = !this.openAdvancedFilter;
  }

  onCreateUpdate(){

  }

  getGroupCreatedMessage() {
    let alertName = `${this.alertForm.controls.alertName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblAlertCreatedSuccessfully)
        return this.translationData.lblAlertCreatedSuccessfully.replace('$', alertName);
      else
        return ("Alert '$' Created Successfully").replace('$', alertName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblAlertUpdatedSuccessfully)
        return this.translationData.lblAlertUpdatedSuccessfully.replace('$', alertName);
      else
        return ("Alert '$' Updated Successfully").replace('$', alertName);
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

  applyFilterForGroup(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.groupDataSource.filter = filterValue;
  }

  applyFilterForCorridor(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.corridorDataSource.filter = filterValue;
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

  masterToggleForGroup() {
    this.isAllSelectedForGroup()
      ? this.selectedGroup.clear()
      : this.groupDataSource.data.forEach((row) =>
        this.selectedGroup.select(row)
      );
  }

  isAllSelectedForGroup() {
    const numSelected = this.selectedGroup.selected.length;
    const numRows = this.groupDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForGroup(row?: any): string {
    if (row)
      return `${this.isAllSelectedForGroup() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedGroup.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  masterToggleForCorridor() {
    this.isAllSelectedForCorridor()
      ? this.selectedCorridor.clear()
      : this.corridorDataSource.data.forEach((row) =>
        this.selectedCorridor.select(row)
      );
  }

  isAllSelectedForCorridor() {
    const numSelected = this.selectedCorridor.selected.length;
    const numRows = this.corridorDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForCorridor(row?: any): string {
    if (row)
      return `${this.isAllSelectedForCorridor() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedCorridor.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onGroupSelect(event: any, row: any){
    let groupDetails= [];
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
    };
    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupData) => {
      groupDetails = groupData["groups"][0];
      this.selectPOITableRows(groupDetails);
      this.selectGeofenceTableRows(groupDetails);
    });
  }
}

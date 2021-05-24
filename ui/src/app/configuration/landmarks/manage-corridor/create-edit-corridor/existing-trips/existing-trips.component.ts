import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
// import { AccountService } from '../../../services/account.service';
import { CustomValidators } from '../../../../../shared/custom.validators';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';

@Component({
  selector: 'app-existing-trips',
  templateUrl: './existing-trips.component.html',
  styleUrls: ['./existing-trips.component.less']
})
export class ExistingTripsComponent implements OnInit {
  endDate = new FormControl();
  startDate = new FormControl();
  startTime = new FormControl();
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @Input() disabled: boolean;
  @Input() value: string = '11:00 PM';
  @Input() format: number = 12;
  selectedStartTime: any = '12:00 AM'
  selectedEndTime: any = '12:00 AM'
  timeValue: any = 0;
  // range = new FormGroup({
  //   start: new FormControl(),
  //   end: new FormControl()
  // });
  // translationData: any;
  OrgId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  // displayedColumns: string[] = ['select', 'firstName', 'emailId', 'roles', 'accountGroups'];
  selectedAccounts = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  displayedColumns = ['All', 'DriverName', 'distance', 'date', 'startPoint', 'endPoint'];
  createEditStatus = false;
  accountOrganizationId: any = 0;
  corridorCreatedMsg: any = '';
  actionType: string;
  titleVisible: boolean = false;
  titleFailVisible: boolean = false;
  showMap: boolean = false;
  map: any;
  initData: any = [];
  // dataSource: any;
  markerArray: any = [];
  showLoadingIndicator: boolean;
  selectedCorridors = new SelectionModel(true, []);

  // @Input() translationData: any;
  // @Input() selectedRowData: any;
  // @Input() actionType: any;
  // userCreatedMsg: any = '';
  // duplicateEmailMsg: boolean = false;
  // breadcumMsg: any = '';
  existingTripForm: FormGroup;
  // groupTypeList: any = [];
  // showUserList: boolean = true;
  @Input() translationData: any;
  @Input() vehicleGroupList: any;
  vinList: any = [];
  vinListSelectedValue: any = [];
  vehicleGroupIdsSet: any = [];
  localStLanguage: any;

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.vehicleGroupList.forEach(item => {
      this.vehicleGroupIdsSet.push(item.vehicleGroupId);
      this.vehicleGroupIdsSet = [...new Set(this.vehicleGroupIdsSet)];
    });
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.existingTripForm = this._formBuilder.group({
      // userGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      // userGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
      {
        validator: [
          // CustomValidators.specialCharValidationForName('userGroupName'),
          // CustomValidators.specialCharValidationForNameWithoutRequired('userGroupDescription')
        ]
      });
    this.loadExistingTripData();
  }
  loadExistingTripData() {
    this.showLoadingIndicator = true;
    // this.corridorService.getCorridorList(this.accountOrganizationId).subscribe((data : any) => {
    //   this.initData = data;
    //   this.hideloader();
    //   this.updatedTableData(this.initData);
    // }, (error) => {
    //   this.initData = [];
    //   this.hideloader();
    //   this.updatedTableData(this.initData);
    // });
  }
  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }
  vehicleGroupSelection(vehicleGroupValue: any) {
    this.vinList = [];
    // console.log("----vehicleGroupList---",this.vehicleGroupList)
    this.vehicleGroupList.forEach(item => {
      // this.vehicleGroupIdsSet.push(item.vehicleGroupId)
      if (item.vehicleGroupId == vehicleGroupValue.value) {
        this.vinList.push(item.vin)
      }
    });
  }

  masterToggleForCorridor() {
    this.markerArray = [];
    if (this.isAllSelectedForCorridor()) {
      this.selectedCorridors.clear();
      this.showMap = false;
    }
    else {
      this.dataSource.data.forEach((row) => {
        this.selectedCorridors.select(row);
        this.markerArray.push(row);
      });
      this.showMap = true;
    }
    // this.addPolylineToMap();
  }

  isAllSelectedForCorridor() {
    const numSelected = this.selectedCorridors.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForCorridor(row?: any): string {
    if (row)
      return `${this.isAllSelectedForCorridor() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedCorridors.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  checkboxClicked(event: any, row: any) {
    this.showMap = this.selectedCorridors.selected.length > 0 ? true : false;
    //console.log(this.selectedpois.selected.length)
    //console.log(row);
    if (event.checked) { //-- add new marker
      this.markerArray.push(row);
    } else { //-- remove existing marker
      //It will filter out checked points only
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
    }
    // this.addPolylineToMap();
  }

  updatedTableData(tableData: any) {
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getNewTagData(data: any) {
    let currentDate = new Date().getTime();
    if (data.length > 0) {
      data.forEach(row => {
        let createdDate = parseInt(row.createdAt);
        let nextDate = createdDate + 86400000;
        if (currentDate > createdDate && currentDate < nextDate) {
          row.newTag = true;
        }
        else {
          row.newTag = false;
        }
      });
      let newTrueData = data.filter(item => item.newTag == true);
      newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
      let newFalseData = data.filter(item => item.newTag == false);
      Array.prototype.push.apply(newTrueData, newFalseData);
      return newTrueData;
    }
    else {
      return data;
    }
  }
  timeChanged(selectedTime: any) {
    this.selectedStartTime = selectedTime;
  }
  endtimeChanged(endTime: any) {
    this.selectedEndTime = endTime;
  }

  vinSelection(vinSelectedValue: any) {
    this.vinListSelectedValue = vinSelectedValue;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }
}

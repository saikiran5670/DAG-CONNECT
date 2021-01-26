import { SelectionModel } from '@angular/cdk/collections';
import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { EmployeeService } from 'src/app/services/employee.service';

@Component({
  selector: 'app-create-edit-vehicle-details',
  templateUrl: './create-edit-vehicle-details.component.html',
  styleUrls: ['./create-edit-vehicle-details.component.less'],
})
export class CreateEditVehicleDetailsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() gridData: any;
  @Input() title: string;
  @Input() createStatus: boolean;
  @Input() translationData: any;
  @Input() groupInfo: any;
  vehicleFormGroup: FormGroup;
  displayColumnHeaders: string[] = [
    'select',
    'Vehicle name',
    'VIN',
    'Registration Number',
    'Model',
  ];
  displayedColumns: string[] = [
    'select',
    'name',
    'vin',
    'license_Plate_Number',
    'model',
  ];
  dataSource: any;
  selectionForRole = new SelectionModel(true, []);
  selectionForVehGrp = new SelectionModel(true, []);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  vehGrpName: string = '';

  constructor(
    private _formBuilder: FormBuilder,
    private userService: EmployeeService
  ) {}

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnInit() {
    this.vehicleFormGroup = this._formBuilder.group({
      vehicleGroupName: ['', [Validators.required]],
      vehicleGroupDescription: [],
    });
    //console.log(this.groupInfo );
    this.vehGrpName = this.groupInfo !== null ? this.groupInfo.name : '';
    this.dataSource = new MatTableDataSource(this.gridData);
  }

  onCancel() {
    this.backToPage.emit({ editFlag: false, editText: 'cancel' });
  }
  onReset(){
    this.vehicleFormGroup.reset();
  }
  getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}
  onCreate() {
    if (this.createStatus) {
      // create func
      let objData = {
        vehicleGroupID: this.getRandomInt(1, 500),
        organizationID: 1,
        name: this.vehicleFormGroup.controls.vehicleGroupName.value,
        parentID: 0,
        isActive: true,
        createdDate: '2020-12-14T06:19:33.111Z',
        createdBy: 0,
        updatedDate: '2020-12-14T06:19:33.111Z',
        updatedBy: 0,
        isDefaultGroup: true,
        isUserDefindGroup: true,
        vin: '-',
        registrationNumber: '-',
        model: '-',
        Status:"assets/images/john.png",
        isGroup: true,
        vehicles: [
          {
            vehicleID: 0,
            vin: 'VIN456',
            registrationNo: 'NL J-229-FV',
            chassisNo: null,
            terminationDate: '2020-12-14T06:19:33.111Z',
            isActive: true,
            createdDate: '2020-12-14T06:19:33.111Z',
            createdBy: 0,
            updatedDate: '2020-12-14T06:19:33.111Z',
            updatedBy: 0,
          },
        ],
        vehicleOrgIds: 'string',
      };
      // let objData = {
      //   name: this.vehicleFormGroup.controls.vehicleGroupName.value
      // }
      this.userService.createVehicleGroup(objData).subscribe(
        (res) => {
          this.userService.getVehicleGroupByID().subscribe(
            (data) => {
              this.backToPage.emit({
                editFlag: false,
                editText: 'create',
                gridData: data,
              });
            },
            (error) => {}
          );
        },
        (error) => {}
      );
    } else {
      // edit func

      let objData = {
        id:this.groupInfo.id,
        vehicleGroupID: this.gridData[0].vehicleGroupID,
        organizationID: 1,
        name: this.vehicleFormGroup.controls.vehicleGroupName.value,
        parentID: 0,
        isActive: true,
        createdDate: '2020-12-14T06:19:33.111Z',
        createdBy: 0,
        updatedDate: '2020-12-14T06:19:33.111Z',
        updatedBy: 0,
        isDefaultGroup: true,
        isUserDefindGroup: true,
        vin: "-",
        registrationNumber: "-",
        model: "-",
        Status:"assets/images/john.png",
        isGroup: true,
        vehicles: [
          {
            vehicleID: 0,
            vin: null,
            registrationNo: null,
            chassisNo: null,
            terminationDate: '2020-12-14T06:19:33.111Z',
            isActive: true,
            createdDate: '2020-12-14T06:19:33.111Z',
            createdBy: 0,
            updatedDate: '2020-12-14T06:19:33.111Z',
            updatedBy: 0,
          },
        ],
        vehicleOrgIds: 'string',
      };
          // let objData = {
      //   vehicleGroupID: this.gridData[0].vehicleGroupID,
      //   name: this.vehicleFormGroup.controls.vehicleGroupName.value
      // }
      this.userService.updateVehicleGroup(objData).subscribe(
        (res) => {
          this.userService.getVehicleGroupByID().subscribe(
            (data) => {
              this.backToPage.emit({
                editFlag: false,
                editText: 'edit',
                gridData: data,
              });
            },
            (error) => {}
          );
        },
        (error) => {}
      );
    }
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  masterToggleForVehGrp() {
    this.isAllSelectedForVehGrp()
      ? this.selectionForVehGrp.clear()
      : this.dataSource.data.forEach((row) =>
          this.selectionForVehGrp.select(row)
        );
  }

  isAllSelectedForVehGrp() {
    const numSelected = this.selectionForVehGrp.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForVehGrp(row?): string {
    if (row)
      return `${this.isAllSelectedForVehGrp() ? 'select' : 'deselect'} all`;
    else
      return `${
        this.selectionForVehGrp.isSelected(row) ? 'deselect' : 'select'
      } row`;
  }
}

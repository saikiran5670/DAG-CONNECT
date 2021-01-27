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
import { vehicleUpdateRequest } from 'src/app/models/vehicle.model';
import { VehicleService } from 'src/app/services/vehicle.service';

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
  @Input() viewGroupMode: boolean;
  vehicleFormGroup: FormGroup;
  orgId: number;
  displayColumnHeaders: string[] = [
    'All',
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
    private vehService: VehicleService
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
    //console.log(this.groupInfo);
    this.vehGrpName = this.groupInfo ? this.groupInfo.name : '';

    this.dataSource = new MatTableDataSource(this.gridData);
    if (localStorage.getItem('accountOrganizationId') != null) {
      this.orgId = parseInt(localStorage.getItem('accountOrganizationId'));
    }

    if (this.viewGroupMode) {
      this.vehicleFormGroup.get('vehicleGroupName').disable();
      this.vehicleFormGroup.get('vehicleGroupDescription').disable();
    }
    //select associated vehicles of the vehicle group.
    this.selectCheckBox(this.groupInfo.id);
  }

  onCancel() {
    this.backToPage.emit({ editFlag: false, editText: 'cancel' });
  }
  onReset() {
    this.vehicleFormGroup.reset();
  }
  selectCheckBox(vehGroupId) {
    this.vehService.getVehicleListById(vehGroupId).subscribe((req) => {
      this.dataSource.data.forEach((row) => {
        let search = req.filter((item) => item.id === row.id);
        if (search.length > 0) {
          this.selectionForVehGrp.select(row);
          
        }
      });
    });
  }
  onCreate() {
    if (this.createStatus) {
      // create func

      let objData = {
        id: 0,
        name: this.vehicleFormGroup.controls.vehicleGroupName.value,
        description: this.vehicleFormGroup.controls.vehicleGroupDescription
          .value,
        organizationId: this.orgId ? this.orgId : 1,
        vehicles: [
          {
            vehicleGroupId: 0,
            vehicleId: 0,
          },
        ],
      };

      this.vehService.createVehicleGroup(objData).subscribe(
        (res) => {
          // this.vehService.getVehicleGroupByID().subscribe(
          //   (data) => {
          this.backToPage.emit({
            editFlag: false,
            editText: 'create',
            //gridData: data,
          });
          //   },
          //   (error) => {}
          // );
        },
        (error) => {}
      );
    } else {
      // edit func

      let objData: vehicleUpdateRequest = {
        id: this.groupInfo.id,
        name: this.vehicleFormGroup.controls.vehicleGroupName.value,
        description: this.vehicleFormGroup.controls.vehicleGroupDescription
          .value,
        organizationId: this.orgId ? this.orgId : 1,
        vehicles: [
          {
            vehicleGroupId: this.groupInfo.id,
            vehicleId: 0,
          },
        ],
      };

      this.vehService.updateVehicleGroup(objData).subscribe(
        (res) => {
          this.backToPage.emit({
            editFlag: false,
            editText: 'edit',
            //gridData: data,
          });
        },
        (error) => {
          console.error(error);
        }
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

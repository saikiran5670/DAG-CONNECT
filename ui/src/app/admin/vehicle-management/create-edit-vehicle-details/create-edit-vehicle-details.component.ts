import { SelectionModel } from '@angular/cdk/collections';
import { isNull } from '@angular/compiler/src/output/output_ast';
import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { vehicleUpdateRequest } from 'src/app/models/vehicle.model';
import { VehicleService } from 'src/app/services/vehicle.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-edit-vehicle-details',
  templateUrl: './create-edit-vehicle-details.component.html',
  styleUrls: ['./create-edit-vehicle-details.component.less'],
})
export class CreateEditVehicleDetailsComponent implements OnInit {
  breadcumMsg: any = '';
  @ViewChild("createVehicleForm",{ static: true })
  public createVehicleForm: NgForm;
  @Output() backToPage = new EventEmitter<any>();
  @Input() gridData: any;
  @Input() title: string;
  @Input() createStatus: boolean;
  @Input() translationData: any;
  @Input() groupInfo: any;
  @Input() viewGroupMode: boolean;
  vehicleFormGroup: FormGroup;
  orgId: number;duplicateMsg:boolean;
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
      vehicleGroupName: ['', [Validators.required,CustomValidators.noWhitespaceValidator]],
      vehicleGroupDescription: ['',[CustomValidators.noWhitespaceValidatorforDesc]],
    });
    //console.log(this.groupInfo);

    this.dataSource = new MatTableDataSource(this.gridData);
    if (localStorage.getItem('accountOrganizationId') != null) {
      this.orgId = parseInt(localStorage.getItem('accountOrganizationId'));
    }
    //disable control in view mode
    if (this.viewGroupMode) {
      this.vehicleFormGroup.get('vehicleGroupName').disable();
      this.vehicleFormGroup.get('vehicleGroupDescription').disable();
    }
    //select associated vehicles of the vehicle group.
    if (this.groupInfo) {
      this.vehGrpName = this.groupInfo ? this.groupInfo.name : '';
    this.vehicleFormGroup.controls.vehicleGroupDescription.setValue(
      this.groupInfo.description ? this.groupInfo.description : ''
    );
      this.selectCheckBox(this.groupInfo.id,this.viewGroupMode);
    }
    this.breadcumMsg = this.getBreadcum();
  }

  getBreadcum(){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} / ${this.translationData.lblVehicleManagement ? this.translationData.lblVehicleManagement : "Vehicle Management"} / ${this.translationData.lblVehicleGroupDetails ? this.translationData.lblVehicleGroupDetails : 'Vehicle Group Details'}`;
  }

  onCancel() {
    this.backToPage.emit({ editFlag: false, editText: 'cancel' });
  }

  onReset() {
    //this.vehicleFormGroup.reset();
    if (this.groupInfo) {
      this.vehGrpName = this.groupInfo ? this.groupInfo.name : '';
      this.vehicleFormGroup.controls.vehicleGroupDescription.setValue(
        this.groupInfo.description ? this.groupInfo.description : ''
      );
    } else {
      this.vehicleFormGroup.get('vehicleGroupName').setValue('');
      this.vehicleFormGroup.controls.vehicleGroupDescription.setValue('');
    }
  }
  selectCheckBox(vehGroupId,viewGroupMode) {
    this.vehService.getVehicleListById(vehGroupId).subscribe((req) => {
      this.dataSource.data.forEach((row) => {
        let search = req.filter((item) => item.id === row.id);
        if (search.length > 0) {
          this.selectionForVehGrp.select(row);
        }
      });
      if(viewGroupMode){
        this.displayedColumns= [
          'name',
          'vin',
          'license_Plate_Number',
          'model',
        ];
        this.displayColumnHeaders = [
          'Vehicle name',
          'VIN',
          'Registration Number',
          'Model',
        ];
        this.dataSource = new MatTableDataSource(this.selectionForVehGrp.selected);
      }

    });
  }
  onCreate() {
    this.duplicateMsg = false;
    if (this.createStatus) {
      // create func
      let objData = {
        id: 0,
        name: this.vehicleFormGroup.controls.vehicleGroupName.value.trim(),
        description: this.vehicleFormGroup.controls.vehicleGroupDescription
          .value.trim(),
        organizationId: this.orgId ? this.orgId : 1,
        vehicles: [],
      };
      //select all vehicles which are selected for vehicle group.
      const numSelected = this.selectionForVehGrp.selected;

      numSelected.forEach((row) => {
        // console.log(row.id);
        objData.vehicles.push({
          vehicleGroupId: 0,
          vehicleId: row.id,
        });
      });
      //console.log(objData);

      this.vehService.createVehicleGroup(JSON.stringify(objData)).subscribe(
        (res) => {
          this.backToPage.emit({
            editFlag: false,
            editText: 'create',
            gridData: objData,
          });
        },
        (error) => {
          console.error(error);
          if(error.status == 409){
            this.duplicateMsg = true;
          }
        }
      );
    } else {
      // edit function here
      let objDataUpdate = {
        //vehicleUpdateRequest
        id: this.groupInfo.id?parseInt(this.groupInfo.id):0,
        name: this.vehicleFormGroup.controls.vehicleGroupName.value.trim(),
        description: this.vehicleFormGroup.controls.vehicleGroupDescription
          .value.trim(),
        organizationId: this.orgId ? this.orgId : 1,
        vehicles: [],
      };
      //select all vehicles which are selected for vehicle group.
      const numSelected = this.selectionForVehGrp.selected;

      numSelected.forEach((row) => {
        objDataUpdate.vehicles.push({
          vehicleGroupId: parseInt(this.groupInfo.id),
          vehicleId: parseInt(row.id),
        });
      });

     // console.log(objDataUpdate);

      this.vehService.updateVehicleGroup(objDataUpdate).subscribe(
        (res) => {
          this.backToPage.emit({
            editFlag: false,
            editText: 'edit',
            gridData: objDataUpdate,
          });
        },
        (error) => {
          console.error(error);
          if(error.status == 409){
            this.duplicateMsg = true;
          }
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

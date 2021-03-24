import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-create-edit-view-vehicle-group',
  templateUrl: './create-edit-view-vehicle-group.component.html',
  styleUrls: ['./create-edit-view-vehicle-group.component.less']
})

export class CreateEditViewVehicleGroupComponent implements OnInit {
  accountOrganizationId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  displayedColumns: string[] = ['select', 'name', 'vin', 'licensePlateNumber', 'modelId'];
  selectedVehicles = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Input() vehicleListData: any;
  vehGroupTypeList: any = [];
  vehicleGroupForm: FormGroup;
  breadcumMsg: any = '';

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() { 
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.vehicleGroupForm = this._formBuilder.group({
      vehicleGroupName: ['', [Validators.required]],
      vehicleGroupType: ['', [Validators.required]],
      vehicleGroupDescription: []
    });
    this.vehGroupTypeList = [
      {
        name: this.translationData.lblGroup || 'Group',
        value: 'G'
      },
      {
        name: this.translationData.lblDynamic || 'Dynamic',
        value: 'D'
      }
    ];
    this.breadcumMsg = this.getBreadcum();
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblVehicleGroupManagement ? this.translationData.lblVehicleGroupManagement : "Vehicle Group Management"} / ${this.translationData.lblVehicleGroupDetails ? this.translationData.lblVehicleGroupDetails : 'Vehicle Group Details'}`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

}
import { Component, OnInit, Input, ViewChildren, QueryList, ViewChild, EventEmitter, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatAccordion } from '@angular/material/expansion';

@Component({
  selector: 'app-confirm-step',
  templateUrl: './confirm-step.component.html',
  styleUrls: ['./confirm-step.component.less']
})
export class ConfirmStepComponent implements OnInit {
  @Input() translationData: any;
  @Input() editFlag: any;
  @Output() grpCreate = new EventEmitter<boolean>();
  @Input() set selectedRoleData(value) {
    this.confirmRoleData = value;
    this.loadRoleData();
  }

  @Input() set selectedVehGrpData(value) {
    this.confirmVehGrpData = value;
    this.loadVehGrpData();
  }
  
  @ViewChild(MatAccordion) accordion: MatAccordion;
  confirmRoleData: any = [];
  confirmVehGrpData: any = [];
  //@Input() selectedRoleData: any;
  //@Input() selectedVehGrpData: any;
  displayedColumnsRoleConfirm: string[] = [ 'name', 'services'];
  displayedColumnsVehGrpConfirm: string[] = [ 'name', 'vehicles', 'registrationNumber' ];
  selectedRoleDataSource: any = [];
  selectedVehGrpDataSource: any = [];
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  constructor() { }
  
  ngAfterViewInit() {}
  
  ngOnInit() {
    //console.log("translationData:: ", this.translationData)
  }

  loadRoleData(){
    this.selectedRoleDataSource = new MatTableDataSource(this.confirmRoleData);
    setTimeout(()=>{                     
      this.selectedRoleDataSource.paginator = this.paginator.toArray()[0];
      this.selectedRoleDataSource.sort = this.sort.toArray()[0];
      this.accordion.openAll();
    });
  }

  loadVehGrpData(){
    this.selectedVehGrpDataSource = new MatTableDataSource(this.confirmVehGrpData);
    setTimeout(()=>{                     
      this.selectedVehGrpDataSource.paginator = this.paginator.toArray()[1];
      this.selectedVehGrpDataSource.sort = this.sort.toArray()[1];
      this.accordion.openAll();
    });
  }

  onUserRoleEdit(){

  }

  onVehicleGroupEdit(){

  }

  onCancel(){
    this.grpCreate.emit(false);
  }

}

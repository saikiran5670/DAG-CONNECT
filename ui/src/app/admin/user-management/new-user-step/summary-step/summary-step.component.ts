import { Component, OnInit, Input, ViewChildren, QueryList, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-summary-step',
  templateUrl: './summary-step.component.html',
  styleUrls: ['./summary-step.component.less']
})
export class SummaryStepComponent implements OnInit {
  @Input() translationData: any;

  @Input() set selectedRoleData(value) {
    this.confirmRoleData = value;
    this.loadRoleData();
  }

  @Input() set selectedUserGrpData(value) {
    this.confirmUserGrpData = value;
    this.loadUserGrpData();
  }

  @Input() set selectedVehGrpData(value) {
    this.confirmVehGrpData = value;
    this.loadVehGrpData();
  }

  @Input() set accountInfoData(value){
    this.confirmAccountInfoData = value;
  }
  
  @Input() isCreateFlag: boolean = true;
  @Input() profilePath: any;
  
  confirmAccountInfoData: any = [];
  confirmRoleData: any = [];
  confirmUserGrpData: any = [];
  confirmVehGrpData: any = [];

  displayedColumnsRoleConfirm: string[] = ['name', 'services'];
  //displayedColumnsUserGrpConfirm: string[] = ['name', 'vehicles', 'users'];
  displayedColumnsUserGrpConfirm: string[] = ['name', 'users'];
  displayedColumnsVehGrpConfirm: string[] = ['name', 'vehicles', 'registrationNumber'];
  selectedRoleDataSource: any = [];
  selecteUserGrpDataSource: any = [];
  selectedVehGrpDataSource: any = [];

  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  constructor() { }

  ngOnInit() { 
    if(this.profilePath == ''){
      this.profilePath='../../assets/images/Account_pic.png';    
    }
  }
  
  loadRoleData(){
    this.selectedRoleDataSource = new MatTableDataSource(this.confirmRoleData);
    setTimeout(()=>{                     
      this.selectedRoleDataSource.paginator = this.paginator.toArray()[0];
      this.selectedRoleDataSource.sort = this.sort.toArray()[0];
    });
  }

  loadUserGrpData(){
    this.selecteUserGrpDataSource = new MatTableDataSource(this.confirmUserGrpData);
    setTimeout(()=>{                     
      this.selecteUserGrpDataSource.paginator = this.paginator.toArray()[1];
      this.selecteUserGrpDataSource.sort = this.sort.toArray()[1];
    });
  }

  loadVehGrpData(){
    this.selectedVehGrpDataSource = new MatTableDataSource(this.confirmVehGrpData);
    setTimeout(()=>{                     
      this.selectedVehGrpDataSource.paginator = this.paginator.toArray()[2];
      this.selectedVehGrpDataSource.sort = this.sort.toArray()[2];
    });
  }
}

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { AccountGroup } from 'src/app/models/users.model';
import { AccountService } from 'src/app/services/account.service';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-common-filter',
  templateUrl: './common-filter.component.html',
  styleUrls: ['./common-filter.component.less']
})
export class CommonFilterComponent implements OnInit {
  OrgId = parseInt(localStorage.getItem("accountOrganizationId"));
  isGlobal: boolean = true;
  @Input() initData : any;
  @Input() translationData: any;
  userGroups : any = [];
  roles : any = [];
  name = '';
  userGroup = 'All';
  role = 'All';
  dataSource : any;
  localData : any;
  @Output() filterValues : EventEmitter<any> = new EventEmitter();
  accountgrp: AccountGroup = {
    // id: 1  ,
    // name: '',
    // description: '',
    accountGroupId : 0,
    organizationId : this.OrgId,
    accountId : 0,
    accounts : true,
    accountCount : true,
  }

  roleObj = { 
    Organizationid : this.OrgId,
    IsGlobal: this.isGlobal
 };

  constructor(
    private accountService: AccountService,
    private roleService: RoleService
  ) { }

  ngOnInit(): void {
    this.localData = this.initData;
    this.dataSource = new MatTableDataSource(this.initData);
    // for(let i = 0; i < this.initData.length; i++){
    //   this.userGroups.push(this.initData[i].userGroup);
    //   this.roles.push(this.initData[i].role);
    // }

    this.accountService.getAccountGroupDetails(this.accountgrp).subscribe((grpData)=>{
      grpData.forEach(item => {
        this.userGroups.push(item.name);
      })
    });

    this.roleService.getUserRoles(this.roleObj).subscribe((roleData) => {
     roleData.forEach(item => {
       this.roles.push(item.roleName);
     })
    });
  

    this.dataSource.filterPredicate = ((data, filter) => {
      let found:boolean=true;
      let searchTerms = JSON.parse(filter);
      
      if(searchTerms.firstName){
        found = data.firstName.toLowerCase().includes(searchTerms.firstName);
      }

      if(searchTerms.userGroup){
        if(data.accountGroups.length != 0){
          let accountGroupName = '';
          for(let i = 0; i < data.accountGroups.length; i++){
            accountGroupName = data.accountGroups[i].name.toLowerCase();
            if(accountGroupName.includes(searchTerms.userGroup)){
              found = found && accountGroupName.includes(searchTerms.userGroup);
              break;
            }
            else{
              found = found && accountGroupName.includes(searchTerms.userGroup);
            }
          }
        }
        else{
          found = false;
        }
      }

      if(searchTerms.role){
        if(data.roles.length != 0){
          let roleName = '';
          for(let i = 0; i < data.roles.length; i++){
            roleName = data.roles[i].name.toLowerCase();
            if(roleName.includes(searchTerms.role)){
              found = found && roleName.includes(searchTerms.role);
              break;
            }
            else{
              found = found && roleName.includes(searchTerms.role);
            }
          }
        }
        else{
          found = false;
        }
      }
      return found;
    }) as (PeriodicElement, string) => boolean;
    
  }

  filter(){
    let filterTerms = {};
    if(this.name != ''){
      filterTerms['firstName'] = this.name.trim().toLowerCase();
    }
    if(this.userGroup != 'All'){
      filterTerms['userGroup'] = this.userGroup.trim().toLowerCase();
    }
    if(this.role != 'All'){
      filterTerms['role'] = this.role.trim().toLowerCase();
    }
    this.dataSource.filter = JSON.stringify(filterTerms);
    this.filterValues.emit(this.dataSource);
  }
  
  reset(){
    this.name="";
    this.userGroup = 'All';
    this.role = 'All';
    this.filter();
  }

}

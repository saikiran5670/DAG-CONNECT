import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ReplaySubject } from 'rxjs';
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
  @Input() translationData: any = {};
  userGroups : any = [];
  roles : any = [];
  name = '';
  userGroup = 'All';
  role = 'All';
  dataSource : any;
  localData : any;
  @Output() filterValues : EventEmitter<any> = new EventEmitter();
  accountgrp: AccountGroup = {
    accountId: 0,
    organizationId: this.OrgId,
    accountGroupId: 0,
    vehicleGroupId: 0,
    roleId: 0,
    name: ""
  }

  roleObj = { 
    Organizationid : this.OrgId,
    IsGlobal: this.isGlobal
 };

 public filteredGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

 public filteredRoles: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

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
        this.userGroups.push(item.accountGroupName);
        console.log("user groups", this.userGroups);
        this.userGroups.sort(this.compare);
        this.resetuserGroupsFilter();
      })
    });
  
    this.roleService.getUserRoles(this.roleObj).subscribe((roleData) => {
     roleData.forEach(item => {
       this.roles.push(item.roleName);
       console.log("roles", this.roles);
       this.roles.sort(this.compare);
       this.resetRolesFilter();
     })
    });
  

    this.dataSource.filterPredicate = ((data, filter) => {
      let found:boolean=true;
      let searchTerms = JSON.parse(filter);
      
      if(searchTerms.firstName){
        found = (data.firstName.toLowerCase()+" "+data.lastName.toLowerCase()).includes(searchTerms.firstName);
        if(!found)
          return found;
      }

      if(searchTerms.userGroup){
        if(data.accountGroups.length != 0){
          let accountGroupName = '';
          let foundUserGroup:boolean=false;
          for(let i = 0; i < data.accountGroups.length; i++){
            accountGroupName = data.accountGroups[i].name.toLowerCase();
            if(accountGroupName.includes(searchTerms.userGroup)){
              foundUserGroup = true;
              break;
            }
          }
          if(!foundUserGroup)
            return foundUserGroup;
        }
        else{
          return false;
        }
      }

      if(searchTerms.role){
        let foundRole:boolean=false;
        if(data.roles.length != 0){
          let roleName = '';
          for(let i = 0; i < data.roles.length; i++){
            roleName = data.roles[i].name.toLowerCase();
            if(roleName.includes(searchTerms.role)){
              found = found && roleName.includes(searchTerms.role);
              foundRole = true;
              break;
            }
          }
          if(!foundRole)
            return foundRole;
        }
        else{
          return foundRole;
        }
      }
      return true;
    }) as (PeriodicElement, string) => boolean;
    
  }
  compare(a, b) {
    if (a.name < b.name) {
      return -1;
    }
    if (a.name > b.name) {
      return 1;
    }
    return 0;
  }
  resetuserGroupsFilter(){
    this.filteredGroups.next(this.userGroups.slice());
  }
  resetRolesFilter(){
    this.filteredRoles.next(this.roles.slice());
  }


  filter(){
    let filterTerms = {};
    if(this.name != ''){
      filterTerms['firstName'] = this.name.trim().toLowerCase();
    } else {
      filterTerms['firstName'] = '';
    }
    if(this.userGroup != 'All'){
      filterTerms['userGroup'] = this.userGroup.trim().toLowerCase();
    } else {
      filterTerms['userGroup'] = '';
    }
    if(this.role != 'All'){
      filterTerms['role'] = this.role.trim().toLowerCase();
    } else {
      filterTerms['role'] = '';
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

  filterUserGroups(groupSearch){
    if(!this.userGroups){
      return;
    }
    if(!groupSearch){
      this.resetuserGroupsFilter();
      return;
     } else{
      groupSearch = groupSearch.toLowerCase();
     }
     this.filteredGroups.next(
       this.userGroups.filter(item=> item.toLowerCase().indexOf(groupSearch) > -1)
     );

  }
 
  filterRoles(roleSearch){
    if(!this.roles){
      return;
    }
    if(!roleSearch){
      this.resetRolesFilter();
      return;
     } else{
      roleSearch = roleSearch.toLowerCase();
     }
     this.filteredRoles.next(
       this.roles.filter(item=> item.toLowerCase().indexOf(roleSearch) > -1)
     );

  }

  


}

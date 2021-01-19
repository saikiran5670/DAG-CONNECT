import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-common-filter',
  templateUrl: './common-filter.component.html',
  styleUrls: ['./common-filter.component.less']
})
export class CommonFilterComponent implements OnInit {

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

  constructor() { }

  ngOnInit(): void {
    this.localData = this.initData;
    this.dataSource = new MatTableDataSource(this.initData);
    for(let i = 0; i < this.initData.length; i++){
      this.userGroups.push(this.initData[i].userGroup);
      this.roles.push(this.initData[i].role);
    }
  
    this.dataSource.filterPredicate = ((data, filter) => {
      let found:boolean=true;
      let searchTerms = JSON.parse(filter);
      
      if(searchTerms.firstName){
        found = data.firstName.toLowerCase().includes(searchTerms.firstName);
      }

      if(searchTerms.userGroup){
        found = found && data.userGroup.toLowerCase().includes(searchTerms.userGroup);
      }

      if(searchTerms.role){
        found = found && data.role.toLowerCase().includes(searchTerms.role);
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

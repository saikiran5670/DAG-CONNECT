import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filter-vehicle',
  templateUrl: './fleet-overview-filter-vehicle.component.html',
  styleUrls: ['./fleet-overview-filter-vehicle.component.less']
})
export class FleetOverviewFilterVehicleComponent implements OnInit {
  @Input() translationData: any;
  groupList : any= [
{
  id:'1',
  val:'test1 group'
},
{
  id:'2',
  val:'test2 group'
}
]

categoryList : any= [
  {
    id:'1',
    val:'category1'
  },
  {
    id:'2',
    val:'category12'
  }
]

levelList : any= [
  {
    id:'1',
    val:'level1'
  },
  {
    id:'2',
    val:'level2'
  }
]

healthList : any= [
  {
    id:'1',
    val:'Health Status1'
  },
  {
    id:'2',
    val:'Health Status1'
  }
]

otherList : any= [
  {
    id:'1',
    val:'Other Filter1'
  },
  {
    id:'2',
    val:'Other Filter2'
  }
]


  constructor() { }

  ngOnInit(): void {
  }

  applyFilter(filterValue: string) {
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeGroup(event: any){
    
  }

  onChangeLevel(event: any){

  }

  onChangeCategory(event: any){
    
  }

  onChangHealthStatus(event: any){
    
  }

  onChangeOtherFilter(event: any){
    
  }
  
}

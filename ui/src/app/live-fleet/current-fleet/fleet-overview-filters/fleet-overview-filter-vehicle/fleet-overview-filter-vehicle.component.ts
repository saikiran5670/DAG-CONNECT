import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-fleet-overview-filter-vehicle',
  templateUrl: './fleet-overview-filter-vehicle.component.html',
  styleUrls: ['./fleet-overview-filter-vehicle.component.less']
})
export class FleetOverviewFilterVehicleComponent implements OnInit {
  @Input() translationData: any;
  @Input() detailsData: any;
 filterData: any;
  filterVehicle:FormGroup;
  isVehicleListOpen: boolean = true;
groupList : any= [];
categoryList : any= [];
levelList : any= [];
healthList : any= [];
otherList : any= [];


  constructor(private _formBuilder: FormBuilder, private reportService: ReportService) { }

  ngOnInit(): void {
    this.filterVehicle = this._formBuilder.group({
      group: [''],
      level: [''],
      category: [''],
      status: [''],
      otherFilter: ['']
    })

    this.reportService.getFilterDetails().subscribe((data: any) => {
console.log("service data=" +data);
this.filterData = data;
this.filterData["vehicleGroups"].forEach(item=>
this.groupList.push(item) );
this.filterData["alertCategory"].forEach(item=>
this.categoryList.push(item) );
this.filterData["alertLevel"].forEach(item=>
this.levelList.push(item) );
this.filterData["healthStatus"].forEach(item=>
this.healthList.push(item) );
this.filterData["otherFilter"].forEach(item=>
this.otherList.push(item) );

    });
  }

  applyFilter(filterValue: string) {
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeGroup(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeLevel(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeCategory(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangHealthStatus(event: any){
    // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }

  onChangeOtherFilter(event: any){
        // filterValue = filterValue.trim();
    // filterValue = filterValue.toLowerCase();
    // this.dataSource.filter = filterValue;
  }
  
}

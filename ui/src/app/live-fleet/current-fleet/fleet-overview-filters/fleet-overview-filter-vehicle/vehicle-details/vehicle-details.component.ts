import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-vehicle-details',
  templateUrl: './vehicle-details.component.html',
  styleUrls: ['./vehicle-details.component.less']
})
export class VehicleDetailsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() selectedElementData: any;
  @Input() translationData: any;
  @Input() levelList: any;
  @Input() categoryList: any;
  gridData: any = [];
  constructor(private router: Router, private dataInterchangeService: DataInterchangeService,private reportService: ReportService) { }

  ngOnInit(): void {
    this.gridData = this.selectedElementData;
    // this.reportService.getFilterDetails().subscribe((data: any) => {
    //   this.filterData = data;
    //   this.levelList = [];
    // });
    this.selectedElementData.fleetOverviewAlert.forEach(item => {
      this.levelList.forEach(element => {
        if(item.level ==element.value)
        {         
         item.level = element.name;
        }
       });              
    }); 
    this.selectedElementData.fleetOverviewAlert.forEach(item => {
      this.categoryList.forEach(element => {
        if(item.categoryType ==element.value)
        {         
         item.categoryType = element.name;
        }
       });              
    }); 
  }

  timeConversion(time: any){
    var d = new Date(time);
    return d.toLocaleString();
  }

  toBack() {
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.backToPage.emit(emitObj);
  }

  gotoHealthStatus(){
    this.dataInterchangeService.gethealthDetails(this.selectedElementData);
  }

  gotoLogBook(){
    const navigationExtras: NavigationExtras = {
      state: {
        fromVehicleDetails: true,
        
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }

}

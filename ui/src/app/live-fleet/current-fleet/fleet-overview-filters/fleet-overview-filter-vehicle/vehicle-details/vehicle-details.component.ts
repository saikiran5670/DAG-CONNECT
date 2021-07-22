import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';

@Component({
  selector: 'app-vehicle-details',
  templateUrl: './vehicle-details.component.html',
  styleUrls: ['./vehicle-details.component.less']
})
export class VehicleDetailsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() selectedElementData: any;
  @Input() translationData: any;
  gridData: any = [];
  constructor(private router: Router, private dataInterchangeService: DataInterchangeService) { }

  ngOnInit(): void {
    this.gridData = this.selectedElementData;
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

  gotoHealthStatus(data: any){
    // const navigationExtras: NavigationExtras = {
    //   state: {
    //     fromVehicleDetails: true,
    //     vehicleDetails: data
    //   }
    // };
    // this.router.navigate(['fleetoverview/livefleet'], navigationExtras);
    this.dataInterchangeService.gethealthDetails(true);
  }

}

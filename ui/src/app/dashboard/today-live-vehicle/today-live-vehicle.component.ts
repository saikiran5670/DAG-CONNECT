import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-today-live-vehicle',
  templateUrl: './today-live-vehicle.component.html',
  styleUrls: ['./today-live-vehicle.component.less']
})
export class TodayLiveVehicleComponent implements OnInit {
  @Input() translationData : any;
  constructor(private router : Router) { }

  ngOnInit(): void {
  }

  navigateToReport(){
    this.router.navigate(['/report/fleetutilisation']);
  }

}

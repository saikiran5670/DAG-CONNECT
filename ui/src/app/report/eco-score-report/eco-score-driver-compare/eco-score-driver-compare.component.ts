


import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-eco-score-driver-compare',
  templateUrl: './eco-score-driver-compare.component.html',
  styleUrls: ['./eco-score-driver-compare.component.css']
})
export class EcoScoreDriverCompareComponent implements OnInit {
  @Input() compareEcoScore: any;
  @Output() backToMainPage = new EventEmitter<any>();
  generalExpandPanel: boolean = true;

  inputColumns = ['driverName', 'driverId', 'averageGrossWeight', 'distance','noOfTrips','noOfVehicles','avgDist'];
  inputData = [
    {position: 1, driverName: 'Driver 1',
    driverId: 'SK 2236526558846039',
    averageGrossWeight: 16.2,
    distance: 272.10,
    noOfTrips: 6,
    noOfVehicles: 1,
    avgDist: 136.05},
    {position: 2, driverName: 'Driver 2',
    driverId: 'SK 2236526558846039',
    averageGrossWeight: 16.2,
    distance: 272.10,
    noOfTrips: 6,
    noOfVehicles: 1,
    avgDist: 136.05},
    {position: 3, driverName: 'Driver 3', driverId: 'B B119626533456003',
    averageGrossWeight: 16.2,
    distance: 43.2,
    noOfTrips: 3,
    noOfVehicles: 1,
    avgDist: 84.05},
    {position: 4, driverName: 'Driver 4', driverId: 'B B119626533456003',
    averageGrossWeight: 16.2,
    distance: 43.2,
    noOfTrips: 3,
    noOfVehicles: 1,
    avgDist: 84.05}
  ];;

  displayColumns: string[];
  displayData: any[];
  showTable: boolean;

  
  ngOnInit() {
    this.displayColumns = ['0'].concat(this.inputData.map(x => x.position.toString()));
    this.displayData = this.inputColumns.map(x => this.formatInputRow(x));

    console.log(this.displayColumns);
    console.log(this.displayData);

    this.showTable = true;
  }

  formatInputRow(row) {
    const output = {};
    
    output[0] = row;
    for (let i = 0; i < this.inputData.length; ++i) {
      output[this.inputData[i].position] = this.inputData[i][row];
    }

    return output;
  }

  generalData: any = [
    {
      driverName: 'Driver 1',
      driverId: 'SK 2236526558846039',
      averageGrossWeight: 16.2,
      distance: 272.10,
      noOfTrips: 6,
      noOfVehicles: 1,
      avgDist: 136.05
    },
    {
      driverName: 'Driver 1',
      driverId: 'B B119626533456003',
      averageGrossWeight: 16.2,
      distance: 43.2,
      noOfTrips: 3,
      noOfVehicles: 1,
      avgDist: 84.05
    }
  ]
  items: any = [
    {
      driverName: 'Driver 1',
      driverId: 'SK 2236526558846039',
      averageGrossWeight: 16.2,
      distance: 272.10,
      noOfTrips: 6,
      value: 15,
      percent: 36.05
    },
    {
      driverName: 'Driver 1',
      driverId: 'B B119626533456003',
      averageGrossWeight: 16.2,
      distance: 43.2,
      noOfTrips: 3,
      value: 25,
      percent: 84.05
    }
  ]
  constructor() { }

  // ngOnInit(): void {
  // }
  backToMainPageCall(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToMainPage.emit(emitObj);
  }
}

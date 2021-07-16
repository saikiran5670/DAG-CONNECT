import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-eco-score-report-driver',
  templateUrl: './eco-score-report-driver.component.html',
  styleUrls: ['./eco-score-report-driver.component.css']
})
export class EcoScoreReportDriverComponent implements OnInit {
  @Input() ecoScoreDriverDetails: any;
  @Input() ecoScoreForm: any;
  @Input() translationData: any;
  fromDisplayDate: any;
  toDisplayDate : any;
  selectedVehicleGroup : string;
  selectedVehicle : string;
  selectedDriverId : string;
  selectedDriverName : string;
  selectedDriverOption : string;
  overallPerformancePanel: boolean = true;
  trendlinesPanel: boolean = true;
  generalTablePanel: boolean = true;
  generalChartPanel: boolean = true;
  driverPerformancePanel: boolean = true;
  driverPerformanceChartPanel: boolean = true;

  constructor() { }

  ngOnInit(): void {
    console.log(this.ecoScoreDriverDetails);
    console.log(this.ecoScoreForm);
    console.log(this.translationData);
    console.log(this.ecoScoreForm.get('vehicle').value);
    console.log(this.ecoScoreForm.get('vehicleGroup').value);
    console.log(this.ecoScoreForm.get('driver').value);
    console.log(this.ecoScoreForm.get('startDate').value);
    console.log(this.ecoScoreForm.get('endDate').value);
    console.log(this.ecoScoreForm.get('minTripValue').value);
    console.log(this.ecoScoreForm.get('minDriverValue').value);
    this.fromDisplayDate = this.ecoScoreForm.get('startDate').value;
    this.toDisplayDate = this.ecoScoreForm.get('endDate').value;
    this.selectedVehicleGroup = this.ecoScoreForm.get('vehicleGroup').value;
    this.selectedVehicle = this.ecoScoreForm.get('vehicle').value;
    this.selectedDriverId = this.ecoScoreForm.get('driver').value;
  }

}

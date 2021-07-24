import { EventEmitter, Input, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ReportService } from 'src/app/services/report.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Output } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filter-driver',
  templateUrl: './fleet-overview-filter-driver.component.html',
  styleUrls: ['./fleet-overview-filter-driver.component.less']
})
export class FleetOverviewFilterDriverComponent implements OnInit { 
  @Input() translationData: any;
  groupList : any= []; 
  isVehicleListOpen: boolean = true;
  
  @Input() driverListData: any;
  driverFlagClicked: boolean = true;
  @Input() noRecordFlag: boolean ;
  driverVehicleForm: FormGroup;
  panelOpenState: boolean = false;
  @Output() driverFilterComponentEmit =  new EventEmitter<object>();  
  constructor(private reportService: ReportService,private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    
  }


onChangedriverCheckbox(event){ 
let emitObj = {
  driverFlagClicked  : event.checked
}
 this.driverFilterComponentEmit.emit(emitObj);
  }

}
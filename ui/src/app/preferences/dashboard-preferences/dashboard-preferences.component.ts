import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-dashboard-preferences',
  templateUrl: './dashboard-preferences.component.html',
  styleUrls: ['./dashboard-preferences.component.less']
})

export class DashboardPreferencesComponent implements OnInit {

  @Input() translationData: any = {};
  @Input() reportListData: any;

  //dashboard preferences
  editDashboardFlag: boolean = false;
  displayMessage: any = '';
  updateMsgVisible: boolean = false;
  showDashboardReport: boolean = false;
  reportId: any;
  initData: any = [];
  getDashboardPreferenceResponse: any = [];

  constructor(private reportService: ReportService) {
    this.loadReportData();
   }

  ngOnInit() {
  
    
  }

  upperLowerDD: any = [
    {
      type: 'U',
      name: 'Upper'
    },
    {
      type: 'L',
      name: 'Lower'
    }  
  ];

  donutPieDD: any = [
    {
      type: 'D',
      name: 'Donut Chart'
    },
    {
      type: 'P',
      name: 'Pie Chart'
    }
  ];

  chartSelectionType: any = [
    {
      type: 'B',
      name: 'Bar Chart'
    },
    {
      type: 'L',
      name: 'Line Chart'
    },
    {
      type: 'P',
      name: 'Pie Chart'
    },
    {
      type: 'D',
      name: 'Donut Chart'
    }
  ] 

  onClose(){
    this.updateMsgVisible = false;
  }

  editDashboardPreferences(){
    this.editDashboardFlag = true;
    this.showDashboardReport = false;
  }

  updateEditFleetUtilFlag(retObj: any){
    if(retObj){
      this.editDashboardFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editDashboardFlag = false; // hard coded
    }
  }

  successMsgBlink(msg: any){
    this.updateMsgVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.updateMsgVisible = false;
    }, 5000);
  }

  getSuccessMsg() {
    if (this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  loadReportData(){
   
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
     
      this.reportListData = reportList.reportDetails;
      let repoId: any = this.reportListData.filter(i => i.name == 'Dashboard');
    
      if (repoId.length > 0) {
        this.reportId = repoId[0].id;
      } else {
        this.reportId = 18; 
      }//- hard coded for Dashboard
  
      this.loadDashboardPreferences();
    }, (error)=>{
      console.log('Report not found...', error);
      
      this.reportListData = [];
    });
  }


  loadDashboardPreferences() {

    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData: any) => {
      this.initData = prefData['userPreferences'];
      this.getDashboardPreferenceResponse = this.initData;  
      console.log("dataaaaaaa--->",this.getDashboardPreferenceResponse);  
        
    }, (error) => {
      this.initData = [];
      
    });
  }

}

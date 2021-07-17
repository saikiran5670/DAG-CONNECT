import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-fuel-benchmark-preferences',
  templateUrl: './fuel-benchmark-preferences.component.html',
  styleUrls: ['./fuel-benchmark-preferences.component.less']
})
export class FuelBenchmarkPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setFuelBenchmarkReportFlag = new EventEmitter<any>();
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  reportId: any;
  initData: any = [];
  fuelBenchmarkComponentPrefData: any = [];
  fuelBenchmarkChartsPrefData: any = [];
  selectionForChartsColumns = new SelectionModel(true, []);
  selectionForComponentColumns = new SelectionModel(true, []);

  constructor(private reportService: ReportService, private router: Router) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Fuel Benchmarking');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 6; //- hard coded for Fuel Benchmarking Report
    }
    this.translationUpdate();
    this.loadFuelBenchmarkReportPreferences();
  }

  translationUpdate(){
    this.translationData = {
      rp_fb_report: 'Report',
      rp_fb_chart: 'Chart',
      rp_fb_chart_fuelconsumption: 'Fuel Consumption',
      rp_fb_component: 'Component',
      rp_fb_component_highfuelefficiency: 'High Fuel Efficiency',
      rp_fb_component_lowfuelefficiency: 'Low Fuel Efficiency'
    }
  }

  loadFuelBenchmarkReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.getTranslatedColumnName(this.initData);
    }, (error)=>{
      this.initData = [];
      this.resetColumnData();
    });
  }

  resetColumnData(){
    this.fuelBenchmarkComponentPrefData = [];
    this.fuelBenchmarkChartsPrefData = [];
  }

  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.key.includes('rp_fb_chart_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 17);   
              }
              this.fuelBenchmarkChartsPrefData.push(_data);
            }else if(item.key.includes('rp_fb_component_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 22);   
              }
              this.fuelBenchmarkComponentPrefData.push(_data);
            }
          });
        }
      });
    }
    this.setColumnCheckbox();
  }

  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForChartsColumns.clear();
    this.selectionForComponentColumns.clear();

    this.fuelBenchmarkChartsPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForChartsColumns.select(element);
      }
    });

    this.fuelBenchmarkComponentPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForComponentColumns.select(element);
      }
    });
  }

  onCancel(){
    this.setFuelBenchmarkReportFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm(){
    
  }

}

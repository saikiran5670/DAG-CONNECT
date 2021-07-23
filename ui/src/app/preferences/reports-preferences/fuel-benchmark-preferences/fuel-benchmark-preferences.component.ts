import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

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
  getReportPreferenceResponse: any;
  fuelBenchMarkReportPreference: any = [];
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
  fuelBenchmarkForm: FormGroup;

  constructor(private reportService: ReportService, private _formBuilder: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Fuel Benchmarking');
    this.fuelBenchmarkForm = this._formBuilder.group({
      rp_fb_chart_fuelconsumption: [],
      rp_fb_component_highfuelefficiency: [],
      rp_fb_component_lowfuelefficiency: []
    });

    if (repoId.length > 0) {
      this.reportId = repoId[0].id;
    } else {
      this.reportId = 6; //- hard coded for Fuel Benchmarking Report
    }
    this.translationUpdate();
    this.loadFuelBenchmarkReportPreferences();
  }

  translationUpdate() {
    this.translationData = {
      rp_fb_report: 'Report',
      rp_fb_chart: 'Chart',
      rp_fb_chart_fuelconsumption: 'Fuel Consumption',
      rp_fb_component: 'Component',
      rp_fb_component_highfuelefficiency: 'High Fuel Efficiency',
      rp_fb_component_lowfuelefficiency: 'Low Fuel Efficiency'
    }
  }

  loadFuelBenchmarkReportPreferences() {

    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData: any) => {
      this.initData = prefData['userPreferences'];
      this.getReportPreferenceResponse = this.initData;    
      this.resetColumnData();
      this.getTranslatedColumnName(this.initData);
    }, (error) => {
      this.initData = [];
      this.resetColumnData();
    });
  }


  resetColumnData() {
    this.fuelBenchmarkComponentPrefData = [];
    this.fuelBenchmarkChartsPrefData = [];
  }

  getTranslatedColumnName(prefData: any) {
    if (prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0) {
      prefData.subReportUserPreferences.forEach(element => {
        if (element.subReportUserPreferences && element.subReportUserPreferences.length > 0) {
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if (item.key.includes('rp_fb_chart_')) {
              if (this.translationData[item.key]) {
                _data.translatedName = this.translationData[item.key];
              } else {
                _data.translatedName = this.getName(item.name, 17);
              }
              this.fuelBenchmarkChartsPrefData.push(_data);
            } else if (item.key.includes('rp_fb_component_')) {
              if (this.translationData[item.key]) {
                _data.translatedName = this.translationData[item.key];
              } else {
                _data.translatedName = this.getName(item.name, 22);
              }
              this.fuelBenchmarkComponentPrefData.push(_data);
            }
          });
        }
      });
    }
    if (this.fuelBenchmarkComponentPrefData.length > 0 && this.fuelBenchmarkChartsPrefData.length > 0) {
      this.setDefaultFormValues();
    }
  }

  setDefaultFormValues() {
    for (let field of this.fuelBenchmarkComponentPrefData) {
      this.fuelBenchmarkForm.get([field.key]).setValue(field.thresholdValue != '' ? field.thresholdValue : '0.00');
    }
    this.fuelBenchmarkForm.get('rp_fb_chart_fuelconsumption').setValue(this.fuelBenchmarkChartsPrefData[0].chartType != '' ? this.fuelBenchmarkChartsPrefData[0].chartType : 'D');
  }

  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  onCancel() {
    this.setFuelBenchmarkReportFlag.emit({ flag: false, msg: '' });
    this.setDefaultFormValues();
  }

  onReset() {
    this.setDefaultFormValues();
  }

  onConfirm() {
   
    let temp_attr = [];   
    for (let section of this.getReportPreferenceResponse.subReportUserPreferences) {
      for (let field of section.subReportUserPreferences) {   
        let testObj;
        if (field.key == "rp_fb_chart_fuelconsumption") {
          testObj = {
            dataAttributeId: field.dataAttributeId,
            state: "A",
            preferenceType: "C",
            chartType: this.fuelBenchmarkForm.get([field.key]).value,
            thresholdType: "",
            thresholdValue: 0
          }
        }
        else {
          testObj = {
            dataAttributeId: field.dataAttributeId,
            state: "A",
            preferenceType: "C",
            chartType: field.chartType,
            thresholdType: "",
            thresholdValue: parseInt(this.fuelBenchmarkForm.get([field.key]).value)
            //thresholdValue:4
          }
        }

        temp_attr.push(testObj)
      }

    }
    let benchmarkObject = {
      reportId: this.reportId,
      attributes: temp_attr
    }
  
    this.reportService.updateReportUserPreference(benchmarkObject).subscribe((data: any) => {
      this.setFuelBenchmarkReportFlag.emit({ flag: false, msg: this.getSuccessMsg() });
      setTimeout(() => {
        window.location.reload();
      }, 1000);
    });

  }

  onDonutPieDDChange(event: any) {

  }

  keyPressNumbers(event: any) {
    // var charCode = (event.which) ? event.which : event.keyCode;
    // // Only Numbers 0-9
    // if ((charCode < 48 || charCode > 57)) {
    //   event.preventDefault();
    //   return false;
    // } else {
    //   return true;
    // }
  }

  getSuccessMsg() {
    if (this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

}

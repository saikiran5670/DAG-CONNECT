import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';
import { DataInterchangeService } from '../../../services/data-interchange.service';

@Component({
  selector: 'app-eco-score-report-preferences',
  templateUrl: './eco-score-report-preferences.component.html',
  styleUrls: ['./eco-score-report-preferences.component.less']
})
export class EcoScoreReportPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any = {};
  @Output() setEcoScoreFlag = new EventEmitter<any>();
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  reportId: any;
  initData: any = [];
  generalColumnData: any = [];
  generalGraphColumnData: any = [];
  driverPerformanceColumnData: any = [];
  driverPerformanceGraphColumnData: any = [];
  selectionForGeneralColumns = new SelectionModel(true, []);
  selectionForGeneralGraphColumns = new SelectionModel(true, []);
  selectionForDriverPerformanceColumns = new SelectionModel(true, []);
  selectionForDriverPerformanceGraphColumns = new SelectionModel(true, []);
  mainParent: any = {
    isChecked: false
  }
  requestSent:boolean = false;
  showLoadingIndicator: boolean = false;

  constructor(private reportService: ReportService, private router: Router, private dataInterchangeService: DataInterchangeService) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'EcoScore Report');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
      this.loadEcoScoreReportPreferences();
    }else{
      console.error("No report id found!")
    }
    // this.translationUpdate();
  }

  translationUpdate(){
    this.translationData = {
      rp_ecoscore: 'Eco Score',
      rp_general: 'General',
      rp_distance: 'Distance',
      rp_averagedistanceperday: 'Average Distance Per Day',
      rp_numberofvehicles: 'Number Of Vehicles',
      rp_averagegrossweight: 'Average Gross Weight',
      rp_numberoftrips: 'Number Of Trips',
      rp_generalgraph: 'General Graph',
      rp_piechart: 'Pie Chart',
      rp_bargraph: 'Bar Graph',
      rp_driverperformancegraph: 'Driver Performance Graph',
      rp_driverperformance: 'Driver Performance',
      rp_anticipationscore: 'Anticipation Score (%)',
      rp_fuelconsumption: 'Fuel Consumption (mpg)',
      rp_cruisecontrolusage: 'Cruise Control Usage (%)',
      rp_CruiseControlUsage30: 'Cruise Control Usage 30-50 km/h (%)',
      rp_cruisecontroldistance50: 'Cruise Control Usage 50-75 km/h (%)',
      rp_cruisecontroldistance75: 'Cruise Control Usage >75 km/h (%)',
      rp_heavythrottling: 'Heavy Throttling (%)',
      rp_heavythrottleduration: 'Heavy Throttle Duration (hh:mm:ss)',
      rp_ptousage: 'PTO Usage (%)',
      rp_ptoduration: 'PTO Duration (hh:mm:ss)',
      rp_averagespeed: 'Average Speed (km/h)',
      rp_idleduration: 'Idle duration (hh:mm:ss)',
      rp_averagedrivingspeed: 'Average Driving Speed (km/h)',
      rp_idling: 'Idling (%)',
      rp_brakingscore: 'Braking Score',
      rp_braking: 'Braking (%)',
      rp_harshbraking: 'Harsh Braking (%)',
      rp_harshbrakeduration: 'Harsh Brake Duration (hh:mm:ss)',
      rp_brakeduration: 'Brake Duration (hh:mm:ss)'
    }
  }

  loadEcoScoreReportPreferences(reloadFlag?: any){
    this.showLoadingIndicator = true;
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.showLoadingIndicator=false;
      this.initData = prefData['userPreferences'];
      if(reloadFlag){
        let _dataObj: any = {
          prefdata: this.initData,
          type: 'eco score report' 
        }
        this.dataInterchangeService.getPrefData(_dataObj);
        this.dataInterchangeService.closedPrefTab(false); // closed pref tab
      }
      this.resetColumnData();
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.showLoadingIndicator = false;
      this.initData = [];
      this.resetColumnData();
    });
  }

  resetColumnData(){
    this.generalColumnData = [];
    this.generalGraphColumnData = [];
    this.driverPerformanceColumnData = [];
    this.driverPerformanceGraphColumnData = [];
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.name.includes('EcoScore.General.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 17);   
              }
              this.generalColumnData.push(_data);
            }else if(item.name.includes('EcoScore.GeneralGraph.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 22);   
              }
              this.generalGraphColumnData.push(_data);
            }else if(item.name.includes('EcoScore.DriverPerformance.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 27);   
              }

              let index: any;
              switch(item.name){
                case 'EcoScore.DriverPerformance.EcoScore':{
                  index = 0;
                  break;
                }
                case 'EcoScore.DriverPerformance.FuelConsumption':{
                  index = 1;
                  break;
                }
                case 'EcoScore.DriverPerformance.BrakingScore':{
                  index = 2;
                  break;
                }
                case 'EcoScore.DriverPerformance.AnticipationScore':{
                  index = 3;
                  break;
                }
              }
              this.driverPerformanceColumnData[index] = _data;
            }else if(item.name.includes('EcoScore.DriverPerformanceGraph.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 32);   
              }
              this.driverPerformanceGraphColumnData.push(_data);
            }
          });
        }
      });
    }
    this.makeNestedDesign();
  }

  makeNestedDesign(){
    if(this.driverPerformanceColumnData && this.driverPerformanceColumnData.length > 0){
      this.driverPerformanceColumnData.forEach((element, index) => {
        if(element.state == 'A'){
          element.isChecked = true;
        }else{
          element.isChecked = false;
        }
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
         if(index == 1){ // fuel consumption
          let childArr: any = [];
          element.subReportUserPreferences.forEach(_elem => {
            if(_elem && _elem.subReportUserPreferences && _elem.subReportUserPreferences.length > 0){ // Cruise child
              _elem.subReportUserPreferences.forEach(_item => {
                if(_item.state == 'A'){
                  _item.isChecked = true;
                }else{
                  _item.isChecked = false;
                }
                if(this.translationData[_item.key]){
                  _item.translatedName = this.translationData[_item.key];  
                }else{
                  _item.translatedName = this.getName(_item.name, 62);   
                }
              });
            }

            if(_elem.state == 'A'){
              _elem.isChecked = true;
            }else{
              _elem.isChecked = false;
            }
            if(this.translationData[_elem.key]){
              _elem.translatedName = this.translationData[_elem.key];  
            }else{
              _elem.translatedName = this.getName(_elem.name, 40);   
            }
            
            if(_elem.name.includes('EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage')){ // "0" position always
              childArr.unshift(_elem); // cruise
            }else{
              childArr.push(_elem); // others
            }
          });
          element.subReportUserPreferences = childArr;
         }
          if(index == 2){ // Braking Score
          element.subReportUserPreferences.forEach(_elem => {
            if(_elem.state == 'A'){
              _elem.isChecked = true;
            }else{
              _elem.isChecked = false;
            }
            if(this.translationData[_elem.key]){
              _elem.translatedName = this.translationData[_elem.key];  
            }else{
              _elem.translatedName = this.getName(_elem.name, 40);   
            }
          });
         } 
        }
      }); 
    }
    this.setColumnCheckbox();
  }

  setColumnCheckbox(){
    this.selectionForGeneralColumns.clear();
    this.selectionForGeneralGraphColumns.clear();
    this.selectionForDriverPerformanceColumns.clear();
    this.selectionForDriverPerformanceGraphColumns.clear();
    
    this.generalColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForGeneralColumns.select(element);
      }
    });

    this.generalGraphColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForGeneralGraphColumns.select(element);
      }
    });

    let _count: any = 0;
    this.driverPerformanceColumnData.forEach(element => {
      if(element.state == 'A'){
        _count++;
        this.selectionForDriverPerformanceColumns.select(element);
      }
    });

    this.mainParent.isChecked = (this.driverPerformanceColumnData.length == _count) ? true : false;
    
    this.driverPerformanceGraphColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForDriverPerformanceGraphColumns.select(element);
      }
    });
  }
  
  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  masterToggleForGeneralColumns(){
    if(this.isAllSelectedForGeneralColumns()){
      this.selectionForGeneralColumns.clear();
    }else{
      this.generalColumnData.forEach(row => { this.selectionForGeneralColumns.select(row) });
    }
  }

  isAllSelectedForGeneralColumns(){
    const numSelected = this.selectionForGeneralColumns.selected.length;
    const numRows = this.generalColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeneralColumns(row?: any){

  }

  masterToggleForGeneralGraphColumns(){
    if(this.isAllSelectedForGeneralGraphColumns()){
      this.selectionForGeneralGraphColumns.clear();
    }else{
      this.generalGraphColumnData.forEach(row => { this.selectionForGeneralGraphColumns.select(row) });
    }
  }

  isAllSelectedForGeneralGraphColumns(){
    const numSelected = this.selectionForGeneralGraphColumns.selected.length;
    const numRows = this.generalGraphColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeneralGraphColumns(row?: any){

  }

  masterToggleForDriverPerformanceColumns(event: any){
    if(event.checked){
      this.driverPerformanceColumnData.forEach(row => { this.selectionForDriverPerformanceColumns.select(row) });
      this.selectDeselectAllChild(true);
    }else{
      this.selectionForDriverPerformanceColumns.clear();
      this.selectDeselectAllChild(false);
    }
  }

  selectDeselectAllChild(_flag: any){
    this.mainParent.isChecked = _flag;
    this.driverPerformanceColumnData.forEach(element => {
      element.isChecked = _flag;
      if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
        element.subReportUserPreferences.forEach(_elem => {
          _elem.isChecked = _flag;
          if(_elem.subReportUserPreferences && _elem.subReportUserPreferences.length > 0){
            _elem.subReportUserPreferences.forEach(_item => {
              _item.isChecked = _flag;
            });
          }
        });
      }
    });
  }

  isAllSelectedForDriverPerformanceColumns(){
    const numSelected = this.selectionForDriverPerformanceColumns.selected.length;
    const numRows = this.driverPerformanceColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForDriverPerformanceColumns(row?: any){

  }

  masterToggleForDriverPerformanceGraphColumns(){
    if(this.isAllSelectedForDriverPerformanceGraphColumns()){
      this.selectionForDriverPerformanceGraphColumns.clear();
    }else{
      this.driverPerformanceGraphColumnData.forEach(row => { this.selectionForDriverPerformanceGraphColumns.select(row) });
    }
  }

  isAllSelectedForDriverPerformanceGraphColumns(){
    const numSelected = this.selectionForDriverPerformanceGraphColumns.selected.length;
    const numRows = this.driverPerformanceGraphColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForDriverPerformanceGraphColumns(row?: any){

  }

  checkboxClicked(event: any, rowData: any){
    
  }

  driverPerformanceCheckboxClicked(event: any, rowData: any, index: any){
    this.driverPerformanceColumnData[index].isChecked = event.checked ? true : false;;
    if(this.driverPerformanceColumnData[index].subReportUserPreferences && this.driverPerformanceColumnData[index].subReportUserPreferences.length > 0){
      this.driverPerformanceColumnData[index].subReportUserPreferences.forEach(element => {
        element.isChecked = event.checked ? true : false;
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(_elem => {
            _elem.isChecked = event.checked ? true : false;
          });  
        }
      });
    }

    let _mainParentCount: any = 0;
    this.driverPerformanceColumnData.forEach(element => {
      if(element.isChecked){
        _mainParentCount++;
      }
    });
    if(this.driverPerformanceColumnData.length == _mainParentCount){ // main parent checked
      this.mainParent.isChecked = true;
    }else{
      this.mainParent.isChecked = false;
    }
  }

  onCancel(){
    this.setEcoScoreFlag.emit({flag: false, msg: ''});
    this.makeNestedDesign();
  }

  onReset(){
    this.makeNestedDesign();
  }

  onConfirm() {
    if (!this.requestSent) {
      this.requestSent = true;
      let _generalArr: any = [];
      let _generalGraphArr: any = [];
      let _driverPerformArr: any = [];
      let _driverPerformGraphArr: any = [];
      let parentDataAttr: any = [];

      this.generalColumnData.forEach(element => {
        let sSearch = this.selectionForGeneralColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (sSearch.length > 0) {
          _generalArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId});
        } else {
          _generalArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      this.generalGraphColumnData.forEach(element => {
        let sSearch = this.selectionForGeneralGraphColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (sSearch.length > 0) {
          _generalGraphArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _generalGraphArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      this.driverPerformanceGraphColumnData.forEach(element => {
        let sSearch = this.selectionForDriverPerformanceGraphColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
        if (sSearch.length > 0) {
          _driverPerformGraphArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _driverPerformGraphArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }
      });

      this.driverPerformanceColumnData.forEach(element => {
        if (element.isChecked) { // parent
          _driverPerformArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        } else {
          _driverPerformArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
        }

        if (element.subReportUserPreferences && element.subReportUserPreferences.length > 0) { // sub-child
          element.subReportUserPreferences.forEach(elem => {
            if (elem.isChecked) {
              _driverPerformArr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
            } else {
              _driverPerformArr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
            }

            if (elem.subReportUserPreferences && elem.subReportUserPreferences.length > 0) { // last child
              elem.subReportUserPreferences.forEach(_item => {
                if (_item.isChecked) {
                  _driverPerformArr.push({ dataAttributeId: _item.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
                } else {
                  _driverPerformArr.push({ dataAttributeId: _item.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
                }
              });
            }
          });
        }
      });

      if (this.initData && this.initData.subReportUserPreferences && this.initData.subReportUserPreferences.length > 0) {
        parentDataAttr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: this.initData.reportId });
        this.initData.subReportUserPreferences.forEach(elem => {
          if (elem.name.includes('EcoScore.General')) {
            if (this.selectionForGeneralColumns.selected.length == this.generalColumnData.length) { // parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          } else if (elem.name.includes('EcoScore.GeneralGraph')) {
            if (this.selectionForGeneralGraphColumns.selected.length == this.generalGraphColumnData.length) { // parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "C", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "C", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          } else if (elem.name.includes('EcoScore.DriverPerformanceGraph')) {
            if (this.selectionForDriverPerformanceGraphColumns.selected.length == this.driverPerformanceGraphColumnData.length) { // parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          } else if (elem.name.includes('EcoScore.DriverPerformance')) {
            if (this.mainParent.isChecked) { // main parent selected
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            } else {
              parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: elem.reportId });
            }
          }
        });
      }

      let objData: any = {
        reportId: this.reportId,
        attributes: [..._generalArr, ..._generalGraphArr, ..._driverPerformGraphArr, ..._driverPerformArr, ...parentDataAttr] //-- merge data
      }
      this.showLoadingIndicator=true;
      this.reportService.updateReportUserPreference(objData).subscribe((_prefData: any) => {
        this.showLoadingIndicator = false;
        let _reloadFlag = false;
        if ((this.router.url).includes("ecoscorereport")) {
          _reloadFlag = true
          //this.reloadCurrentComponent();
        }
        this.loadEcoScoreReportPreferences(_reloadFlag);
        this.setEcoScoreFlag.emit({ flag: false, msg: this.getSuccessMsg() });
        this.requestSent = false;
      }, (error) => {
        this.showLoadingIndicator=false;
        //console.log(error)
      });
    }
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  changeSubChildChecked(event: any, rowData: any, parentIndex: any, childIndex: any){
    if(this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex]){
      this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].isChecked = event.checked ? true : false; 
      if(this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].subReportUserPreferences && this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].subReportUserPreferences.length > 0){
        // sub-child select/unselect
        this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].subReportUserPreferences.forEach(_index => {
          _index.isChecked = event.checked ? true : false;
        });
      }

      let _checkCount: any = 0;
      this.driverPerformanceColumnData[parentIndex].subReportUserPreferences.forEach(element => {
        if(element.isChecked){
          _checkCount++;
        }
      });
      if(this.driverPerformanceColumnData[parentIndex].subReportUserPreferences.length == _checkCount){ // parent checked
        this.driverPerformanceColumnData[parentIndex].isChecked = true;
      }else{
        this.driverPerformanceColumnData[parentIndex].isChecked = false;
      }

      // main parent
      let _mainParentCount: any = 0;
      this.driverPerformanceColumnData.forEach(_el => {
        if(_el.isChecked){
          _mainParentCount++;
        }
      });
      if(this.driverPerformanceColumnData.length == _mainParentCount){ // main parent selected
        this.mainParent.isChecked = true;
      }else{
        this.mainParent.isChecked = false;
      }
    }
  }

  changeLastSubChildChecked(event: any, rowData: any, parentIndex: any, childIndex: any, lastSubChild: any){
    if(this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].subReportUserPreferences[lastSubChild]){
      this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].subReportUserPreferences[lastSubChild].isChecked = event.checked ? true : false;
      
      // child
      let _checkCount: any = 0;
      this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].subReportUserPreferences.forEach(element => {
        if(element.isChecked){
          _checkCount++;
        }
      });
      if(this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].subReportUserPreferences.length == _checkCount){ // child->parent selected
        this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].isChecked = true;
      }else{
        this.driverPerformanceColumnData[parentIndex].subReportUserPreferences[childIndex].isChecked = false;
      }

      // parent
      let _parentCheckCount: any = 0;
      this.driverPerformanceColumnData[parentIndex].subReportUserPreferences.forEach(element => {
        if(element.isChecked){
          _parentCheckCount++;
        }
      });
      if(this.driverPerformanceColumnData[parentIndex].subReportUserPreferences.length == _parentCheckCount){ // child->parent selected
        this.driverPerformanceColumnData[parentIndex].isChecked = true;
      }else{
        this.driverPerformanceColumnData[parentIndex].isChecked = false;
      }

      // main parent
      let _mainParentCount: any = 0;
      this.driverPerformanceColumnData.forEach(_el => {
        if(_el.isChecked){
          _mainParentCount++;
        }
      });
      if(this.driverPerformanceColumnData.length == _mainParentCount){ // main parent selected
        this.mainParent.isChecked = true;
      }else{
        this.mainParent.isChecked = false;
      }
    }
  }

}

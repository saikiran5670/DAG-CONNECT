import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ReportMapService } from 'src/app/report/report-map.service';
@Component({
  selector: 'app-alerts-filter',
  templateUrl: './alerts-filter.component.html',
  styleUrls: ['./alerts-filter.component.less']
})
export class AlertsFilterComponent implements OnInit {
  OrgId = parseInt(localStorage.getItem("accountOrganizationId"));
  isGlobal: boolean = true;   

  @Input() translationData: any = {}; 
  @Input() alertCategoryList: any;
  @Input() alertTypeList: any; 
  @Input() vehicleList: any;
  @Input() alertCriticalityList:any;
  @Input() vehicleGroupList: any= [];
  @Input() alertStatusList: any= [];
  @Input() initData : any;
  @Input() filteredVehicles: any;
  @Input() vehicleByVehGroupList: any;
  @Input() associatedVehicleData :any;
  singleVehicle = [];

  isDisabledAlerts = true; 
  localData : any; 
  tempData: any; 
 
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  accountPrefObj: any;
  alertTypeEnum:any;
  dataResultTypes:any=[];
  @Output() filterValues : EventEmitter<any> = new EventEmitter();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
 
  alertCategory = ''; 
  alertType = ''; 
  alertVehicleGroup: any = '';
  alertVehicle = ''; 
  alertCriticality = ''; 
  alertStatus = '';

  localStLanguage: any;
  accountOrganizationId: any;
  roleObj = { 
    Organizationid : this.OrgId,
    IsGlobal: this.isGlobal
 };
 vehicle_group_selected:any;
 filterListValues = {};
 dataSource = new MatTableDataSource();

 constructor(private translationService: TranslationService, private reportMapService : ReportMapService) { }
   
  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    //this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    if(localStorage.getItem('contextOrgId')){
      this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    }
    else{
      this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    }

    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 17 //-- for alerts
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
     this.processTranslation(data);     
     this.updatedTableData(this.initData);     
     this.dataSource.filterPredicate = this.createFilter();  

     this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
      let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
      if(vehicleDisplayId) {
        let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        if(vehicledisplay.length != 0) {
          this.vehicleDisplayPreference = vehicledisplay[0].name;
        }
      }  
    });
    this.resetVehiclesFilter();
    });
  } 

  resetVehiclesFilter(){
    this.filteredVehicles.next(this.vehicleByVehGroupList.slice());
  }


  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)  
   }

  handleCategoryChange(filter, tempEnum, event) {
    if(event.value == ''){  
        this.dataResultTypes =this.alertTypeList;
        this.isDisabledAlerts = true;
        this.filterListValues['type'] = '';
    }
    else{     
        this.alertType='';
        this.isDisabledAlerts = false;  
        this.dataResultTypes =[]; 
        this.dataResultTypes = this.alertTypeList.filter((s) => s.parentEnum === event.value.enum);
     
      }  
      if(this.alertTypeEnum != undefined){
        if(this.alertTypeEnum != event.value.enum){
          this.filterListValues['type'] = '';
        }
      }    
     this.filterChange(filter, event);
    }

    filterAlertTypeChange(filter, event) {
      let event_val;
      this.alertTypeEnum=event.value.parentEnum ;
        this.filterChange(filter, event);
    }
    
    updatedTableData(tableData : any) {   
      this.dataSource = new MatTableDataSource(tableData);
      setTimeout(()=>{
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
    }
  // Called on Filter change
  filterChange(filter, event, status? : boolean) {   
    console.log("alertVehicle", this.alertVehicleGroup) 
    let event_val;      
      if(filter == "highUrgencyLevel"){          
        if(event.value == ''){          
          event_val = event.value;  
        }
        else{
          event_val = event.value.enum; 
        }
        }else if(filter == "vehicleGroupName"){
          if(status){ //for all option vehicle
            // this.alertVehicleGroup='';
            //event_val = event.value.trim();         
            event_val = this.alertVehicleGroup == ''? this.alertVehicleGroup :this.alertVehicleGroup.value; 
          }
          else if(event.value == ''){ // for all option vehicle group
            this.alertVehicle = '';
             this.alertVehicleGroup='';
            event_val = event.value.trim(); 
          }
          else{   
            this.alertVehicle = '';         
            if(event.value!= undefined){
              this.vehicle_group_selected= event.value.value;
              this.vehicleByVehGroupList= this.associatedVehicleData.filter(item => item.vehicleGroupDetails.includes(this.vehicle_group_selected+"~"));
              this.resetVehiclesFilter();
              event_val = event.value.value.trim();
            }
            else{
              event_val = this.alertVehicleGroup == ''? this.alertVehicleGroup :this.alertVehicleGroup.value; 
            }
          }
       }       
      else{   
      if(event.value == ''){
      event_val = event.value.trim();  
      }
      else{
        event_val = event.value.value.trim();
      }
    }
   this.filterListValues[filter] =event_val;
   this.dataSource.filter = JSON.stringify(this.filterListValues);
   this.filterValues.emit(this.dataSource);    
  }
  

  getUniqueVINs(vinList: any){
    let uniqueVINList = [];
    for(let vin of vinList){
      let vinPresent = uniqueVINList.map(element => element.vin).indexOf(vin.vin);
      if(vinPresent == -1) {
        uniqueVINList.push(vin);
      }
    }
    return uniqueVINList;
  }

  createFilter() {
    return (data: any, filter: string): boolean => {
      let searchTerms = JSON.parse(filter);
      let isFilterSet = false;
      for (const col in searchTerms) {
        if (searchTerms[col].toString() !== '') {
          isFilterSet = true;
        } else {    
          if( col == "highUrgencyLevel"){
          let critical  = data.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'C');
          let warning   = data.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'W');
          let advisory  = data.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'A');
         if(critical.length > 0){
            critical.forEach(obj => { 
            data["highUrgencyLevel"]=obj.urgencyLevelType;
            data["highThresholdValue"]=obj.thresholdValue;
            });
          }else if(warning.length > 0){
            warning.forEach(obj => { 
            data["highUrgencyLevel"]=obj.urgencyLevelType;
            data["highThresholdValue"]=obj.thresholdValue;
          });
          }
          else {
            advisory.forEach(obj => { 
            data["highUrgencyLevel"]=obj.urgencyLevelType;
            data["highThresholdValue"]=obj.thresholdValue;
          });
          }                     
         }         
          delete searchTerms[col];
        }
      }
      console.log(searchTerms);
      let nameSearch = () => {
        let found = false;
        if (isFilterSet) {          
          if(searchTerms.category){
            let category = '';
            category = data.category;
            if(category.includes(searchTerms.category)){
              found = true;    
            }     
          else{
            return false;
          }
        }
        if(searchTerms.type){          
          let type = '';
            type = data.type;
            if(type.includes(searchTerms.type)){
              found = true;    
          }
        else{
          return false;
        }
      }

        if(searchTerms.vehicleGroupName){          
          let vehicleGroupName = '';
          vehicleGroupName = data.vehicleGroupName;
            if(vehicleGroupName.includes(searchTerms.vehicleGroupName)){
              found = true;    
          }
        else{
          return false;
        }
      }

      if(searchTerms.highUrgencyLevel){              
        let highUrgencyLevel = '';
        let levelVal = searchTerms.highUrgencyLevel;
        let criticality  = data.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == levelVal);
      
        if(criticality.length < 1){
          criticality=data.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'C');
        }
        if( levelVal == searchTerms.highUrgencyLevel){
          if(criticality.length > 0){
              criticality.forEach(obj => { 
                data["highUrgencyLevel"]=obj.urgencyLevelType;
                data["highThresholdValue"]=obj.unitType !='N' ? this.getConvertedThresholdValues(obj.thresholdValue, obj.unitType) :  obj.thresholdValue;
              });     
              highUrgencyLevel = data.highUrgencyLevel;
              if(highUrgencyLevel.includes(searchTerms.highUrgencyLevel)){
                  found = true;    
              }      
              else{
                return false;             
              }  
            }   
            else{
              return false;
            }
          }
        }

        if(searchTerms.state){          
          let state = '';
          state = data.state;
            if(state.includes(searchTerms.state)){
              found = true;    
          }
          else{
          return false;
          }
        }

          return found
        } else {
          return true;
        }
      }
      return nameSearch()
    }
    // return filterFunction
  }

  getConvertedThresholdValues(originalThreshold,unitType){
    let threshold;
    if(unitType == 'H' || unitType == 'T' ||unitType == 'S'){
     threshold =this.reportMapService.getConvertedTime(originalThreshold,unitType);
    }
    else if(unitType == 'K' || unitType == 'L'){
     threshold =this.reportMapService.getConvertedDistance(originalThreshold,unitType);
     }
   else if(unitType == 'A' || unitType == 'B'){
     threshold =this.reportMapService.getConvertedSpeed(originalThreshold,unitType);
    }
    else if(unitType == 'P'){
      threshold=originalThreshold;
    }
    return threshold;
  }
 

}

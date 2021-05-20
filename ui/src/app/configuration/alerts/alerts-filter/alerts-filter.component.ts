import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
@Component({
  selector: 'app-alerts-filter',
  templateUrl: './alerts-filter.component.html',
  styleUrls: ['./alerts-filter.component.less']
})
export class AlertsFilterComponent implements OnInit {
  OrgId = parseInt(localStorage.getItem("accountOrganizationId"));
  isGlobal: boolean = true;   

  @Input() translationData: any; 
  @Input() alertCategoryList: any;
  @Input() alertTypeList: any; 
  @Input() vehicleList: any;
  @Input() alertCriticalityList:any;
  @Input() vehicleGroupList: any= [];
  @Input() alertStatusList: any= [];
  @Input() initData : any;

  isDisabledAlerts = true; 
  localData : any; 
  tempData: any;
  alertType:any;
  alertTypeEnum:any;
  dataResultTypes:any=[];
  @Output() filterValues : EventEmitter<any> = new EventEmitter();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
 
  localStLanguage: any;
  accountOrganizationId: any;
  roleObj = { 
    Organizationid : this.OrgId,
    IsGlobal: this.isGlobal
 };

 filterListValues = {};
 dataSource = new MatTableDataSource();
 constructor(private translationService: TranslationService) { }
   
  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 18 //-- for landmark
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
     this.processTranslation(data);  
     this.updatedTableData(this.initData);     
     this.dataSource.filterPredicate = this.createFilter();    
    });   
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
  filterChange(filter, event) {     
    let event_val;      
      if(filter == "highUrgencyLevel"){          
          if(event.value == ''){          
            event_val = event.value;  
          }
          else{
            event_val = event.value.enum; 
          }
          }else if(filter == "vehicleGroupName"){
            if(event.value == ''){
              event_val = event.value.trim();  
            }
            else{
              if(event.value.vehicleName != undefined){
                event_val = event.value.vehicleName.trim();
              }
              else{
                event_val = event.value.value.trim();  
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

  createFilter() {
    let filterFunction = function (data: any, filter: string): boolean {
      let searchTerms = JSON.parse(filter);
      let isFilterSet = false;
      for (const col in searchTerms) {
        if (searchTerms[col].toString() !== '') {
          isFilterSet = true;
        } else {
          delete searchTerms[col];
        }
      }

      console.log(searchTerms);

      let nameSearch = () => {
        let found = false;
        if (isFilterSet) {  
          for (const col in searchTerms) {         
           if( col == "highUrgencyLevel"){
            data.alertUrgencyLevelRefs.forEach(obj => { 
              if (data[col].toString().indexOf(searchTerms[col]) != -1 && isFilterSet) {
                found = true;
                data["highUrgencyLevel"]=obj.urgencyLevelType;
                data["highThresholdValue"]=obj.thresholdValue;
              }             
            });          
           }  
           else if(col == "type"){  
            if (data['category'].toString().indexOf(searchTerms['category']) != -1 && isFilterSet) {
              found = false; 
              if (data[col].toString().indexOf(searchTerms[col]) != -1 && isFilterSet) {
                  found = true;      
              }     
             }                  
            }  
           else{                                
            if (data[col].toString().indexOf(searchTerms[col]) != -1 && isFilterSet) {
            found = true
             }                     
            }
           }
         
          return found
        } else {
          return true;
        }
      }
      return nameSearch()
    }
    return filterFunction
  }

}

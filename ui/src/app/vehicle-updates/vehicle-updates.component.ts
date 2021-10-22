import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReplaySubject } from 'rxjs';
import { FormControl } from '@angular/forms'
import { OtaSoftwareUpdateService } from 'src/app/services/ota-softwareupdate.service';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/core';
import { OrganizationService } from '../services/organization.service';
@Component({
  selector: 'app-vehicle-updates',
  templateUrl: './vehicle-updates.component.html',
  styleUrls: ['./vehicle-updates.component.less']
})
export class VehicleUpdatesComponent implements OnInit {
  displayedColumns: string[] = ['vehicleName', 'registrationNo', 'vehicleGroupNames', 'modelYear', 'type', 'softwareStatus', 'action'];
  grpTitleVisible: boolean = false;
  errorMsgVisible: boolean = false;
  displayMessage: any;

  translationData: any = {};
  localStLanguage: any;
  dataSource: any;
  initData: any = [];
  accountOrganizationId: any;
  accountId: any;
  accountRoleId: any;
  showLoadingIndicator: any = false;
  showVehicalDetails: boolean = false;
  vehicleGroup: any = [];
  vehicleName: any = [];
  vehicleListArrany = [];
  vehicleFilterList: any = [];
  vehicleUpdatesForm: FormGroup;
  vehicleSoftwareStatus: any = [];
  vehicleGroupArr: any = [];
  vehicleNameArr: any = [];
  filterListValues = {};
  vehicleStatusList: any = [];
  searchFilter = new FormControl();
  filteredValues = {
    search: ''
  };
  ngVehicleName = '';
  ngSoftStatus = '';
  actionType: any;
  accountPrefObj: any;
  selectedVehicleUpdateDetails: any = [];
  selectedVehicleUpdateDetailsData: any;
  viewVehicleUpdateDetailsFlag: boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;  
  @ViewChild('select') select: MatSelect;
  public filteredSoftwareStatus: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicleGroup: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicleName: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  allSelected:any = false;
  allDeselected:any = false;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting  
  constructor(private translationService: TranslationService, private otaSoftwareUpdateService: OtaSoftwareUpdateService, private _formBuilder: FormBuilder,  private organizationService: OrganizationService) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 45 //-- for vehicle updates
    }
    this.vehicleUpdatesForm = this._formBuilder.group({
      vehicleGroup: ['', []],
      vehicle: ['', []],
      softStatus: ['', []]
    });
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
       if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        } else { // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }  
      });
      this.loadVehicleStatusData();
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }
  getVehicleSoftStatus() {
    this.vehicleSoftwareStatus = [];
    this.otaSoftwareUpdateService.getVehicleSoftwareStatus().subscribe((data) => {
      let vehicleSoftStatusArr = data['vehicleSoftwareStatus'];
      if (this.translationData != undefined) {
        vehicleSoftStatusArr.forEach(element => {
          if (element.enum == "U") {
            element["value"] = 'Up-to-Date';
          }
          else {
            element["value"] = this.translationData[element["key"]];
          }
          this.vehicleSoftwareStatus.push(element);
        });
      }     
    }, (error) => {
    })   
  }

  searchAllDataFilter() {
    this.dataSource.filterPredicate = this.createFilter();
  
    this.searchFilter.valueChanges.subscribe(filterValue => {
      this.filteredValues['search'] = filterValue.trim().toLowerCase();
      this.dataSource.filter = JSON.stringify(this.filteredValues);
      this.vehicleUpdatesForm.get('vehicle').setValue("all");
      this.vehicleUpdatesForm.get('vehicleGroup').setValue("all");
      this.filterListValues['vehicleName'] = '';
      this.filterListValues['vehicleGroup'] = '';
      this.filterListValues['vehicleGroupNames'] = '';
      this.vehicleName = this.removeDuplicates(this.vehicleNameArr, "vehicleName");
      
      this.select.options.forEach((item: MatOption) => item.select());      
    });
  }

  updateDataSource(tableData: any) {
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  onBackToPage(objData) {
    this.showVehicalDetails = false;
    if (objData.successMsg && objData.successMsg != '') {
      this.successMsgBlink(objData.successMsg);
    }
    this.loadVehicleStatusData();
    this.searchAllDataFilter();
  }

  loadVehicleStatusData() {
    this.showLoadingIndicator = true;    
    this.getVehicleSoftStatus();
    let vehicleStatusObj = {
      languageCode: 'en',
      retention: 'active'
    }  
      this.otaSoftwareUpdateService.getVehicleStatusList(vehicleStatusObj).subscribe((data) => {
      this.showLoadingIndicator = false;
      this.vehicleStatusList = data["vehicleStatusList"];
      this.vehicleStatusList.filter((element) => {
      this.vehicleGroupArr.push(element.vehicleGroupNames);
      this.vehicleNameArr.push({ 'vehicleName': element.vehicleName.trim(), 'vehicleGroup': element.vehicleGroupNames.trim() });
      });

      let vehGrp: any = [];
      this.vehicleGroupArr.forEach(element => {
        let vehGrpTemp = element.split(',');
        vehGrpTemp.forEach((ele: any) => {
          vehGrp.push({ 'vehicleGroup': ele.trim() });
        })
      });
      this.vehicleGroup = this.removeDuplicates(vehGrp, "vehicleGroup");
      this.vehicleName = this.removeDuplicates(this.vehicleNameArr, "vehicleName");
     
      this.initData = this.vehicleStatusList;
      this.updateDataSource(this.initData);        
      this.searchAllDataFilter();
      this.resetSoftStatusFilter();
      this.resetVehicleGroupFilter();
      this.resetVehicleNameFilter();      
    }, (error) => {
      this.showLoadingIndicator = false; 
    })
  }

  removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject = {};
    for (var i in originalArray) {
      lookupObject[originalArray[i][prop]] = originalArray[i];
    }
    for (i in lookupObject) {
      newArray.push(lookupObject[i]);
    }
    return newArray;
  }

  onVehicleGroupChange(filter, event) {    
    this.vehicleName = [];
    this.ngVehicleName = 'all';    
   
    let event_val;

    if (event == 'all') {
      this.vehicleName = this.removeDuplicates(this.vehicleNameArr, "vehicleName");
      event_val = '';
    }
    else {
      let vehicle_group_selected = event.vehicleGroup;
      let vehicle = this.vehicleNameArr.filter(item => item.vehicleGroup.includes(vehicle_group_selected + ","));
      this.vehicleNameArr.forEach(element => {
        if (element.vehicleGroup.includes(vehicle_group_selected)) {
          this.vehicleName.push(element);
        }
      });
      this.vehicleName = this.removeDuplicates(this.vehicleName, "vehicleName");
      this.resetVehicleNameFilter();
      event_val = event.vehicleGroup.trim();
    }
    this.filterListValues['vehicleName'] = '';
    this.select.options.forEach((item: MatOption) => item.select());   
    this.filterListValues[filter] = event_val.toLowerCase();
    this.dataSource.filter = JSON.stringify(this.filterListValues);
  }


  onViewVehicleList(rowData: any, type: any) {
    this.actionType = type;
    this.selectedVehicleUpdateDetails = rowData;
    this.getVehicleUpdateDetails(this.selectedVehicleUpdateDetails);
  }

  getVehicleUpdateDetails(selectedVehicleUpdateDetails: any) {
    this.showLoadingIndicator = true;
    this.showVehicalDetails = true;   
       // Uncomment for Actual API
    this.otaSoftwareUpdateService.getvehicleupdatedetails(selectedVehicleUpdateDetails.vin).subscribe((data: any) => {
      // this.otaSoftwareUpdateService.getvehicleupdatedetails('XLR000000BE000080').subscribe((data: any) => {
        if (data  && data.vehicleUpdateDetails && data.vehicleUpdateDetails !== null) {
        this.selectedVehicleUpdateDetailsData = data.vehicleUpdateDetails;
      }
      this.hideloader();
    }, (error) => {
      this.hideloader();
      console.log("error:: ", error)
    });
  }

  checkViewVehicleUpdateDetails(item: any) {
    //this.createEditViewFeatureFlag = !this.createEditViewFeatureFlag;
    this.viewVehicleUpdateDetailsFlag = item.stepFlag;
    if (item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
  }
  
  filterVehicleSoft(softStatus) {
    if (!this.vehicleSoftwareStatus) {
      return;
    }
    if (!softStatus) {
      this.resetSoftStatusFilter();
      return;
    } else {
      softStatus = softStatus.toLowerCase();     
    }
    this.filteredSoftwareStatus.next(
      this.vehicleSoftwareStatus.filter(item => item.value.toLowerCase().indexOf(softStatus) > -1)
    );
  }

  filterVehicleGroup(vehGroup) {
    if (!this.vehicleGroup) {
      return;
    }
    if (!vehGroup) {
      this.resetVehicleGroupFilter();
      return;
    } else {
      vehGroup = vehGroup.toLowerCase();
     
    }
    this.filteredVehicleGroup.next(
      this.vehicleGroup.filter(item => item.vehicleGroup.toLowerCase().indexOf(vehGroup) > -1)
    );
  }
  filterVehicleName(vehName) {
    if (!this.vehicleName) {
      return;
    }
    if (!vehName) {
      this.resetVehicleNameFilter();
      return;
    } else {
      vehName = vehName.toLowerCase();
     
    }
    this.filteredVehicleName.next(
      this.vehicleName.filter(item => item.vehicleName.toLowerCase().indexOf(vehName) > -1)
    );
  }

  toggleAllSelection() { 
     let filter = "softwareStatus";  
    // this.filterListValues[filter]='';  
    if (this.allSelected) {
      this.select.options.forEach((item: MatOption) => item.select());
      } else {
      this.allDeselected=true;
      let vehicleNm = this.vehicleUpdatesForm.get('vehicle');
      let vehicleGrp = this.vehicleUpdatesForm.get('vehicleGroup');
      if(vehicleGrp.value == 'all' || vehicleNm.value=='all'){
        this.filterListValues['vehicleName'] = '';
        this.filterListValues['vehicleGroupNames'] = '';
      }     
      this.select.options.forEach((item: MatOption) => item.deselect());
      this.allDeselected = false;
     }
  }
   optionClick(filter, event) {
    let newStatus = true;  
    let newSelectAll = false; 
    this.select.options.forEach((item: MatOption) => {
      if (!item.selected) {
        newStatus = false;
        if(event.length==1 && event[0]==''){
          newSelectAll= true;          
        }
        else{
          newSelectAll= false;
        }
      }
    });
    this.allSelected = newStatus;  
    if((this.allDeselected || newSelectAll) && !this.allSelected){  
     this.filterListValues[filter] = 'No Data'; 
     this.dataSource.filter = JSON.stringify(this.filterListValues);     
       }
    else{
      this.filterListValues[filter] = event; 
      this.dataSource.filter = JSON.stringify(this.filterListValues);  
    }
  }
  
  resetSoftStatusFilter() {
    this.filteredSoftwareStatus.next(this.vehicleSoftwareStatus.slice());
  }
  resetVehicleGroupFilter() {
    this.filteredVehicleGroup.next(this.vehicleGroup.slice());
  }
  resetVehicleNameFilter() {
    this.filteredVehicleName.next(this.vehicleName.slice());
  }

  onClose() {
    this.grpTitleVisible = false;
  }

  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }
  successMsgBlink(msg: any) {
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {
      this.grpTitleVisible = false;
    }, 5000);
  }

  errorMsgBlink(errorMsg: any) {
    this.errorMsgVisible = true;
    this.displayMessage = errorMsg;
    setTimeout(() => {
      this.errorMsgVisible = false;
    }, 5000);
  }

  onVehicleChange(filter, event) {
    let event_val;   
    if (filter == "vehicleName") {
      if (event == 'all') {
        event_val = '';
      } else {
        event_val = event.vehicleName.trim();
      }
    }
    else {
      event_val = '';
    }
    this.filterListValues[filter] = event_val.toLowerCase();
    this.dataSource.filter = JSON.stringify(this.filterListValues);

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
      let nameSearch = () => {
        let found = false;
        if (isFilterSet) {
          if (searchTerms.vehicleName) {
            let vehName = '';
            vehName = data.vehicleName;
            if (vehName.toLowerCase().includes(searchTerms.vehicleName)) {
              found = true;
            }
            else {
              return false;
            }
          }
          if (searchTerms.vehicleGroupNames) {
            let vehGrpName = '';
            vehGrpName = data.vehicleGroupNames;
            if (vehGrpName.toLowerCase().includes(searchTerms.vehicleGroupNames)) {
              found = true;
            }
            else {
              return false;
            }
          }
            
          if (searchTerms.softwareStatus) {
            let softStatus = '';
            softStatus = data.softwareStatus.slice('.',-1);
            if (searchTerms.softwareStatus.includes(softStatus.toLowerCase())) {
              found = true;
            }
            else {
              return false;
            }
          }         
          if (searchTerms.search) {

            let searchData = '';
            searchData = data;
            if (searchData["softwareStatus"].toLowerCase().includes(searchTerms.search)) {
              found = true;
            } else if (searchData["vehicleGroupNames"].toLowerCase().includes(searchTerms.search)) {
              found = true;
            } else if (searchData["vehicleName"].toLowerCase().includes(searchTerms.search)) {
              found = true;
            }
            else if (searchData["registrationNo"].toLowerCase().includes(searchTerms.search)) {
              found = true;
            }
            else if (searchData["modelYear"].toLowerCase().includes(searchTerms.search)) {
              found = true;
            }
            else if (searchData["type"].toLowerCase().includes(searchTerms.search)) {
              found = true;
            }
            else {
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
    return filterFunction
  }
  
  proceedStep(prefData: any, preference: any) {
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
    } else {
       this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
    }
   
  }
}

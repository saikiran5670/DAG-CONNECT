import { Component, Input, OnInit, OnDestroy, ViewChild, ElementRef, ChangeDetectorRef, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../../services/translation.service';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { DataInterchangeService } from '../../../services/data-interchange.service';
import { MessageService } from 'src/app/services/message.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { FleetMapService } from '../fleet-map.service';
import { ReplaySubject } from 'rxjs';
import { MatOption } from '@angular/material/core';
import { MatSelect } from '@angular/material/select';
import { Util } from 'src/app/shared/util';
import { ReportMapService } from 'src/app/report/report-map.service';


@Component({
  selector: 'app-fleet-overview-filters',
  templateUrl: './fleet-overview-filters.component.html',
  styleUrls: ['./fleet-overview-filters.component.less']
})
export class FleetOverviewFiltersComponent implements OnInit, OnChanges, OnDestroy {
  @Input() translationData: any = {};
  @Input() fleetOverViewDetail: any;
  @Input() fromVehicleHealth: any;
  @Input() vehInfoPrefData: any;
  @Input() filterData: any;
  @Input() preferenceObject: any;
  @Input() vehicleGroups: any;
  @Input() isFilterOpenClick: any;
  @ViewChild('dataContainer') dataContainer: ElementRef;
  @ViewChild('select1') select1: MatSelect;
  @ViewChild('select2') select2: MatSelect;
  @ViewChild('select3') select3: MatSelect;
  @ViewChild('select4') select4: MatSelect;
  detailsData: any;
  fleetData: any;
  getFleetOverviewDetails: any;
  tabVisibilityStatus: boolean = true;
  drivingStatus: boolean = false;
  selectedIndex: number = 0;
  filterValue: any;
  selection1: any;
  selection2: any;
  selection3: any;
  selection4: any;
  filterVehicleForm: FormGroup;
  isBackClicked: boolean = true;
  todayFlagClicked: boolean = true;
  driverFlagClicked: boolean = true;
  isVehicleDetails: boolean = false;
  isVehicleListOpen: boolean = true;
  noRecordFlag: boolean = true;
  groupList: any = [];
  finalgroupList: any = [];
  driverList: any = [];
  categoryList: any = [];
  vehicleListData: any = [];
  driverListData: any = [];
  finalDriverList: any = [];
  levelList: any = [];
  firstClick: number = 0;
  healthList: any = [];
  otherList: any = [];
  vehicleGroupData = [];
  driverVehicleForm: FormGroup;
  showLoadingIndicator: any = false;
  dataSource: any = new MatTableDataSource([]);
  initData: any = [];
  objData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  translationAlertData: any = {};
  svgIcon: any;
  displayedColumns: string[] = ['icon', 'vin', 'driverName', 'drivingStatus', 'healthStatus'];
  allSelectedHealthStatus = false;
  allSelectedOther = false;
  allSelectedAlertCategory = false;
  allSelectedAlertLevel = false;
  allSelectedGroup = true;
  messages: any[] = [];
  subscription: Subscription;
  status = new FormControl();
  setData: any = {};
  setflag: boolean = false;
  driversListGet = [];
  driversListfilterGet = [];
  forFilterVehicleListData = [];
  getDropDataDriverList = '';
  driverListDropFilter = [];
  filterType: any = [];
  alertListData: any = [];
  categoryListData: any = [];
  groupFilterData: any = [];
  firstClickData: any = [];

  public filteredSelectGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredDrivers: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(private fleetMapService: FleetMapService, private messageService: MessageService, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private sanitizer: DomSanitizer,
    private dataInterchangeService: DataInterchangeService, private cdr: ChangeDetectorRef, private reportMapService: ReportMapService) {
    this.subscription = this.messageService.getMessage().subscribe(message => {
      if (message.key.indexOf("refreshData") !== -1) {
        this.loadVehicleData(true);
      }
    });
    this.dataInterchangeService.isFleetOverViewFilterOpen=true;
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();

    if (this.selectedIndex == 1) {
      this.updateDriverFilter();
    }
    else {
      if (this.detailsData) {
        this.updateVehicleFilter();
      }
      if(this.fromVehicleHealth && this.fromVehicleHealth.fromVehicleHealth){
        this.fleetOverViewDetail={};
      }
      else if(this.fleetOverViewDetail){
        this.setVehicleData();
      }
    }

  }

  ngOnDestroy() {
    this.dataInterchangeService.isFleetOverViewFilterOpen=false;
    if(this.getFleetOverviewDetails){
    this.getFleetOverviewDetails.unsubscribe();
    this.getFleetOverviewDetails=undefined;
    }
    this.subscription.unsubscribe();
  }

  getVinObj(chngs) {
    let arr = [];
    chngs.forEach(item => {
      item.vehicleGroupDetails.split("||").forEach(val => {
        let vgGroupDetail = val.split('~');
        
        if (vgGroupDetail[2] != 'S') {
          arr.push({
            "vehicleGroupId": (vgGroupDetail[0] && vgGroupDetail[0] != '') ? parseInt(vgGroupDetail[0]) : 0,
            "vehicleGroupName": vgGroupDetail[1],
            "vehicleId": parseInt(item.vehicleId),
            'vin': item.vin
          });
        };

      });
    });
    return arr;
  }

  ngOnChanges(changes) {
    if (changes && changes.filterData && changes.filterData.currentValue) {
      this.filterData = changes.filterData.currentValue;
    }

     if (changes && changes.vehicleGroups && changes.vehicleGroups.currentValue && changes.vehicleGroups.currentValue.length > 0 && this.filterData) {
      this.filterData.vehicleGroups = this.getVinObj(changes.vehicleGroups.currentValue);
    }

    if (this.filterData && this.fleetData && this.firstClick == 0) {
      this.updateVehicleFilter();
      this.setDropdownValues(this.fleetData);
      this.selection1 = [];
      this.selection2 = [];
      this.selection3 = [];
      this.selection4 = [];
      this.vehicleListData = this.firstClickData;
      this.noRecordFlag = false;
      this.filterVINonMap();
      this.firstClick = this.firstClick + 1;
    }
  }

  ngOnInit(): void {
    if(this.fleetOverViewDetail)
      this.detailsData=this.fleetOverViewDetail.fleetOverviewDetailList;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.selection1 = ['all'];
    this.selection2 = ['all'];
    this.selection3 = ['all'];
    this.selection4 = ['all'];

    this.filterVehicleForm = this._formBuilder.group({
      group: ['all'],
      level: ['all'],
      category: ['all'],
      status: ['all'],
      otherFilter: ['all'],
      vehicleSearch: ['']
    })

    this.driverVehicleForm = this._formBuilder.group({
      driver: ['all'],
      driverSearch: ['']
    });
    this.getFilterData();
    this.drawIcons(this.detailsData);
  }

  setDefaultDropValue() {
    let healthStatusList = [], otherFilterList = [], alertLevelList = [], alertCategoryList = [];
    healthStatusList = this.healthList.map(i => i.value);
    otherFilterList = this.otherList.map(i => i.value);
    alertLevelList = this.levelList.map(i => i.value);
    alertCategoryList = this.categoryList.map(i => i.value);
    healthStatusList.unshift(0);
    otherFilterList.unshift(0);
    alertLevelList.unshift(0);
    alertCategoryList.unshift(0);
    this.filterVehicleForm.get("status").setValue(healthStatusList);
    this.filterVehicleForm.get("otherFilter").setValue(otherFilterList);
    this.filterVehicleForm.get("level").setValue(alertLevelList);
    this.filterVehicleForm.get("category").setValue(alertCategoryList);
    
    if (this.filterData) {
      let obj = this.levelList.find(element => element.value === 'B');
      
      if (!obj) {
        this.levelList.push({ name: 'No Alert', value: 'B' });
        this.categoryList.push({ name: 'No Alert', value: 'B' });
      }

      if (this.select1 && this.select2 && this.select3 && this.select4) {
        this.toggleAllSelectionAlertLevel();
        this.toggleAllSelectionAlertCategory();
        this.toggleAllSelectionHealth();
        this.toggleAllSelectionOther();
      }

    }
  }

  tabVisibilityHandler(tabVisibility: boolean) {
    this.tabVisibilityStatus = tabVisibility;
  }

  onTabChanged(event: any) {
    this.selectedIndex = event.index;
    this.todayFlagClicked = true;
    
    if (this.selectedIndex == 0) {
      this.updateVehicleFilter();
      this.loadVehicleData();
    }
    
    if (this.selectedIndex == 1) {
      this.updateDriverFilter();
      this.loadDriverData();
    }

  }

  updateDriverFilter() {
    this.driverList = [];
    
    if (this.selectedIndex == 1) {
      this.filterData["driverList"].forEach(item => {
        this.driverList.push(item)
      });

      this.driverList = this.removeDuplicates(this.driverList, "driverId");
      this.finalDriverList = this.driverList;
      this.finalDriverList.sort(this.compareName);
      this.resetDriverSearchFilter();
    }

    else {
      this.detailsData.forEach(element => {
        let currentDate = new Date().getTime();
        let createdDate = parseInt(element.latestProcessedMessageTimeStamp);
        let nextDate = createdDate + 86400000;
        
        if (currentDate > createdDate && currentDate < nextDate) {
          let driverData = this.filterData["driverList"]?.filter(item => item.driverId == element.driver1Id);
          driverData.forEach(item =>
            this.driverList.push(item));
        }
        this.driverList = this.removeDuplicates(this.driverList, "driverId");
      })
    }
  }

  compare(a, b) {
    if (a.vehicleGroupName < b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }

  compareName(a, b) {
    if (a.firstName < b.firstName) {
      return -1;
    }
    if (a.firstName > b.firstName) {
      return 1;
    }
    return 0;
  }

  resetSelectGroupFilter() {
    this.filteredSelectGroups.next(this.finalgroupList.slice());
  }

  resetDriverSearchFilter() {
    if (this.driverListDropFilter) {
      this.driversListfilterGet = this.driverListDropFilter;
    }
  }

  loadDriverData() {
    this.noRecordFlag = true;
    this.driversListGet = [];
    this.driversListfilterGet = [];
    let selectedDriverId: any;
    let selectedDriverDays: any;
    let selectedStartTime = '';
    let selectedEndTime = '';
    
    if (this.preferenceObject.prefTimeFormat == 24) {
      selectedStartTime = "00:00";
      selectedEndTime = "23:59";
    } else {
      selectedStartTime = "12:00 AM";
      selectedEndTime = "11:59 PM";
    }

    let startDateValue = this.setStartEndDateTime(Util.getUTCDate(this.preferenceObject.prefTimeZone), selectedStartTime, 'start');
    let endDateValue = this.setStartEndDateTime(Util.getUTCDate(this.preferenceObject.prefTimeZone), selectedEndTime, 'end');
    let _startTime = Util.getMillisecondsToUTCDate(startDateValue, this.preferenceObject.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(endDateValue, this.preferenceObject.prefTimeZone);
    
    if (this.selectedIndex == 1) {
      if (!this.todayFlagClicked) {
        selectedDriverId = this.driverVehicleForm.controls.driver.value.toString();
        selectedDriverDays = 90;
        this.objData = {
          "groupId": ['all'],
          "alertLevel": ['all'],
          "alertCategory": ['all'],
          "healthStatus": ['all'],
          "otherFilter": ['all'],
          "driverId": [selectedDriverId],
          "days": selectedDriverDays,
          "languagecode": this.localStLanguage ? this.localStLanguage.code : "EN-GB"
        }
      }
      else {
        selectedDriverId = this.driverVehicleForm.controls.driver.value.toString();
        selectedDriverDays = 0;
        this.objData = {
          "groupId": ['all'],
          "alertLevel": ['all'],
          "alertCategory": ['all'],
          "healthStatus": ['all'],
          "otherFilter": ['all'],
          "driverId": [selectedDriverId],
          "days": selectedDriverDays,
          "languagecode": this.localStLanguage ? this.localStLanguage.code : "EN-GB",
          "StartDateTime": _startTime,
          "EndDateTime": _endTime
        }
      }
    }

    let driverSelected = this.driverList.filter((elem) => elem.driverId === this.driverVehicleForm.get("driver").value);
    this.getFleetOverviewDetails =this.reportService.getFleetOverviewDetails(this.objData).subscribe((fleetdata: any) => {
    let data = fleetdata.fleetOverviewDetailList;
    if(this.todayFlagClicked)
      this.dataInterchangeService.setFleetOverViewDetailsToday(JSON.parse(JSON.stringify(fleetdata)));
    else
      this.dataInterchangeService.setFleetOverViewDetails(JSON.parse(JSON.stringify(fleetdata)));
    let val: any;
      if (data.length > 0) {
        if (driverSelected.length > 0) {
          val = [{ driver: driverSelected[0].driverId, data: data }];
        }
        else {
          val = [{ driver: 'all', data: data }];
        }
        this.messageService.sendMessage(val);
        this.drawIcons(data);
      }

      data.forEach(item => {
        if (this.filterData && this.filterData.healthStatus) {
          this.filterData["healthStatus"].forEach(e => {
            if (item.vehicleHealthStatusType == e.value) {
              item.vehicleHealthStatusType = this.translationData[e.name];
            }
          });
        }

        if (this.filterData && this.filterData.otherFilter) {
          this.filterData["otherFilter"].forEach(element => {
            if (item.vehicleDrivingStatusType == element.value) {
              item.vehicleDrivingStatusType = this.translationData[element.name];
            }
          });
        }
      });

      this.vehicleListData = data;
      this.forFilterVehicleListData = data;
      this.detailsData = data;
      // get Drivers's List from fleetOverview - Start
      this.driversListGet = [];
      this.driversListfilterGet = [];
      let unknownandEmptyCount = 0;
      this.driversListGet = JSON.parse(JSON.stringify(data));
      
      for (let item of this.driversListGet) {
        // condition's for Driver List
        if (item.driverName == "" && item.driver1Id == "") {
          unknownandEmptyCount = unknownandEmptyCount + 1;
        }
        if (item.driverName == "Unknown") {
          unknownandEmptyCount = unknownandEmptyCount + 1;
        }
        if (item.driverName == "" && item.driver1Id == "Unknown") {
          unknownandEmptyCount = unknownandEmptyCount + 1;
        }

        if (item.driverName != "" && item.driverName != "Unknown") {
          this.driversListfilterGet.push({ driverName: item.driverName });
        }
        if (item.driverName == "" && item.driver1Id != "" && item.driver1Id != "Unknown") {
          this.driversListfilterGet.push({ driverName: item.driver1Id });
        }
      }

      if (unknownandEmptyCount > 0) {
        this.driversListfilterGet.push({ driverName: 'Unknown' })
      }
      this.driverListDropFilter = this.driversListfilterGet;
      // end

      let _dataObj = {
        vehicleDetailsFlag: this.isVehicleDetails,
        data: data
      }
      this.dataInterchangeService.getVehicleData(_dataObj);//change as per filter data

      if (this.getDropDataDriverList != '') {
        this.driverVehicleForm.get("driver").setValue(this.getDropDataDriverList);
        this.onChangeDriver(this.getDropDataDriverList);
      }

      if (data.length > 0) {
        this.noRecordFlag = false;
      }
      this.applyFilterDriver(this.driverVehicleForm.controls.driverSearch.value);
    }, (error) => {
      this.getFleetOverviewDetails.unsubscribe();
      let val = [{ vehicleGroup: driverSelected[0]?.driverId, data: error }];
      this.messageService.sendMessage(val);
      this.messageService.sendMessage("refreshTimer");
      if (error.status == 404) {
        let _dataObj = {
          vehicleDetailsFlag: this.isVehicleDetails,
          data: null
        }
        this.dataInterchangeService.getVehicleData(_dataObj);
      }
      this.driverVehicleForm.get("driver").setValue('all');
      this.driversListGet = [];
      this.driversListfilterGet = [];
    });
  }
  setStartEndDateTime(date: any, timeObj: any, type: any) {
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.preferenceObject.prefTimeFormat);
  }

  updateVehicleFilter() {
    this.groupList = [];
    this.categoryList = [];
    this.levelList = [];
    this.healthList = [];
    this.otherList = [];
    this.vehicleListData = [];
   
    if (!this.todayFlagClicked && this.selectedIndex == 0) {
      if(this.filterData && this.filterData["vehicleGroups"] && this.filterData["vehicleGroups"].length > 0){
        this.filterData["vehicleGroups"].forEach(item => {
          this.groupList.push(item);
        });
        this.groupList = this.removeDuplicates(this.groupList, "vehicleGroupId");
        this.filteredSelectGroups.next(this.groupList);
      }
      

      if (this.filterData && this.filterData.alertCategory) {
        this.filterData["alertCategory"].forEach(item => {
          let catName = this.translationData[item.name];
          if (catName != undefined) {
            this.categoryList.push({ 'name': catName, 'value': item.value })
          }
        });
      }

      if (this.filterData && this.filterData.alertLevel) {
        this.filterData["alertLevel"].forEach(item => {
          let levelName = this.translationData[item.name];
          this.levelList.push({ 'name': levelName, 'value': item.value })
        });
      }

      if (this.filterData && this.filterData.healthStatus) {
        this.filterData["healthStatus"].forEach(item => {
          let statusName = this.translationData[item.name];
          this.healthList.push({ 'name': statusName, 'value': item.value });
        });
      }

      if (this.filterData && this.filterData.otherFilter) {
        this.filterData["otherFilter"].forEach(item => {
          let statusName = this.translationData[item.name];
          this.otherList.push({ 'name': statusName, 'value': item.value })
        });
      }

      this.setDefaultDropValue();
      this.vehicleListData = this.detailsData;
    }

    if (this.todayFlagClicked && this.selectedIndex == 0) {
      if (this.detailsData) {
        this.detailsData.forEach(element => {
          let currentDate = new Date().getTime();
          let createdDate = parseInt(element.latestProcessedMessageTimeStamp);
          let nextDate = createdDate + 86400000;
          if (currentDate > createdDate && currentDate < nextDate) {
            if (this.filterData && this.filterData.vehicleGroups) {
              let vehicleData = this.filterData["vehicleGroups"].filter(item => item.vin == element.vin);
              vehicleData.forEach(item => this.groupList.push(item));
            }
          }
          this.groupList = this.removeDuplicates(this.groupList, "vehicleGroupId");
          this.finalgroupList = this.groupList;
          this.finalgroupList.sort(this.compare);
          this.resetSelectGroupFilter();
        });
      }

      if (this.filterData && this.filterData.alertCategory) {
        this.filterData["alertCategory"].forEach(item => {
          let catName = this.translationData[item.name];
          if (catName != undefined) {
            this.categoryList.push({ 'name': catName, 'value': item.value })
          }
        });
      }

      if (this.filterData && this.filterData.alertLevel) {
        this.filterData["alertLevel"].forEach(item => {
          let levelName = this.translationData[item.name];
          this.levelList.push({ 'name': levelName, 'value': item.value })
        });
      }

      if (this.filterData && this.filterData.healthStatus) {
        this.filterData["healthStatus"].forEach(item => {
          let statusName = this.translationData[item.name];
          this.healthList.push({ 'name': statusName, 'value': item.value });
        });
      }
      if (this.filterData && this.filterData.otherFilter) {
        this.filterData["otherFilter"].forEach(item => {
          let statusName = this.translationData[item.name];
          this.otherList.push({ 'name': statusName, 'value': item.value })
        });
      }
      this.setDefaultDropValue();
      this.vehicleListData = this.detailsData;
    }

  }

  getFilterData() {
    if (this.selectedIndex == 0) {
      this.updateVehicleFilter();
    }
    if (this.selectedIndex == 1) {
      this.updateDriverFilter();
    }
    this.setDropdownValues(this.fleetData);
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

  applyFilter(filterValue: string) {
    this.vehicleListData = this.detailsData;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();

    const filteredData = this.detailsData.filter(value => {
      const searchStr = filterValue.toLowerCase();
      const vin = value.vin.toLowerCase().toString().includes(searchStr);
      const driver = value.driverName.toLowerCase().toString().includes(searchStr);
      const drivingStatus = value.vehicleDrivingStatusType.toLowerCase().toString().includes(searchStr);
      const healthStatus = value.vehicleHealthStatusType.toLowerCase().toString().includes(searchStr);
      const driverId = value.driver1Id.toLowerCase().toString().includes(searchStr);
      return vin || driver || drivingStatus || healthStatus || driverId;
    });

    this.vehicleListData = filteredData;
    this.filterVINonMap(); // VIN's on map
  }

  applyFilterDriver(filterValue: string) {
    this.vehicleListData = this.detailsData;
    filterValue = filterValue.trim();
    filterValue = filterValue.toLowerCase();

    const filteredData = this.detailsData.filter(value => {
      const searchStr = filterValue.toLowerCase();
      const vin = value.vin.toLowerCase().toString().includes(searchStr);
      const driver = value.driverName.toLowerCase().toString().includes(searchStr);
      const drivingStatus = value.vehicleDrivingStatusType.toLowerCase().toString().includes(searchStr);
      const healthStatus = value.vehicleHealthStatusType.toLowerCase().toString().includes(searchStr);
      const driverId = value.driver1Id.toLowerCase().toString().includes(searchStr);
      return vin || driver || drivingStatus || healthStatus || driverId;
    });

    this.vehicleListData = filteredData;
    this.filterVINonMap(); // VIN's on map
  }

  onChangeGroup(id: any) {
    this.filterVehicleForm.get("group").setValue(id);
    if (id == 'all') {
      this.vehicleListData = this.fleetData;
      this.allSelectedGroup = true;
    }
    else {
      this.allSelectedGroup = false;
      let selectedVehicleGroup = this.filterData.vehicleGroups.filter(item => item.vehicleGroupId == id);
      let VehicleGroupList = this.removeDuplicates(selectedVehicleGroup, "vehicleGroupId");
      let newFilterData = [];
      selectedVehicleGroup.forEach(element => {
        let filterDataList = this.fleetData.filter(item => item.vin == element.vin);
        newFilterData.push(...filterDataList);
      });
      this.vehicleListData = newFilterData;
      this.groupFilterData = [...this.vehicleListData];
    }
    if (!this.allSelectedGroup && this.allSelectedAlertLevel && this.allSelectedAlertCategory && this.allSelectedHealthStatus && this.allSelectedOther) {
      this.vehicleListData = this.groupFilterData;
      this.noRecordFlag = false;
    }
    else {
      this.applyFilterOnVehicleData();
    }
    this.filterVINonMap();
    this.filterVehicleForm.get("vehicleSearch").setValue('');
  }

  toggleAllSelectionAlertLevel() {
    if (this.allSelectedAlertLevel) {
      this.select3.options.forEach((item: MatOption) => item.select());
      this.vehicleListData = this.fleetData;
      this.noRecordFlag = false;
      this.applyFilterOnVehicleData();
    } else {
      this.select3.options.forEach((item: MatOption) => item.deselect());
      if (this.firstClick > 0) {
        this.vehicleListData = [];
        this.onChangeLevel();
      }
    }
    this.filterVINonMap();
  }

  applyFilterOnVehicleData() {

    let objData = [];

    if (this.allSelectedGroup && this.allSelectedAlertLevel && this.allSelectedAlertCategory && this.allSelectedHealthStatus && this.allSelectedOther) {
      this.vehicleListData = this.fleetData;

    }

    else if (this.allSelectedGroup && !this.allSelectedAlertLevel && !this.allSelectedAlertCategory && !this.allSelectedHealthStatus && !this.allSelectedOther &&
      this.filterVehicleForm.controls.level.value.length == 0 && this.filterVehicleForm.controls.category.value.length == 0 &&
      this.filterVehicleForm.controls.status.value.length == 0 && this.filterVehicleForm.controls.otherFilter.value.length == 0) {
      this.vehicleListData = this.fleetData;
      this.noRecordFlag = false;
    }
    else if (!this.allSelectedGroup && !this.allSelectedAlertLevel && !this.allSelectedAlertCategory && !this.allSelectedHealthStatus && !this.allSelectedOther &&
      this.filterVehicleForm.controls.level.value.length == 0 && this.filterVehicleForm.controls.category.value.length == 0 &&
      this.filterVehicleForm.controls.status.value.length == 0 && this.filterVehicleForm.controls.otherFilter.value.length == 0) {
      this.vehicleListData = this.groupFilterData;
      this.noRecordFlag = false;
    }

    else {
      let distinctTypes = ['level', 'category', 'health', 'other'];
      distinctTypes.forEach(ele => {
        switch (ele) {
          case 'level':
            {
              this.fleetData = this.fleetData.filter((value, index, self) => self.indexOf(value) === index);
              if (this.allSelectedAlertLevel && this.allSelectedGroup) {
                this.vehicleListData = this.fleetData;
                this.noRecordFlag = false;
              }
              else if (this.filterVehicleForm.controls.level.value.length > 0) {
                if (this.filterVehicleForm.controls.group.value) {
                  if (this.allSelectedAlertLevel && this.allSelectedAlertCategory) {
                    this.vehicleListData = this.fleetData;
                    this.noRecordFlag = false;
                  }
                  else {
                    this.filterLevel(this.groupFilterData);
                  }
                }

                else {
                  this.filterLevel(this.vehicleListData);
                }
              }
              if (this.filterVehicleForm.controls.level.value.length <= 0 && !this.allSelectedGroup) {
                this.filterLevel(this.groupFilterData);
              }
              this.vehicleListData = this.vehicleListData.filter((value, index, self) => self.indexOf(value) === index);
              break;

            }

          case 'category': {
            if (this.filterVehicleForm.controls.level.value.length <= 0) {
              this.allSelectedGroup ? objData = this.fleetData : objData = this.groupFilterData;
            }
            else {
              this.allSelectedGroup ? objData = this.vehicleListData : objData = this.vehicleListData;
            }
            if (this.filterVehicleForm.controls.category.value.length != 0 && !this.filterVehicleForm.controls.category.value.includes('all')) {
              this.filterCategory(objData);
            }
            break;


          }

          case 'health': {
            if ((this.allSelectedAlertLevel && this.allSelectedAlertCategory && this.allSelectedGroup) ||
              (this.filterVehicleForm.controls.level.value.length == 0 && this.allSelectedAlertCategory) || (this.filterVehicleForm.controls.category.value.length == 0 && this.allSelectedAlertLevel)) {
              this.allSelectedGroup ? this.vehicleListData = this.fleetData : this.vehicleListData = this.groupFilterData;
              objData = this.vehicleListData;
            }
            else if (this.allSelectedAlertLevel && this.allSelectedAlertCategory && !this.allSelectedGroup) {
              this.vehicleListData = this.groupFilterData;
              objData = this.vehicleListData;
            }
            else if (this.allSelectedGroup && this.filterVehicleForm.controls.level.value <= 0 && this.filterVehicleForm.controls.category.value <= 0) {
              objData = this.fleetData;
            }
            else if (!this.allSelectedGroup && this.filterVehicleForm.controls.level.value <= 0 && this.filterVehicleForm.controls.category.value <= 0) {
              objData = this.groupFilterData;
            }
            else {
              objData = this.vehicleListData;
            }
            let vehicledata = [];
            this.filterData["healthStatus"].forEach(e => {
              for (let i of this.filterVehicleForm.controls.status.value) {
                if (i == e.value) {
                  vehicledata.push(this.translationData[e.name]);
                  break;
                }
              }
            });


            if ((this.filterVehicleForm.controls.level.value == 'B' &&
              (this.allSelectedAlertCategory || this.filterVehicleForm.controls.category.value.length == 0 || this.filterVehicleForm.controls.category.value == 'B'))
              || (this.filterVehicleForm.controls.category.value == 'B' && (this.allSelectedAlertLevel || this.filterVehicleForm.controls.level.value.length == 0 ||
                this.filterVehicleForm.controls.level.value == 'B'))) {
              let newData: any;
              if (this.allSelectedGroup) {
                newData = this.fleetData;
              }
              else {
                newData = this.groupFilterData;
              }
              newData.forEach(i => {
                if (i.fleetOverviewAlert.length == 0) {
                  this.vehicleListData.push(i);
                }
              })
            }

            let helthStatusData: any = [];
            if (this.filterVehicleForm.controls.status.value != '' && this.filterVehicleForm.controls.status.value != 'all') {
              objData.forEach(i => {
                for (let e of vehicledata) {
                  if (i.vehicleHealthStatusType == e) {
                    helthStatusData.push(i);
                    break;
                  }
                }
              });
              this.vehicleListData = helthStatusData;
            }
            this.vehicleListData = this.vehicleListData.filter((value, index, self) => self.indexOf(value) === index);
            this.noRecordFlag = (this.vehicleListData.length > 0) ? false : true;
            break;
          }

          case 'other': {
            if (this.filterVehicleForm.controls.level.value <= 0 && this.filterVehicleForm.controls.category.value <= 0 &&
              this.filterVehicleForm.controls.status.value <= 0) {
              objData = this.fleetData;
            }
            else {
              objData = this.vehicleListData;
            }
            if (this.filterVehicleForm.controls.otherFilter.value != 'all') {
              let otherFilterVehicleData = [];
              this.filterData["otherFilter"].forEach(e => {
                for (let i of this.filterVehicleForm.controls.otherFilter.value) {
                  if (i == e.value) {
                    otherFilterVehicleData.push(this.translationData[e.name]);
                    break;
                  }
                }
              });

              let otherFilterData: any = []
              if (otherFilterVehicleData.length > 0) { //new condition
                objData.forEach(i => {
                  for (let e of otherFilterVehicleData) {
                    if (i.vehicleDrivingStatusType == e) {
                      otherFilterData.push(i);
                      break;
                    }
                  }
                });

                this.vehicleListData = otherFilterData;
              }
              this.noRecordFlag = (this.vehicleListData.length > 0) ? false : true;
            }
            break;
          }
        }
      })
    }
  }

  setDefaultValues() {
    if (this.filterVehicleForm.controls.level.value.length == 0 || this.filterVehicleForm.controls.category.value.length == 0 ||
      this.filterVehicleForm.controls.status.value.length == 0 || this.filterVehicleForm.controls.otherFilter.value.length == 0) {
      this.vehicleListData = this.fleetData;
    }
  }

  filterCategory(objData) {
    this.vehicleListData = [];
    if (objData && objData.length > 0) {

      for (let i of this.filterVehicleForm.controls.category.value) {
        this.findCriticality(objData);
        objData.forEach(e => {
          let latestAlert = e.fleetOverviewAlert.sort((x, y) => y.time - x.time);
          if (latestAlert.length > 0) {
            e.alertCategoryType = latestAlert[0].categoryType;
          }
          if (e.alertCategoryType == i) {
            this.vehicleListData.push(e);
            this.noRecordFlag = false;
          }
        })
      }
    }
    this.vehicleListData = this.vehicleListData.filter((value, index, self) => self.indexOf(value) === index);
  }

  findCriticality(objData) {
    if (objData.length > 0) {
      objData.forEach((ele, index) => {
        let newData = ele.fleetOverviewAlert;
        if (newData.length > 0) {
          newData.forEach(j => {
            if (j.level == 'C') {
              ele.alertDetails = [j];
            }
            else if (j.level == 'W' && !(ele.fleetOverviewAlert.some(c => c.level == 'C'))) {
              ele.alertDetails = [j];
            }
            else if (j.level == 'A' && !(ele.fleetOverviewAlert.some(c => c.level == 'C')) && !(ele.fleetOverviewAlert.some(c => c.level == 'W'))) {
              ele.alertDetails = [j];
            }
          })
        }
      });
    }
  }

  filterLevel(fleetData: any) {
    this.vehicleListData = [];
    let critical = 0;
    let warning = 0;
    let advisory = 0;
    let objData: any;
    let latestAlert: any = [];
    if (!this.allSelectedGroup) {
      objData = this.groupFilterData;
    }
    else {
      objData = this.fleetData;
    }
    if (fleetData && fleetData.length > 0 && this.allSelectedAlertLevel) {
      fleetData.forEach(e => {
        if (e.fleetOverviewAlert && e.fleetOverviewAlert.length > 0) {
          for (let i of this.filterVehicleForm.controls.level.value) {
           
            switch (i) {

              case 'C': if (e.fleetOverviewAlert.some(c => c.level == 'C')) {
                critical = 1;
                this.vehicleListData.push(e);
                this.noRecordFlag = false;
                break;
              }
          
              case 'W': if (e.fleetOverviewAlert.some(w => w.level == 'W') && !(e.fleetOverviewAlert.some(c => c.level == 'C'))) {
                warning = 1;
                this.vehicleListData.push(e);
                this.noRecordFlag = false;

                break;
              }
              case 'A': if (e.fleetOverviewAlert.some(a => a.level == 'A') && !(e.fleetOverviewAlert.some(c => c.level == 'C') || e.fleetOverviewAlert.some(w => w.level == 'W'))) {
                advisory = 1;
                this.vehicleListData.push(e);
                this.noRecordFlag = false;

                break;
              }
            }
            if (critical == 1 && warning == 1 && advisory == 1) {
              let newdata = [...this.vehicleListData];
              this.vehicleListData = [];
              newdata.forEach(i => {
                if (i.level == 'C') {
                  this.vehicleListData.push(i);
                }
              })
            }
          }
        }
      });
    }
    else if (this.filterVehicleForm.controls.level.value.length > 0) {
      this.filterVehicleForm.controls.level.value.forEach(i => {
        this.findCriticality(objData);
        switch (i) {
          case 'C': {
            objData.forEach(i => {
              i.alertDetails?.filter(j => {
                if (j.level == 'C') {
                  this.vehicleListData.push(i);
                }
              })
            })

            this.noRecordFlag = false;
            break;
          }
          case 'W': {
            warning = 1;
            objData.forEach(i => {
              i.alertDetails?.filter(j => {
                if (j.level == 'W') {
                  this.vehicleListData.push(i);
                }
              })
            })
            this.noRecordFlag = false;

            break;
          }
          case 'A': {
            advisory = 1;
            objData.forEach(i => {
              i.alertDetails?.filter(j => {
                if (j.level == 'A') {
                  this.vehicleListData.push(i);
                }
              })
            })
            this.noRecordFlag = false;

            break;
          }

        }
      });

    }
    else {
      this.vehicleListData = this.groupFilterData;
    }
    if (this.allSelectedAlertLevel && this.allSelectedAlertCategory && this.filterVehicleForm.controls.status.value == '' && this.filterVehicleForm.controls.otherFilter.value == '') {
      this.filterCategory(this.vehicleListData);
    }
  }

  onChangeLevel() {
    let newStatus = true;
    this.noRecordFlag = true;
    this.vehicleListData = [];

    this.select3.options.forEach((item: MatOption) => {
      if (!item.selected) {
        newStatus = false;
      }
    });
    this.allSelectedAlertLevel = newStatus;
    if (this.filterVehicleForm.controls.level.value.length <= 0) {
      if (this.allSelectedGroup) {
        this.vehicleListData = this.fleetData;
      }
      else {
        this.vehicleListData = this.groupFilterData;
      }
    }
    if (this.allSelectedAlertLevel) {
      this.toggleAllSelectionAlertLevel();
    }
    else {
      this.filterVehicleForm.get("vehicleSearch").setValue('');
      this.applyFilterOnVehicleData();
      this.filterVINonMap();

    }
  }

  toggleAllSelectionAlertCategory() {
    if (this.allSelectedAlertCategory) {
      this.select1.options.forEach((item: MatOption) => item.select());
      this.vehicleListData = this.fleetData;
      this.applyFilterOnVehicleData();
      this.noRecordFlag = false;
    } else {
      this.select1.options.forEach((item: MatOption) => item.deselect());
      if (this.firstClick > 0) {
        this.vehicleListData = [];
        this.onChangeCategory();
      }
    }
    this.filterVINonMap();
  }
  onChangeCategory() {
    let newStatus = true;
    this.noRecordFlag = true;

    this.select1.options.forEach((item: MatOption) => {
      if (!item.selected) {
        newStatus = false;
      }
    });
    this.allSelectedAlertCategory = newStatus;

    if (this.allSelectedAlertCategory) {
      this.toggleAllSelectionAlertCategory();
    }
    else {
      this.applyFilterOnVehicleData();
      this.filterVINonMap();
      this.filterVehicleForm.get("vehicleSearch").setValue('');
    }
  }

  onChangeHealthStatus() {
    let newStatus = true;
    let vehicledata = [];
    this.noRecordFlag = true;

    this.select2.options.forEach((item: MatOption) => {
      if (!item.selected) {
        newStatus = false;
      }
    });
    this.allSelectedHealthStatus = newStatus;

    if (this.allSelectedHealthStatus) {
      this.toggleAllSelectionHealth();
    }
    else {
      this.applyFilterOnVehicleData();
      this.filterVINonMap();
      this.filterVehicleForm.get("vehicleSearch").setValue('');
    }
  }

  toggleAllSelectionHealth() {
    if (this.allSelectedHealthStatus) {
      this.select2.options.forEach((item: MatOption) => item.select());
      this.vehicleListData = this.fleetData;
      this.noRecordFlag = false;
      this.applyFilterOnVehicleData();
    } else {
      this.select2.options.forEach((item: MatOption) => item.deselect());
      if (this.firstClick > 0) {
        this.vehicleListData = [];
        this.onChangeHealthStatus();
      }
    }
    this.filterVINonMap();
  }

  toggleAllSelectionOther() {
    if (this.allSelectedOther) {
      this.select4.options.forEach((item: MatOption) => item.select());
      this.vehicleListData = this.fleetData;
      this.noRecordFlag = false;
      this.applyFilterOnVehicleData();

    } else {
      this.select4.options.forEach((item: MatOption) => item.deselect());
      if (this.firstClick > 0) {
        this.onChangeOtherFilter();
      }
    }
    this.filterVINonMap();
  }

  onChangeOtherFilter() {
    let otherFilterVehicleData = [];
    this.noRecordFlag = true;
    let newStatus = true;
    this.select4.options.forEach((item: MatOption) => {
      if (!item.selected) {
        newStatus = false;
      }
    });
    this.allSelectedOther = newStatus;
    if (this.allSelectedOther) {
      this.toggleAllSelectionOther();
    }
    else {
      this.applyFilterOnVehicleData();
      this.filterVINonMap();
      this.filterVehicleForm.get("vehicleSearch").setValue('');
    }
  }

  filterVINonMap() { // VIN on map
    let _dataObj: any = {
      vehicleDetailsFlag: this.isVehicleDetails,
      data: this.vehicleListData
    }
    if (!this.fromVehicleHealth || (this.fromVehicleHealth && !this.fromVehicleHealth.fromVehicleHealth))
      this.dataInterchangeService.getVehicleData(_dataObj);//change as per filter data
  }

  onChangeDriver(val: any) {
    let value = [];
    this.driverVehicleForm.get("driver").setValue(val);
    this.vehicleListData = this.forFilterVehicleListData.filter(item => {
      if (val == "Unknown") {
        if (item.driverName == "" && item.driver1Id == "") {
          return item;
        }
        if (item.driverName == "Unknown") {
          return item;
        }
        if (item.driverName == "" && item.driver1Id == "Unknown") {
          return item;
        }
      } else if (val == "all") {
        return item;
      } else if (val != "Unknown" && val != "all") {
        if (val == item.driverName) {
          return item;
        }
        if (val == item.driver1Id) {
          return item;
        }
      }
    });

    this.getprocessedLiveFLeetFilterData(this.vehicleListData, value);
    this.driverVehicleForm.get("driverSearch").setValue('');
  }

  getprocessedLiveFLeetFilterData(vehicalDataVal, val) {
    let data = vehicalDataVal;
    this.drawIcons(data);
    data.forEach(item => {
      if (this.filterData && this.filterData.healthStatus) {
        this.filterData["healthStatus"].forEach(e => {
          if (item.vehicleHealthStatusType == e.value) {
            item.vehicleHealthStatusType = this.translationData[e.name];
          }
        });
      }

      if (this.filterData && this.filterData.otherFilter) {
        this.filterData["otherFilter"].forEach(element => {
          if (item.vehicleDrivingStatusType == element.value) {
            item.vehicleDrivingStatusType = this.translationData[element.name];
          }
        });
      }

    });

    let _dataObj = {
      vehicleDetailsFlag: this.isVehicleDetails,
      data: data
    }
    this.dataInterchangeService.getVehicleData(_dataObj);//change as per filter data
  }

  loadVehicleData(refresh?: boolean) {
    this.noRecordFlag = true;
    this.showLoadingIndicator = true;
    this.initData = this.detailsData;
    let newAlertCat = [];
    let otherList = this.filterVehicleForm.controls.otherFilter.value;
    let levelList = this.filterVehicleForm.controls.level.value;
    let status = this.filterVehicleForm.controls.status.value;
    let categoryList = this.filterVehicleForm.controls.category.value;
    let health_status: any;
    if (status.length == 0 || status == 'all') {
      health_status = ['all'];
    }
    else {
      if (status.includes(0)) {
        let newStatus = status.filter(i => i != 0 && i != undefined);
        newStatus.push('all');
        status = newStatus
      }
      health_status = status;
    }

    if (this.filterVehicleForm.controls.otherFilter.value.length == 0 || this.filterVehicleForm.controls.otherFilter.value == 'all') {
      otherList = ['all'];
    }
    else {
      let newOtherList = this.filterVehicleForm.controls.otherFilter.value.filter(i => i != 0 && i != undefined);
      if (this.filterVehicleForm.controls.otherFilter.value.includes(0)) {
        newOtherList.push('all');

      }
      otherList = newOtherList;
    }

    if (this.filterVehicleForm.controls.category.value.length == 0 || this.filterVehicleForm.controls.category.value == 'all') {
      categoryList = ['all'];
    }
    else {
      let newCategoryList = this.filterVehicleForm.controls.category.value.filter(i => i != 0 && i != undefined);
      if (this.filterVehicleForm.controls.category.value.includes(0)) {
        newCategoryList.push('all');

      }
      categoryList = newCategoryList;
    }

    if (this.filterVehicleForm.controls.level.value.length == 0 || this.filterVehicleForm.controls.level.value == 'all') {
      levelList = ['all'];
    }
    else {
      let newLevelList = this.filterVehicleForm.controls.level.value.filter(i => i != 0 && i != undefined);
      if (this.filterVehicleForm.controls.level.value.includes(0)) {
        newLevelList.push('all');
      }
      levelList = newLevelList;
    }

    if (!this.todayFlagClicked && this.selectedIndex == 0) {
      this.objData = {
        "groupId": [this.filterVehicleForm.controls.group.value.toString()],
        "alertLevel": ["all"],
        "alertCategory": ["all"],
        "healthStatus": ["all"],
        "otherFilter": ["all"],
        "driverId": ["all"],
        "days": 90,
        "languagecode": this.localStLanguage ? this.localStLanguage.code : "EN-GB"
      }
    }
    let _startTime;
    let _endTime;
    if (this.todayFlagClicked && this.selectedIndex == 0) {
      let selectedStartTime = '';
      let selectedEndTime = '';
      if (this.preferenceObject.prefTimeFormat == 24) {
        selectedStartTime = "00:00";
        selectedEndTime = "23:59";
      } else {
        selectedStartTime = "12:00 AM";
        selectedEndTime = "11:59 PM";
      }
      let startDateValue = this.setStartEndDateTime(Util.getUTCDate(this.preferenceObject.prefTimeZone), selectedStartTime, 'start');
      let endDateValue = this.setStartEndDateTime(Util.getUTCDate(this.preferenceObject.prefTimeZone), selectedEndTime, 'end');
      _startTime = Util.getMillisecondsToUTCDate(startDateValue, this.preferenceObject.prefTimeZone);
      _endTime = Util.getMillisecondsToUTCDate(endDateValue, this.preferenceObject.prefTimeZone);
      this.objData = {
        "groupId": [this.filterVehicleForm.controls.group.value.toString()],
        "alertLevel": ["all"],
        "alertCategory": ["all"],
        "healthStatus": ["all"],
        "otherFilter": ["all"],
        "driverId": ["all"],
        "days": 0,
        "languagecode": this.localStLanguage ? this.localStLanguage.code : "EN-GB",
        "StartDateTime": _startTime,
        "EndDateTime": _endTime
      }
    }
    let vehicleGroupSel = this.groupList.filter((elem) => elem.vehicleId === this.filterVehicleForm.get("group").value);
    this.getFleetOverviewDetails = this.reportService.getFleetOverviewDetails(this.objData).subscribe((fleetdata: any) => {
      if(this.todayFlagClicked)
        this.dataInterchangeService.setFleetOverViewDetailsToday(JSON.parse(JSON.stringify(fleetdata)));
      else
        this.dataInterchangeService.setFleetOverViewDetails(JSON.parse(JSON.stringify(fleetdata)));
      this.fleetOverViewDetail=fleetdata;
      this.setVehicleData();
    }, (error) => {
      this.getFleetOverviewDetails.unsubscribe();
      this.vehicleListData = [];
      this.detailsData = [];
      this.filterData.vehicleGroups = [];
     this.resetSelectGroupFilter();
      let val = [{ vehicleGroup: vehicleGroupSel.vehicleGroupName, data: error }];
      this.messageService.sendMessage(val);
      this.messageService.sendMessage("refreshTimer");
      if (error.status == 404) {
        this.showLoadingIndicator = false;
        let _dataObj = {
          vehicleDetailsFlag: this.isVehicleDetails,
          data: null
        }
        this.dataInterchangeService.getVehicleData(_dataObj);
      }
      this.showLoadingIndicator = false;
    });
    if (this.filterData && !refresh) {
      this.setDefaultDropValue();
    }
  }
  
  setVehicleData(){
    if(this.fleetOverViewDetail){
      let vehicleGroupSel = this.groupList.filter((elem) => elem.vehicleId === this.filterVehicleForm.get("group").value);
      this.showLoadingIndicator=false;
      let data = this.fleetOverViewDetail.fleetOverviewDetailList;//this.fleetMapService.processedLiveFLeetData(fleetdata.fleetOverviewDetailList);
      this.fleetData = data
      if(this.fleetOverViewDetail && this.fleetOverViewDetail.vehicleGroups && this.fleetOverViewDetail.vehicleGroups.length > 0){
      this.vehicleGroupData = this.fleetOverViewDetail.vehicleGroups;
      }
      else{
        this.vehicleGroupData = [];
      }
      if (this.vehicleGroupData && this.vehicleGroupData.length > 0 && !this.todayFlagClicked && this.selectedIndex == 0 && this.filterData) {
        this.filterData.vehicleGroups = [];
        this.filterData.vehicleGroups = this.getVinObj(this.vehicleGroupData)
      }
      let val = [{ vehicleGroup: vehicleGroupSel.vehicleGroupName, data: data }];
      this.messageService.sendMessage(val);
      this.drawIcons(data);
      this.setDropdownValues(this.fleetData);
      this.vehicleListData = data;
      if(!this.isBackClicked){
      this.applyFilterOnVehicleData();
      }
      this.detailsData = this.vehicleListData;

      let _dataObj: any = {};
      if (this.setflag) { // vehicle details
        let _filterData = data.filter(i => i.id == this.setData.id);
        _dataObj = {
          vehicleDetailsFlag: this.isVehicleDetails,
          data: _filterData.length > 0 ? _filterData[0] : data
        }
      } else { // vehicle filter
        _dataObj = {
          vehicleDetailsFlag: this.isVehicleDetails,
          data: data
        }
      }
        this.dataInterchangeService.getVehicleData(_dataObj);//change as per filter data
      if (data.length > 0) {
        this.noRecordFlag = false;
      }
      // if(this.fleetData && this.fleetData.length > 0){
      // this.showLoadingIndicator = false;
      // }
      this.applyFilter(this.filterVehicleForm.controls.vehicleSearch.value);
    }
  }

  setDropdownValues(data: any) {
    let newAlertCat = [];
    let vehicleGrps = [];
    if (data) {
      data.forEach(item => {

        if (this.filterData && this.filterData.vehicleGroups) {
          this.filterData["vehicleGroups"].forEach(element => {
            if (item.vin == element.vin) {
              vehicleGrps.push(element);
            }
          });

          vehicleGrps = this.removeDuplicates(vehicleGrps, "vehicleGroupId");
          this.filteredSelectGroups.next(vehicleGrps);
        }

        if (this.filterData && this.filterData.healthStatus) {
          this.filterData["healthStatus"].forEach(e => {
            if (item.vehicleHealthStatusType == e.value) {
              item.vehicleHealthStatusType = this.translationData[e.name];
            }
          });
        }

        if (this.filterData && this.filterData.otherFilter) {
          this.filterData["otherFilter"].forEach(element => {
            if (item.vehicleDrivingStatusType == element.value) {
              item.vehicleDrivingStatusType = this.translationData[element.name];
            }
          });
        }
      });
      
      this.firstClickData = data;
    }
  }


  checkToHideFilter(item: any) {
    this.isVehicleDetails = item.vehicleDetailsFlag;
    this.setData = item.data;
    this.setflag = item.setFlag;
  }

  checkCreationForVehicle(item: any) {
    if (!(item.hasOwnProperty('isBackClick') && item.isBackClick === true)) {
      this.vehicleListData = [];
    }
    this.fleetMapService.clearRoutesFromMap();

    this.todayFlagClicked = item.todayFlagClicked;
    if (!this.todayFlagClicked) {
      this.getFilterData();
    }
    this.isVehicleDetails = item.vehicleDetailsFlag;
    if (this.selectedIndex == 1) {
      if (item.hasOwnProperty('vehicleDetailsFlag') && !item.vehicleDetailsFlag) {

        this.getDropDataDriverList = this.driverVehicleForm.controls.driver.value.toString();
      } else {
        this.getDropDataDriverList = '';
      }
      this.driverVehicleForm.get("driver").setValue('all');
      this.loadDriverData();
    } else {
      if (!(item.hasOwnProperty('isBackClick') && item.isBackClick === true)) {
        this.loadVehicleData();
      } else if (item.hasOwnProperty('isBackClick') && item.isBackClick === true) { // back from detail page
        let _dataObj = {
          vehicleDetailsFlag: this.isVehicleDetails,
          data: this.vehicleListData
        }
        this.dataInterchangeService.getVehicleData(_dataObj); // when back clicked 
        this.isBackClicked = true;
        this.loadVehicleData();
      }
    }
  }

  checkCreationForDriver(item: any) {
    this.driverFlagClicked = item.driverFlagClicked;
  }


  drawIcons(_selectedRoutes) {
    _selectedRoutes?.forEach(elem => {

      let _vehicleMarkerDetails = this.setIconsOnMap(elem);
      let _vehicleMarker = _vehicleMarkerDetails['icon'];
      let _alertConfig = _vehicleMarkerDetails['alertConfig'];
      let _type = 'No Warning';
      if (_alertConfig) {
        _type = _alertConfig.type;
      }
      let markerSize = { w: 40, h: 49 };
      let _healthStatus = '', _drivingStatus = '';

      this.svgIcon = this.sanitizer.bypassSecurityTrustHtml(_vehicleMarkerDetails.icon);
      elem = Object.defineProperty(elem, "icon", {
        value: this.svgIcon,
        writable: true, enumerable: true, configurable: true
      });
      if (_alertConfig && _alertConfig.level) {
        if (_alertConfig.level == 'Critical')
          elem['alertName'] = this.translationData.enumurgencylevel_critical;
        else if (_alertConfig.level == 'Warning')
          elem['alertName'] = this.translationData.enumurgencylevel_warning;
        else if (_alertConfig.level == 'Advisory')
          elem['alertName'] = this.translationData.enumurgencylevel_advisory;
      }
    });


  }

  setIconsOnMap(element) {

    let _healthStatus = '', _drivingStatus = '';
    if (element.vehicleDrivingStatusType === 'D' || element.vehicleDrivingStatusType === 'Driving') {
      this.drivingStatus = false;
    }
  
    let healthColor = '#606060';
    let _alertConfig = undefined;
    _drivingStatus = this.fleetMapService.getDrivingStatus(element, _drivingStatus);
    let obj = this.fleetMapService.getVehicleHealthStatusType(element, _healthStatus, healthColor, this.drivingStatus);
    _healthStatus = obj._healthStatus;
    healthColor = obj.healthColor;

    let _vehicleIcon: any;
    if (_drivingStatus) {

    }
    let _alertFound = undefined;
    let alertsData = [];
    if (element.fleetOverviewAlert.length > 0) {
      if (element.tripId != "" && element.fleetOverviewAlert.length > 0) {
        _alertFound = element.fleetOverviewAlert.sort((x, y) => y.time - x.time);
        if (_alertFound) {
          alertsData.push(_alertFound);
        }
      }
      else if (element.tripId == "" && element.fleetOverviewAlert.length > 0) {
        _alertFound = element.fleetOverviewAlert.sort((x, y) => y.time - x.time); //latest timestamp
        if (_alertFound) {
          alertsData.push(_alertFound);
        }
      }

      else {
        //only for never moved type of driving status
        if (_drivingStatus == "Never Moved") {
          let latestAlert: any = [];
          latestAlert = element.fleetOverviewAlert.sort((x, y) => y.time - x.time); //latest timestamp
          _alertFound = latestAlert[0];
          alertsData.push(_alertFound);
        }
      }
    }

    if (_alertFound) {
      if (alertsData[0].length > 1) { //check for criticality
        let criticalCount = 0;
        let warningCount = 0;
        let advisoryCount = 0;
        alertsData[0].forEach(element => {
          criticalCount += element.level === 'C' ? 1 : 0;
          warningCount += element.level === 'W' ? 1 : 0;
          advisoryCount += element.level === 'A' ? 1 : 0;

        });
        
        if (criticalCount > 0) {
          _alertConfig = this.getAlertConfig(alertsData[0].filter(item => item.level === 'C')[0]);
        }
        else if (warningCount > 0) {
          _alertConfig = this.getAlertConfig(alertsData[0].filter(item => item.level === 'W')[0]);
        }
        else if (advisoryCount > 0) {
          _alertConfig = this.getAlertConfig(alertsData[0].filter(item => item.level === 'A')[0]);
        }
      }
      else if (alertsData[0].length == 1) {
        _alertConfig = this.getAlertConfig(_alertFound[0]);
      }
    }

    if (_drivingStatus == "Unknown" || _drivingStatus == "Never Moved") {
      let obj = this.fleetMapService.setIconForUnknownOrNeverMoved(_alertFound, _drivingStatus, _healthStatus, _alertConfig);
      let data = obj.icon;
      return { icon: data, alertConfig: _alertConfig };
    }
    else {
      if (_alertFound) {
        _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M32.5 24.75C32.5 37 16.75 47.5 16.75 47.5C16.75 47.5 1 37 1 24.75C1 20.5728 2.65937 16.5668 5.61307 13.6131C8.56677 10.6594 12.5728 9 16.75 9C20.9272 9 24.9332 10.6594 27.8869 13.6131C30.8406 16.5668 32.5 20.5728 32.5 24.75Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 46.625C24.1875 40.5 31.625 32.9652 31.625 24.75C31.625 16.5348 24.9652 9.875 16.75 9.875C8.53477 9.875 1.875 16.5348 1.875 24.75C1.875 32.9652 9.75 40.9375 16.75 46.625Z" fill="${healthColor}"/>
      <path d="M16.75 37.4375C23.9987 37.4375 29.875 31.8551 29.875 24.9688C29.875 18.0824 23.9987 12.5 16.75 12.5C9.50126 12.5 3.625 18.0824 3.625 24.9688C3.625 31.8551 9.50126 37.4375 16.75 37.4375Z" fill="white"/>
      <g clip-path="url(#clip1)">
      <path d="M11.7041 30.1148C10.8917 30.1148 10.2307 29.4539 10.2307 28.6415C10.2307 27.8291 10.8917 27.1682 11.7041 27.1682C12.5164 27.1682 13.1773 27.8291 13.1773 28.6415C13.1773 29.4539 12.5164 30.1148 11.7041 30.1148ZM11.7041 27.974C11.3359 27.974 11.0365 28.2735 11.0365 28.6416C11.0365 29.0096 11.3359 29.3091 11.7041 29.3091C12.0721 29.3091 12.3715 29.0096 12.3715 28.6416C12.3715 28.2735 12.0721 27.974 11.7041 27.974Z" fill="${healthColor}"/>
      <path d="M21.7961 30.1148C20.9838 30.1148 20.3228 29.4539 20.3228 28.6415C20.3228 27.8291 20.9838 27.1682 21.7961 27.1682C22.6085 27.1682 23.2694 27.8291 23.2694 28.6415C23.2694 29.4539 22.6085 30.1148 21.7961 30.1148ZM21.7961 27.974C21.4281 27.974 21.1285 28.2735 21.1285 28.6416C21.1285 29.0096 21.4281 29.3091 21.7961 29.3091C22.1642 29.3091 22.4637 29.0096 22.4637 28.6416C22.4637 28.2735 22.1642 27.974 21.7961 27.974Z" fill="${healthColor}"/>
      <path d="M18.819 18.5846H14.6812C14.4587 18.5846 14.2783 18.4043 14.2783 18.1817C14.2783 17.9592 14.4587 17.7788 14.6812 17.7788H18.819C19.0415 17.7788 19.2219 17.9592 19.2219 18.1817C19.2219 18.4042 19.0415 18.5846 18.819 18.5846Z" fill="${healthColor}"/>
      <path d="M19.6206 30.2772H13.8795C13.6569 30.2772 13.4766 30.0969 13.4766 29.8743C13.4766 29.6518 13.6569 29.4714 13.8795 29.4714H19.6206C19.8431 29.4714 20.0235 29.6518 20.0235 29.8743C20.0235 30.0968 19.8431 30.2772 19.6206 30.2772Z" fill="${healthColor}"/>
      <path d="M19.6206 27.8119H13.8795C13.6569 27.8119 13.4766 27.6315 13.4766 27.409C13.4766 27.1864 13.6569 27.0061 13.8795 27.0061H19.6206C19.8431 27.0061 20.0235 27.1864 20.0235 27.409C20.0235 27.6315 19.8431 27.8119 19.6206 27.8119Z" fill="${healthColor}"/>
      <path d="M19.6206 29.0445H13.8795C13.6569 29.0445 13.4766 28.8642 13.4766 28.6417C13.4766 28.4191 13.6569 28.2388 13.8795 28.2388H19.6206C19.8431 28.2388 20.0235 28.4191 20.0235 28.6417C20.0235 28.8642 19.8431 29.0445 19.6206 29.0445Z" fill="${healthColor}"/>
      <path d="M25.5346 22.0678H23.552C23.2742 22.0678 23.0491 22.2929 23.0491 22.5707V23.6681L22.7635 23.9697V18.1753C22.7635 17.2023 21.9722 16.411 20.9993 16.411H12.5009C11.528 16.411 10.7365 17.2023 10.7365 18.1753V23.9696L10.451 23.6681V22.5707C10.451 22.2929 10.2259 22.0678 9.94814 22.0678H7.96539C7.68767 22.0678 7.4625 22.2929 7.4625 22.5707V23.8683C7.4625 24.1461 7.68767 24.3712 7.96539 24.3712H9.73176L10.1695 24.8335C9.49853 25.0833 9.01905 25.73 9.01905 26.4873V31.7339C9.01905 32.0117 9.24416 32.2368 9.52194 32.2368H10.1291V33.4026C10.1291 34.1947 10.7734 34.839 11.5655 34.839C12.3575 34.839 13.0018 34.1947 13.0018 33.4026V32.2368H20.4981V33.4026C20.4981 34.1947 21.1424 34.839 21.9345 34.839C22.7266 34.839 23.3709 34.1947 23.3709 33.4026V32.2368H23.9781C24.2558 32.2368 24.481 32.0117 24.481 31.7339V26.4873C24.481 25.73 24.0015 25.0834 23.3306 24.8336L23.7683 24.3712H25.5346C25.8124 24.3712 26.0375 24.1461 26.0375 23.8683V22.5707C26.0375 22.2929 25.8123 22.0678 25.5346 22.0678ZM9.4452 23.3655H8.46828V23.0736H9.4452V23.3655ZM11.7422 18.1753C11.7422 17.7571 12.0826 17.4168 12.5009 17.4168H20.9992C21.4173 17.4168 21.7576 17.7571 21.7576 18.1753V18.9469H11.7422V18.1753ZM21.7577 19.9526V24.723H17.2529V19.9526H21.7577ZM11.7422 19.9526H16.2471V24.723H11.7422V19.9526ZM11.996 33.4025C11.996 33.6399 11.8027 33.8331 11.5655 33.8331C11.3281 33.8331 11.1349 33.6399 11.1349 33.4025V32.2368H11.996V33.4025ZM22.3651 33.4025C22.3651 33.6399 22.1718 33.8331 21.9345 33.8331C21.6972 33.8331 21.5039 33.6399 21.5039 33.4025V32.2368H22.3651V33.4025ZM23.4752 26.4873V31.231H10.0248V26.4873C10.0248 26.0692 10.3652 25.7288 10.7834 25.7288H22.7166C23.1348 25.7288 23.4752 26.0692 23.4752 26.4873ZM25.0317 23.3655H24.0549V23.0736H25.0317V23.3655Z" fill="${healthColor}" stroke="${healthColor}" stroke-width="0.2"/>
      </g>
      <mask id="path-11-outside-1" maskUnits="userSpaceOnUse" x="17.6667" y="0.666748" width="23" height="19" fill="black">
      <rect fill="white" x="17.6667" y="0.666748" width="23" height="19"/>
      <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z"/>
      </mask>
      <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z" fill="${_alertConfig.color}"/>
      <path d="M29.0001 4.66675L30.7309 3.66468L29.0001 0.675021L27.2692 3.66468L29.0001 4.66675ZM21.6667 17.3334L19.9359 16.3313L18.1979 19.3334H21.6667V17.3334ZM36.3334 17.3334V19.3334H39.8023L38.0643 16.3313L36.3334 17.3334ZM27.2692 3.66468L19.9359 16.3313L23.3976 18.3355L30.7309 5.66882L27.2692 3.66468ZM21.6667 19.3334H36.3334V15.3334H21.6667V19.3334ZM38.0643 16.3313L30.7309 3.66468L27.2692 5.66882L34.6026 18.3355L38.0643 16.3313Z" fill="white" mask="url(#path-11-outside-1)"/>
      <path d="M29.6666 14H28.3333V15.3333H29.6666V14Z" fill="white"/>
      <path d="M29.6666 10H28.3333V12.6667H29.6666V10Z" fill="white"/>
      <defs>
      <clipPath id="clip1">
      <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 16.4375)"/>
      </clipPath>
      </defs>
      </svg>`;
      }
      else {
        _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M32.5 17.5C32.5 29.75 16.75 40.25 16.75 40.25C16.75 40.25 1 29.75 1 17.5C1 13.3228 2.65937 9.31677 5.61307 6.36307C8.56677 3.40937 12.5728 1.75 16.75 1.75C20.9272 1.75 24.9332 3.40937 27.8869 6.36307C30.8406 9.31677 32.5 13.3228 32.5 17.5Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 39.375C24.1875 33.25 31.625 25.7152 31.625 17.5C31.625 9.28475 24.9652 2.62498 16.75 2.62498C8.53477 2.62498 1.875 9.28475 1.875 17.5C1.875 25.7152 9.75 33.6875 16.75 39.375Z" fill="${healthColor}"/>
      <path d="M16.75 30.1875C23.9987 30.1875 29.875 24.605 29.875 17.7187C29.875 10.8324 23.9987 5.24998 16.75 5.24998C9.50126 5.24998 3.625 10.8324 3.625 17.7187C3.625 24.605 9.50126 30.1875 16.75 30.1875Z" fill="white"/>
      <g clip-path="url(#clip0)">
      <path d="M11.7041 22.8649C10.8917 22.8649 10.2307 22.2039 10.2307 21.3916C10.2307 20.5792 10.8917 19.9183 11.7041 19.9183C12.5164 19.9183 13.1773 20.5792 13.1773 21.3916C13.1773 22.204 12.5164 22.8649 11.7041 22.8649ZM11.7041 20.7241C11.3359 20.7241 11.0365 21.0235 11.0365 21.3916C11.0365 21.7597 11.3359 22.0591 11.7041 22.0591C12.0721 22.0591 12.3715 21.7597 12.3715 21.3916C12.3715 21.0235 12.0721 20.7241 11.7041 20.7241Z" fill="${healthColor}"/>
      <path d="M21.7961 22.8649C20.9838 22.8649 20.3228 22.2039 20.3228 21.3916C20.3228 20.5792 20.9838 19.9183 21.7961 19.9183C22.6085 19.9183 23.2694 20.5792 23.2694 21.3916C23.2694 22.204 22.6085 22.8649 21.7961 22.8649ZM21.7961 20.7241C21.4281 20.7241 21.1285 21.0235 21.1285 21.3916C21.1285 21.7597 21.4281 22.0591 21.7961 22.0591C22.1642 22.0591 22.4637 21.7597 22.4637 21.3916C22.4637 21.0235 22.1642 20.7241 21.7961 20.7241Z" fill="${healthColor}"/>
      <path d="M18.819 11.3345H14.6812C14.4587 11.3345 14.2783 11.1542 14.2783 10.9317C14.2783 10.7092 14.4587 10.5288 14.6812 10.5288H18.819C19.0415 10.5288 19.2219 10.7092 19.2219 10.9317C19.2219 11.1542 19.0415 11.3345 18.819 11.3345Z" fill="${healthColor}"/>
      <path d="M19.6206 23.0272H13.8795C13.6569 23.0272 13.4766 22.8468 13.4766 22.6243C13.4766 22.4018 13.6569 22.2214 13.8795 22.2214H19.6206C19.8431 22.2214 20.0235 22.4018 20.0235 22.6243C20.0235 22.8468 19.8431 23.0272 19.6206 23.0272Z" fill="${healthColor}"/>
      <path d="M19.6206 20.5619H13.8795C13.6569 20.5619 13.4766 20.3815 13.4766 20.159C13.4766 19.9364 13.6569 19.7561 13.8795 19.7561H19.6206C19.8431 19.7561 20.0235 19.9364 20.0235 20.159C20.0235 20.3815 19.8431 20.5619 19.6206 20.5619Z" fill="${healthColor}"/>
      <path d="M19.6206 21.7945H13.8795C13.6569 21.7945 13.4766 21.6142 13.4766 21.3916C13.4766 21.1691 13.6569 20.9887 13.8795 20.9887H19.6206C19.8431 20.9887 20.0235 21.1691 20.0235 21.3916C20.0235 21.6142 19.8431 21.7945 19.6206 21.7945Z" fill="${healthColor}"/>
      <path d="M25.5346 14.8178H23.552C23.2742 14.8178 23.0491 15.0429 23.0491 15.3207V16.4181L22.7635 16.7197V10.9253C22.7635 9.95231 21.9722 9.16096 20.9993 9.16096H12.5009C11.528 9.16096 10.7365 9.9523 10.7365 10.9253V16.7196L10.451 16.4181V15.3207C10.451 15.0429 10.2259 14.8178 9.94814 14.8178H7.96539C7.68767 14.8178 7.4625 15.0429 7.4625 15.3207V16.6183C7.4625 16.8961 7.68767 17.1212 7.96539 17.1212H9.73176L10.1695 17.5835C9.49853 17.8333 9.01905 18.48 9.01905 19.2373V24.4839C9.01905 24.7617 9.24416 24.9868 9.52194 24.9868H10.1291V26.1526C10.1291 26.9447 10.7734 27.5889 11.5655 27.5889C12.3575 27.5889 13.0018 26.9447 13.0018 26.1526V24.9868H20.4981V26.1526C20.4981 26.9447 21.1424 27.5889 21.9345 27.5889C22.7266 27.5889 23.3709 26.9447 23.3709 26.1526V24.9868H23.9781C24.2558 24.9868 24.481 24.7617 24.481 24.4839V19.2373C24.481 18.48 24.0015 17.8333 23.3306 17.5835L23.7683 17.1212H25.5346C25.8124 17.1212 26.0375 16.8961 26.0375 16.6183V15.3207C26.0375 15.0429 25.8123 14.8178 25.5346 14.8178ZM9.4452 16.1154H8.46828V15.8236H9.4452V16.1154ZM11.7422 10.9253C11.7422 10.5071 12.0826 10.1667 12.5009 10.1667H20.9992C21.4173 10.1667 21.7576 10.5071 21.7576 10.9253V11.6969H11.7422V10.9253ZM21.7577 12.7026V17.4729H17.2529V12.7026H21.7577ZM11.7422 12.7026H16.2471V17.4729H11.7422V12.7026ZM11.996 26.1525C11.996 26.3898 11.8027 26.5831 11.5655 26.5831C11.3281 26.5831 11.1349 26.3898 11.1349 26.1525V24.9867H11.996V26.1525ZM22.3651 26.1525C22.3651 26.3898 22.1718 26.5831 21.9345 26.5831C21.6972 26.5831 21.5039 26.3898 21.5039 26.1525V24.9867H22.3651V26.1525ZM23.4752 19.2373V23.981H10.0248V19.2373C10.0248 18.8191 10.3652 18.4788 10.7834 18.4788H22.7166C23.1348 18.4788 23.4752 18.8191 23.4752 19.2373ZM25.0317 16.1154H24.0549V15.8236H25.0317V16.1154Z" fill="${healthColor}" stroke="${healthColor}" stroke-width="0.2"/>
      </g>
      <defs>
      <clipPath id="clip0">
      <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 9.18748)"/>
      </clipPath>
      </defs>
      </svg>`

      }
      return { icon: _vehicleIcon, alertConfig: _alertConfig };
    }
  }

  getAlertConfig(_currentAlert) {
    let _alertConfig = { color: '#D50017', level: 'Critical', type: '' };
    let _fillColor = '#D50017';
    let _level = 'Critical';
    let _type = '';
    
    switch (_currentAlert.level) {
      case 'C': {
        _fillColor = '#D50017';
        _level = 'Critical'
      }
        break;
      case 'W': {
        _fillColor = '#FC5F01';
        _level = 'Warning'
      }
        break;
      case 'A': {
        _fillColor = '#FFD80D';
        _level = 'Advisory'
      }
        break;
      default:
        break;
    }

    switch (_currentAlert.categoryType) {
      case 'L': {
        _type = 'Logistics Alerts'
      }
        break;
      case 'F': {
        _type = 'Fuel and Driver Performance'
      }
        break;
      case 'R': {
        _type = 'Repair and Maintenance'
      }
        break;
      default:
        break;
    }
    return { color: _fillColor, level: _level, type: _type };
  }

  filterSelectGroups(groupsearch) {
    if (!this.finalgroupList) {
      return;
    }

    if (!groupsearch) {
      this.resetSelectGroupFilter();
      return;
    } else {
      groupsearch = groupsearch.toLowerCase();
    }

    this.filteredSelectGroups.next(
      this.finalgroupList.filter(item => item.vehicleGroupName.toLowerCase().indexOf(groupsearch) > -1)

    );
  }

  filterSelectDrivers(driversearch) {
    if (driversearch == "") {
      this.driversListfilterGet = this.driverListDropFilter;
    }

    if (!this.driversListfilterGet) {
      return;
    }

    if (!driversearch) {
      this.resetDriverSearchFilter();
      return;
    } else {
      driversearch = driversearch.toLowerCase();
    }
    this.driversListfilterGet = this.driverListDropFilter.filter(item => item.driverName.toLowerCase().indexOf(driversearch) > -1);
  }

}

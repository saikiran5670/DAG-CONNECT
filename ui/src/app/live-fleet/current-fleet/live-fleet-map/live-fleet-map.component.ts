import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslationService } from '../../../services/translation.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { ReportService } from '../../../services/report.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { ReportMapService } from '../../../report/report-map.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import { ConfigService } from '@ngx-config/core';
import 'jspdf-autotable';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { LandmarkCategoryService } from '../../../services/landmarkCategory.service';
import { HereService } from '../../../services/here.service';
import * as moment from 'moment-timezone';
import { Router, NavigationExtras } from '@angular/router';
import { OrganizationService } from '../../../services/organization.service';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { FleetMapService } from '../fleet-map.service'

declare var H: any;

@Component({
  selector: 'app-live-fleet-map',
  templateUrl: './live-fleet-map.component.html',
  styleUrls: ['./live-fleet-map.component.less']
})
export class LiveFleetMapComponent implements OnInit {
  showMap: boolean = false;
  showBack: boolean = false;
  showMapPanel: boolean = false;
  advanceFilterOpen: boolean = false;
  mapFilterForm: FormGroup;
  userPOIList: any = [];
  herePOIList: any = [];
  displayPOIList: any = [];
  internalSelection: boolean = false;
  herePOIArr: any = [];
  map_key: any = '';
  platform: any = '';
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  dataSource: any = new MatTableDataSource([]);
  selectedTrip = new SelectionModel(true, []);
  selectedPOI = new SelectionModel(true, []);
  selectedHerePOI = new SelectionModel(true, []);
  trackType: any = 'snail';
  displayRouteView: any = 'C';
  @Input() translationData:any;
  @ViewChild("map") public mapElement: ElementRef;
  accountId:any;
  accountPrefObj : any;
  tripTraceArray = [];
  searchMarker: any = {};
  constructor(
    private translationService: TranslationService,
    private _formBuilder: FormBuilder,
    private reportService: ReportService,
    private reportMapService: ReportMapService,
    private landmarkCategoryService: LandmarkCategoryService,
    private router: Router,
    private organizationService: OrganizationService,
    private _configService: ConfigService,
    private hereService: HereService,
    private fleetMapService:FleetMapService) {
    this.map_key = _configService.getSettings("hereMap").api_key;
    //Add for Search Fucntionality with Zoom
    ///this.query = "starbucks";
    this.platform = new H.service.Platform({
      "apikey": this.map_key // "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
    //this.configureAutoSuggest();
    //this.defaultTranslation();
    const navigation = this.router.getCurrentNavigation();

  }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
   
    this.mapFilterForm = this._formBuilder.group({
      routeType: ['', []],
      trackType: ['', []]
    });
    this.mapFilterForm.get('trackType').setValue('snail');
    this.mapFilterForm.get('routeType').setValue('C');
    setTimeout(() => {
      this.reportMapService.initMap(this.mapElement);
    }, 0);
    this.makeHerePOIList();
    this.loadUserPOI();

  }


  makeHerePOIList(){
    this.herePOIList = [{
      key: 'Hotel',
      translatedName: this.translationData.lblHotel || 'Hotel'
    },
    {
      key: 'Parking',
      translatedName: this.translationData.lblParking || 'Parking'
    },
    {
      key: 'Petrol Station',
      translatedName: this.translationData.lblPetrolStation || 'Petrol Station'
    },
    {
      key: 'Railway Station',
      translatedName: this.translationData.lblRailwayStation || 'Railway Station'
    }];
  }

  loadUserPOI() {
    this.landmarkCategoryService.getCategoryWisePOI(this.accountOrganizationId).subscribe((poiData: any) => {
      this.userPOIList = this.makeUserCategoryPOIList(poiData);
    }, (error) => {
      this.userPOIList = [];
    });
  }

  makeUserCategoryPOIList(poiData: any) {
    let categoryArr: any = [];
    let _arr: any = poiData.map(item => item.categoryId).filter((value, index, self) => self.indexOf(value) === index);
    _arr.forEach(element => {
      let _data = poiData.filter(i => i.categoryId == element);
      if (_data.length > 0) {
        let subCatUniq = _data.map(i => i.subCategoryId).filter((value, index, self) => self.indexOf(value) === index);
        let _subCatArr = [];
        if (subCatUniq.length > 0) {
          subCatUniq.forEach(elem => {
            let _subData = _data.filter(i => i.subCategoryId == elem && i.subCategoryId != 0);
            if (_subData.length > 0) {
              _subCatArr.push({
                poiList: _subData,
                subCategoryName: _subData[0].subCategoryName,
                subCategoryId: _subData[0].subCategoryId,
                checked: false
              });
            }
          });
        }

        _data.forEach(data => {
          data.checked = false;
        });

        categoryArr.push({
          categoryId: _data[0].categoryId,
          categoryName: _data[0].categoryName,
          poiList: _data,
          subCategoryPOIList: _subCatArr,
          open: false,
          parentChecked: false
        });
      }
    });

    return categoryArr;
  }

  changeUserPOISelection(event: any, poiData: any, index: any){
    if (event.checked){ // checked
      this.userPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = true;
      });
      this.userPOIList[index].poiList.forEach(_elem => {
        _elem.checked = true;
      });
      this.userPOIList[index].parentChecked = true;
      // if(this.selectedPOI.selected.length > 0){
      //   let _s: any = this.selectedPOI.selected.filter(i => i.categoryId == this.userPOIList[index].categoryId);
      //   if(_s.length > 0){

      //   }
      // }else{

      // }
    }else{ // unchecked
      this.userPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = false;
      });
      this.userPOIList[index].poiList.forEach(_elem => {
        _elem.checked = false;
      });
      this.userPOIList[index].parentChecked = false;
    }
    this.displayPOIList = [];
    this.selectedPOI.selected.forEach(item => {
      if(item.poiList && item.poiList.length > 0){
        item.poiList.forEach(element => {
          if(element.checked){ // only checked
            this.displayPOIList.push(element);
          }
        });
      }
    });
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);

    //this.reportMapService.showCategoryPOI(this.displayPOIList,_ui);
  }

  openClosedUserPOI(index: any) {
    this.userPOIList[index].open = !this.userPOIList[index].open;
  }
  changeSubCategory(event: any, subCatPOI: any, _index: any) {
    let _uncheckedCount: any = 0;
    this.userPOIList[_index].subCategoryPOIList.forEach(element => {
      if (element.subCategoryId == subCatPOI.subCategoryId) {
        element.checked = event.checked ? true : false;
      }

      if (!element.checked) { // unchecked count
        _uncheckedCount += element.poiList.length;
      }
    });

    if (this.userPOIList[_index].poiList.length == _uncheckedCount) {
      this.userPOIList[_index].parentChecked = false; // parent POI - unchecked
      let _s: any = this.selectedPOI.selected;
      if (_s.length > 0) {
        this.selectedPOI.clear(); // clear parent category data
        _s.forEach(element => {
          if (element.categoryId != this.userPOIList[_index].categoryId) { // exclude parent category data
            this.selectedPOI.select(element);
          }
        });
      }
    } else {
      this.userPOIList[_index].parentChecked = true; // parent POI - checked
      let _check: any = this.selectedPOI.selected.filter(k => k.categoryId == this.userPOIList[_index].categoryId); // already present
      if (_check.length == 0) { // not present, add it
        let _s: any = this.selectedPOI.selected;
        if (_s.length > 0) { // other element present
          this.selectedPOI.clear(); // clear all
          _s.forEach(element => {
            this.selectedPOI.select(element);
          });
        }
        this.userPOIList[_index].poiList.forEach(_el => {
          if (_el.subCategoryId == 0) {
            _el.checked = true;
          }
        });
        this.selectedPOI.select(this.userPOIList[_index]); // add parent element
      }
    }

    this.displayPOIList = [];
    //if(this.selectedPOI.selected.length > 0){
    this.selectedPOI.selected.forEach(item => {
      if (item.poiList && item.poiList.length > 0) {
        item.poiList.forEach(element => {
          if (element.subCategoryId == subCatPOI.subCategoryId) { // element match
            if (event.checked) { // event checked
              element.checked = true;
              this.displayPOIList.push(element);
            } else { // event unchecked
              element.checked = false;
            }
          } else {
            if (element.checked) { // element checked
              this.displayPOIList.push(element);
            }
          }
        });
      }
    });
    let _ui = this.reportMapService.getUI();
    //this.fleetMapService.showCategoryPOI(this.displayPOIList,_ui);
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
    //}
  }

  changeHerePOISelection(event: any, hereData: any){
    this.herePOIArr = [];
    this.selectedHerePOI.selected.forEach(item => {
      this.herePOIArr.push(item.key);
    });
    this.searchPlaces();
  }

  searchPlaces() {
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr); 
  }

  onMapRepresentationChange(event: any) {
    this.trackType = event.value;
    let _ui = this.reportMapService.getUI();
    //this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }
  onAdvanceFilterOpen() {
    this.advanceFilterOpen = !this.advanceFilterOpen;
  }


  onDisplayChange(event: any) {
    this.displayRouteView = event.value;
    let _ui = this.reportMapService.getUI();
    this.reportMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }

}

import { Component, Input, Output, EventEmitter,OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CorridorService } from 'src/app/services/corridor.service';

declare var H: any;

@Component({
  selector: 'app-manage-corridor',
  templateUrl: './manage-corridor.component.html',
  styleUrls: ['./manage-corridor.component.less']
})

export class ManageCorridorComponent implements OnInit {
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  @Input() translationData: any;
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  displayedColumns = ['corridoreName','startPoint', 'endPoint', 'distance', 'width', 'action'];
  createEditStatus = false;
  accountOrganizationId: any = 0;
  corridorCreatedMsg: any = '';
  actionType : string;
  titleVisible : boolean = false;
  showMap: boolean = false;
  map: any;
  initData: any = [];
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @ViewChild("map")
  public mapElement: ElementRef;
  platform: any;
  selectedElementData: any;
  showLoadingIndicator: boolean;
  localStLanguage: any;

  
  constructor( private corridorService : CorridorService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(){ 
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.loadCorridorData()
  }

  loadCorridorData(){
    this.showLoadingIndicator = true;
    this.corridorService.getCorridorList(this.accountOrganizationId).subscribe((data : any) => {
      this.initData = data;
      this.hideloader();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedTableData(this.initData);
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  updatedTableData(tableData : any) {
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    if(data.length > 0){
      data.forEach(row => {
        let createdDate = parseInt(row.createdAt); 
        let nextDate = createdDate + 86400000;
        if(currentDate > createdDate && currentDate < nextDate){
          row.newTag = true;
        }
        else{
          row.newTag = false;
        }
      });
      let newTrueData = data.filter(item => item.newTag == true);
      newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
      let newFalseData = data.filter(item => item.newTag == false);
      Array.prototype.push.apply(newTrueData, newFalseData); 
      return newTrueData;
    }
    else{
      return data;
    }
  }

  public ngAfterViewInit() {
    setTimeout(() => {
    this.initMap();
    }, 0);
  }

  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    this.map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 50, lng: 5 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.map.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
    var ui = H.ui.UI.createDefault(this.map, defaultLayers);
  }

  onClose() {
    this.titleVisible = false;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onNewCorridor(){
    this.actionType = "create";
    this.createEditStatus = true;
    this.tabVisibility.emit(false);

  }
  
  editViewCorridor(rowData: any, type: any){
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditStatus = true;
  }

  deleteCorridor(rowData: any){
    // let packageId = rowData.id;
    // const options = {
    //   title: this.translationData.lblDelete || "Delete",
    //   message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
    //   cancelText: this.translationData.lblCancel || "Cancel",
    //   confirmText: this.translationData.lblDelete || "Delete"
    // };
    // this.dialogService.DeleteModelOpen(options, rowData.code);
    // this.dialogService.confirmedDel().subscribe((res) => {
    // if (res) {
    //   this.packageService.deletePackage(packageId).subscribe((data) => {
    //     this.openSnackBar('Item delete', 'dismiss');
    //     this.loadPackageData();
    //   })
    //     this.successMsgBlink(this.getDeletMsg(rowData.code));
    //   }
    // });
  }

  onBackToPage(_eventObj) {
    this.createEditStatus = false;

    this.tabVisibility.emit(true);
  }
}

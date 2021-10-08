import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-vehicle-update-details',
  templateUrl: './vehicle-update-details.component.html',
  styleUrls: ['./vehicle-update-details.component.less']
})
export class VehicleUpdateDetailsComponent implements OnInit {
  public selectedIndex: number = 0; 
  dataSource: any;
  displayedColumns: string[] = ['campaignId','subject','affectedSystem(s)','type','category','status','endDate', 'scheduledDateTime','action'];
  translationData: any= {};
  localStLanguage: any;
  initData: any = [];
  showLoadingIndicator: boolean = false;
  accountOrganizationId: any = 0;
  accountOrganizationSetting: any ;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private translationService: TranslationService) { 
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
  }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
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
      this.loadVehicleDetailsData();  
    }); 
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }
  loadVehicleDetailsData(){
    let vehicleUpdateDetails: any  = {
        vin: "XLR000000BE000080",
        vehicleSoftwareStatus: "Update running.",
        campaigns: [
          {
            campaignID: "EU-T000080",
            baselineAssignmentId: "475d9b10-a9c9-410e-8a26-a00d14169852",
            campaignSubject: "Rear light fix 1",
            systems: [
              "PCI-2"
            ],
            campaignType: "OTAUCRITICAL",
            campaignCategory: "Safety Recall",
            status: "Waiting for update condition",
            endDate: "",
            scheduleDateTime: 0
          },
          {
            campaignID: "EU-T000081",
            baselineAssignmentId: "88a0345c-80ad-4f97-beb1-98eb703efd78",
            campaignSubject: "Rear light fix 2",
            systems: [
              'PCI-2'
            ],
            campaignType: "OTAUCRITICAL",
            campaignCategory: "Safety Recall",
            status: "Waiting for update condition",
            endDate: "",
            scheduleDateTime: 0
          },
          {
            campaignID: "EU-T000088",
            baselineAssignmentId: "2bd2fdfe-3e9b-47c1-96ce-22f4a4d64120",
            campaignSubject: "PCI 2 fix",
            systems: [
              "PCI-2"
            ],
            campaignType: "OTAUCRITICAL",
            campaignCategory: "Safety Recall",
            status: "Waiting for update condition",
            endDate: "",
            scheduleDateTime: 0
          },
          {
            campaignID: "EU-T000089",
            baselineAssignmentId: "13cd89fb-48eb-4f0c-a80f-1c6aea4bac81",
            campaignSubject: "PCI Fix 3",
            systems: [
              "PCI-2"
            ],
            campaignType: "OTA Software Update",
            campaignCategory: "Safety Recall",
            status: "Waiting for update condition",
            endDate: "",
            scheduleDateTime: 0
          },
          {
            campaignID: "EU-T000101",
            baselineAssignmentId: "4dc741a0-43d8-4fed-8afd-38df75235547",
            campaignSubject: "PCI Fix (FM) 28-6 5",
            systems: [
              "PCI-2"
            ],
            campaignType: "OTA Software Update",
            campaignCategory: "Sales Option",
            status: "Waiting for update condition",
            endDate: "",
            scheduleDateTime: 0
          },
          {
            campaignID: "EU-T000103",
            baselineAssignmentId: "ced986b3-db3e-4012-832c-5f167d8d485a",
            campaignSubject: "PCI fix 29-6 2",
            systems: [
              "PCI-2"
            ],
            campaignType: "OTAUCRITICAL",
            campaignCategory: "Safety Recall",
            status: "Waiting for update condition",
            endDate: "",
            scheduleDateTime: 0
          },
          {
            campaignID: "EU-T000104",
            baselineAssignmentId: "41805a61-53a9-4938-8edf-d39ff4aa5c36",
            campaignSubject: "PCI fix 29-6 3",
            systems: [
              "PCI-2"
            ],
            campaignType: "OTAUCRITICAL",
            campaignCategory: "Safety Recall",
            status: "Installing",
            endDate: "",
            scheduleDateTime: 0
          }
        ]
    }
    this.initData= vehicleUpdateDetails.campaigns;
    this.updateDataSource(this.initData);

  }
  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      // this.dataSource.sortData = (data: String[], sort: MatSort) => {
      //   const isAsc = sort.direction === 'asc';
      //   return data.sort((a: any, b: any) => {
      //     return this.compare(a[sort.active], b[sort.active], isAsc);
      //   });
      //  }
    });
  }

  onCancel(){
    
  }
  applyFilter(filterValue: string){

  }
  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }
}

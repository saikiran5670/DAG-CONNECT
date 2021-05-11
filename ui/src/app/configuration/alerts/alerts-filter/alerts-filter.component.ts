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

  @Input() translationData: any = []; 
  @Input() alertCategoryList: any;
  @Input() alertTypeList: any; 
  @Input() vehicleList: any;
  @Input() alertCriticalityList:any;
  @Input() vehicleGroupList: any= [];
  @Input() alertStatusList: any= [];
  @Input() initData : any;

  isDisabledAlerts = true;
  selectedCategory: any;
  selectedType:any;
  localData : any;
  dataSource = new MatTableDataSource();

  filterAlertType: any;


  filterData: any;
  @Output() filterValues : EventEmitter<any> = new EventEmitter();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
 
  localStLanguage: any;
  accountOrganizationId: any;
  roleObj = { 
    Organizationid : this.OrgId,
    IsGlobal: this.isGlobal
 };

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
    });
    this.localData = this.initData ;
    this.dataSource = new MatTableDataSource(this.initData);    
  } 
 
  applyFilterOnAlertType(filterValue){   
   this.localData = this.initData; 
   if(filterValue=="All"){
    this.filterData = this.localData;
   } 
   else{   
   this.filterData = this.localData.filter(item => item.alertType == filterValue.value);
   }
   this.dataSource = new MatTableDataSource(this.filterData);
   this.filterValues.emit(this.dataSource);
 }


 applyFilterOnAlertCategory(filterValue){   
  this.localData = this.initData; 
  //this.filterAlertType=
  // this.localData.forEach(item => {
  //  // this.filterAlertType= item.category;
 
  //   if(item.category == filterValue.value){
  //     this.filterData.push(item);
  //   }
  //   else if(item.category == filterValue.value && item.alertType == filterValue.value){

  //   }
  // });

  if(filterValue=="All"){
   this.filterData = this.localData;
  } 
  else{
  this.filterData = this.localData.filter(item => item.category == filterValue.value);
  }
  this.dataSource = new MatTableDataSource(this.filterData);
  this.filterValues.emit(this.dataSource);
}


applyFilterOnAlertVehGroup(filterValue){   
  this.localData = this.initData; 
  if(filterValue=="All"){
   this.filterData = this.localData;
  } 
  else{
  this.filterData = this.localData.filter(item => item.vehicleGroup == filterValue.value);
  }
  this.dataSource = new MatTableDataSource(this.filterData);
  this.filterValues.emit(this.dataSource);
}


applyFilterOnAlertStatus(filterValue){   
  this.localData = this.initData; 
  if(filterValue=="All"){
   this.filterData = this.localData;
  } 
  else{
  this.filterData = this.localData.filter(item => item.status == filterValue);
  }
  this.dataSource = new MatTableDataSource(this.filterData);
  this.filterValues.emit(this.dataSource);
}


applyFilterOnAlertCriticality(filterValue){   
  
}

applyFilterOnAlertVehicle(filterValue){  
}



  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  public dataResultTypes: Array<{ productName: string, productId: number, categoryId: number }>;

  //public dataResultOrders: Array<{ orderName: string, orderId: number, productId: number, }>;
  
  //public selectedOrder: { orderName: string, orderId: number };

  handleCategoryChange(value) {
    this.selectedCategory = value;   
    this.selectedType = undefined;
    //this.selectedOrder = undefined;

    if(value == "All"){
          this.isDisabledAlerts = false;
          this.dataResultTypes = this.alertTypeList;
    }
    else{
        this.isDisabledAlerts = false;
        this.dataResultTypes = this.alertTypeList.filter((s) => s.parentEnum === value.enum);
     }
     this.applyFilter(value)
    // this.isDisabledOrders = true;
    // this.dataResultOrders = [];
    }

    applyFilter(filterValue) {
      filterValue.value = filterValue.value.trim(); // Remove whitespace    
      this.dataSource.filter= filterValue.value;
    }

   updatedTableData(tableData : any) {
    this.localData = tableData;
    this.dataSource = new MatTableDataSource(this.localData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  // handleProductChange(value) {
  //     this.selectedProduct = value;
  //     this.selectedOrder = undefined;

  //     if (value.productId === this.defaultItemProducts.productId) {
  //         this.isDisabledOrders = true;
  //         this.dataResultOrders = [];
  //     } else {
  //         this.isDisabledOrders = false;
  //         this.dataResultOrders = this.dataOrders.filter((s) => s.productId === value.productId);
  //     }
  // }

  // handleOrderChange(value) {
  //     this.selectedOrder = value;
  // }



}

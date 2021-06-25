import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TranslationService } from 'src/app/services/translation.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { SelectionModel } from '@angular/cdk/collections';
import { OrganizationService } from 'src/app/services/organization.service';
import { Router, ActivatedRoute,  } from '@angular/router';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-organisation-relationship',
  templateUrl: './organisation-relationship.component.html',
  styleUrls: ['./organisation-relationship.component.less']
})
export class OrganisationRelationshipComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @Output() createViewEditPackageEmit = new EventEmitter<object>();
  dataSource: any;
  orgrelationshipDisplayedColumns: string[]= ['select', 'relationshipName', 'vehicleGroupName', 'organizationName', 'startDate', 'endDate','allowChain', 'endRelationship'];
  editFlag: boolean = false;
  viewFlag: boolean = false;
  initData: any = [];
  rowsData: any;
  createStatus: boolean = false;
  titleText: string;
  translationData: any;
  grpTitleVisible : boolean = false;
  showLoadingIndicator: any;
  displayMessage: any;
  organizationId: number;
  localStLanguage: any;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  selectedOrgRelations = new SelectionModel(true, []);
  relationshipList: any = [];
  vehicleList: any = [];
  organizationList: any = [];
  startDateList: any = [];
  viewRelationshipName: any; 
  allTypes: any = [
    {
      name: 'Active'
    },
    {
      name: 'Terminated'
    }
  ];
  relationFilter= new FormControl();
  vehicleGrpFilter= new FormControl();
  orgFilter= new FormControl();
  typeFilter= new FormControl();
  searchFilter= new FormControl();
  filteredValues = {
    relation: '',
    vehicleGrp: '',
    org: '',
    type: '',
    search: ''
  };

  constructor(private translationService: TranslationService, private dialogService: ConfirmDialogService, private dialog: MatDialog, private organizationService: OrganizationService, private router: Router, private route: ActivatedRoute) { 
 this.defaultTranslation();
    this.route.queryParams.subscribe(params => {
      this.viewRelationshipName = params["name"]; 
   });
  }
  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 29 //-- for org relationship mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadInitData();
    });
    this.loadInitData();//temporary
    this.relationFilter.valueChanges.subscribe(filterValue => {
      if(filterValue==='allRelations'){
        this.filteredValues['relation'] = '';
      } else{
        this.filteredValues['relation'] = filterValue;
      }
      this.dataSource.filter = JSON.stringify(this.filteredValues);
    });
    this.vehicleGrpFilter.valueChanges.subscribe(filterValue => {
      if(filterValue==='allVehicle'){
        this.filteredValues['vehicleGrp'] = '';
      } else{
        this.filteredValues['vehicleGrp'] = filterValue;
      }
      this.dataSource.filter = JSON.stringify(this.filteredValues);
    });
    this.orgFilter.valueChanges.subscribe(filterValue => {
      if(filterValue==='allOrg'){
        this.filteredValues['org'] = '';
      } else{
        this.filteredValues['org'] = filterValue;
      }
      this.dataSource.filter = JSON.stringify(this.filteredValues);
    });
    this.typeFilter.valueChanges.subscribe(filterValue => {
      if(filterValue==='all'){
        this.filteredValues['type'] = '';
      } else if(filterValue === 'Active'){
        this.filteredValues['type'] = 'true';
      } else if(filterValue === 'Terminated'){
        this.filteredValues['type'] = 'false';
      }
      this.dataSource.filter = JSON.stringify(this.filteredValues);
    });
    this.searchFilter.valueChanges.subscribe(filterValue => {
      this.filteredValues['search'] = filterValue;
      this.dataSource.filter = JSON.stringify(this.filteredValues);
    });
  }

  loadInitData() {

        let objData = {
            Organization_Id: this.organizationId
              }
          this.showLoadingIndicator = true;
          this.organizationService.GetOrgRelationdetails(objData).subscribe((newdata: any) => {
          this.organizationService.getOrgRelationshipDetailsLandingPage().subscribe((data: any) => {
          this.hideloader();
            if(data)
            {
              if(this.viewRelationshipName!=undefined)
               {     
                  this.successMsgBlink(this.getEditSuccessMsg('Update', this.viewRelationshipName));
                  this.router.navigate(['/admin/organisationrelationshipmanagement']);                 
                }
                this.relationshipList = newdata["relationShipData"];
                this.organizationList = newdata["organizationData"];
                this.vehicleList =  newdata["vehicleGroup"];
                this.initData = data["orgRelationshipMappingList"];
                this.initData = this.getNewTagData(this.initData)
                  setTimeout(()=>{
                    this.dataSource = new MatTableDataSource(this.initData);
                    this.dataSource.paginator = this.paginator;
                    this.dataSource.sort = this.sort;
                    this.dataSource.filterPredicate = function(data, filter: any){
                      let val = JSON.parse(filter);
                        return (data.allowChain).toString().indexOf(val.type) !== -1 &&
                              data.orgRelationId.toString().indexOf(val.relation) !== -1 &&
                              data.targetOrgId.toString().indexOf(val.org) !== -1 &&
                              data.vehicleGroupID.toString().indexOf(val.vehicleGrp) !== -1 &&
                              ((data.relationshipName.toLowerCase().indexOf(val.search.toLowerCase()) !== -1 || 
                                data.vehicleGroupName.toLowerCase().indexOf(val.search.toLowerCase()) !== -1 ||
                                data.organizationName.toLowerCase().indexOf(val.search.toLowerCase()) !== -1 ||
                                (getDt(data.startDate)).toString().toLowerCase().indexOf(val.search.toLowerCase()) !== -1 ||
                                (getDt(data.endDate)).toString().toLowerCase().indexOf(val.search.toLowerCase()) !== -1 ||
                                getChaining(data.allowChain).indexOf(val.search.toLowerCase())) !== -1);

                      };
                    });
             }

            },(error)=>{
              this.hideloader();
            }
            );
          }
          ); 
    
  }
  getEditSuccessMsg(editText: any, name: any){
    if(editText == 'Update'){
      if(this.translationData.lblRelationshipdetailssuccessfullyupdated)
        return this.translationData.lblRelationshipdetailssuccessfullyupdated.replace('$', this.viewRelationshipName);
      else
        return ("Relationship '$' details successfully updated").replace('$', this.viewRelationshipName);
    } 
  }
  setDate(date : any){​​​​​​​​
    if (date === 0) {​​​​​​​​
        return '-';
        }​​​​​​​​
    else {​​​​​​​​
      var newdate = new Date(date);
      var day = newdate.getDate();
      var month = newdate.getMonth();
      var year = newdate.getFullYear();
      return (`${​​​​​​​​day}​​​​​​​​/${​​​​​​​​month + 1}​​​​​​​​/${​​​​​​​​year}​​​​​​​​`);
        }​​​​​​​​
      }​​​​​​​​

      exportAsCSV(){
        this.matTableExporter.exportTable('csv', {fileName:'OrganisationRelationship_Data', sheet: 'sheet_name'});
      }
    
      exportAsPdf() {
        let DATA = document.getElementById('organisationRelationData');
          
        html2canvas(DATA).then(canvas => {
            
            let fileWidth = 208;
            let fileHeight = canvas.height * fileWidth / canvas.width;
            
            const FILEURI = canvas.toDataURL('image/png')
            let PDF = new jsPDF('p', 'mm', 'a4');
            let position = 0;
            PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)
            
            PDF.save('OrganisationRelationship_Data.pdf');
            PDF.output('dataurlnewwindow');
        });     
      }

  defaultTranslation () {
    this.translationData = {
      lblOrganisationRelationshipDetails : "Organisation Relationship Details",
      lblSearch: "Search",
      lblNewRelationship: "New Relationship",
      lblNoRecordFound: "No Record Found",
      lblOrganisationName: "Organisation Name",
      lblVehicleGroup: "Vehicle Group Name",
      lblSelectVehicleGroup: "Select Vehicle Group",
      lblSelectOrganisation: "Select Organisation"

    }
  }
  processTranslation(transData: any){   
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  newRelationship(){
    this.editFlag = false;
    this.createStatus = true;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); 
    filterValue = filterValue.toLowerCase(); 
    this.dataSource.filter = filterValue;
  }

  applyFilterOnRelationship(filterValue: string){
    if(filterValue == "allRelations"){
      this.dataSource.filter = '';
    }
    else{
    this.dataSource.filterPredicate = function(data, filter: string): boolean {
      return data.relationShipId === filter;
    }; this.dataSource.filter = filterValue; 
      }
  }

  applyFilterOnVehicle(filterValue: string){
    if(filterValue == "allVehicle"){
      this.dataSource.filter = '';
    }
    else{
    this.dataSource.filterPredicate = function(data, filter: string): boolean {
      return data.vehicleGroupID === filter;
    };  
    this.dataSource.filter = filterValue;
    }
  }

  applyFilterOnOrganisation(filterValue: string){
    if(filterValue == "allOrg"){
      this.dataSource.filter = '';
    }
    else{
    this.dataSource.filterPredicate = function(data, filter: string): boolean {
      return data.targetOrgId === filter;
    };  
    this.dataSource.filter = filterValue;
    }
  }

  applyFilterOnType(filterValue: string){
    if(filterValue == 'all')
    {
     this.dataSource.filter = '';
    }
    else{
    this.dataSource.filterPredicate = function(data, filter: string): boolean {
      if(filterValue == 'Terminated')
      {
      if(data.endDate != 0)
      {
        return data.endDate != 0;
      }
    }

    if(filterValue == 'Active')
      {
      if(data.endDate == 0)
      {
        return data.endDate == 0;
      }
    }

    };  
    this.dataSource.filter = filterValue;
    }
  }

  onClose(){
    this.grpTitleVisible = false;
  }

  changeStatus(data) {

  }
  // editViewFeature(data, viewEdit){

  // }
  deleteRow(rowData){
    let selectedOptions = [rowData.id];
    const options = {
      title: this.translationData.lblDeleteAccount || 'Delete',
      message: this.translationData.lblAreyousureyouwanttodeleterelationship || "Do you want to end '$' relationship?",
      cancelText: this.translationData.lblNo || 'No',
      confirmText: this.translationData.lblYes || 'Yes'
    };
    let name = rowData.relationshipName;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
       {
        this.organizationService
        .deleteOrgRelationship(selectedOptions) 
        .subscribe((d) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadInitData();
        });
        }
    }
  });

  }

 

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

  // viewRelationship(row: any){
  //   this.titleText = this.translationData.lblRelationshipDetails || "Relationship Details";
  //   this.editFlag = true;
  //   this.viewFlag = true;
  //   this.rowsData = [];
  //   this.rowsData.push(row);
  // }

  changeOrgRelationStatus(rowData: any){
    if(rowData.endDate == 0)
    {
    const options = {
      title: this.translationData.lblAlert || "Alert",
      message: this.translationData.lblYouwanttoDetails || "You want to # '$' Details?",
      // cancelText: this.translationData.lblNo || "No",
      // confirmText: this.translationData.lblYes || "Yes",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.allowChain == true) ? this.translationData.lblDeactivate || " Deactivate" : this.translationData.lblActivate || " Activate",
      status: rowData.allowChain == true ? 'Inactive' : 'Active' ,
      name: rowData.relationshipName
    };
   
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){ 
        let objData = {
          "orgRelationID": rowData.id,
          "allowChaining": !rowData.allowChain 
        }
        this.organizationService.updateAllowChain(objData).subscribe((data) => {
          this.loadInitData();
        })
      }
    });
  }
    
  }

  deleteOrgRelationship(){
    let relList: any = '';
    let relId = 
    { 
      id: this.selectedOrgRelations.selected.map(item=>item.id)
    }
    const options = {
      title: this.translationData.lblDelete || 'Delete',
      message: this.translationData.lblAreyousureyouwanttodeleterelationship || "Do you want to end  '$' relationship?",
      cancelText: this.translationData.lblNo || 'No',
      confirmText: this.translationData.lblYes || 'Yes'
    };
    //let name = this.selectedOrgRelations.selected[0].relationshipName;
    let name = this.selectedOrgRelations.selected.forEach(item => {
      relList += item.relationshipName + ', ';
    });
    if(relList != ''){
      relList = relList.slice(0, -2);
    }
    this.dialogService.DeleteModelOpen(options, relList);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
       {
        this.organizationService.deleteOrgRelationship(relId) //need to change
        .subscribe((d) => {
          this.successMsgBlink(this.getDeletMsg(relList));
          this.loadInitData();
        });
        }
    }
  });
}

  getDeletMsg(relationshipName: any){
    if(this.translationData.lblRelationshipwassuccessfullydeleted)
      return this.translationData.lblRelationshipDelete.replace('$', relationshipName);
    else
      return ("Relationship '$' was successfully deleted").replace('$', relationshipName);
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  masterToggleForOrgRelationship() {
    this.isAllSelectedForOrgRelationship()
      ? this.selectedOrgRelations.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedOrgRelations.select(row)
      );
  }

  isAllSelectedForOrgRelationship() {
    const numSelected = this.selectedOrgRelations.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForOrgRelationship(row?: any): string {
    if (row)
      return `${this.isAllSelectedForOrgRelationship() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedOrgRelations.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  checkCreationForOrgRelationship(item: any){
    this.createStatus = item.stepFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.initData = item.tableData["orgRelationshipMappingList"];
      // this.initData = this.getNewTagData(this.initData)
          this.dataSource = new MatTableDataSource(this.initData);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
    }
    this.loadInitData();
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
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

}

function getDt(date){
    if (date === 0) {​​​​​​​​
      return '-';
    }​​​​​​​​
    else {​​​​​​​​
      var newdate = new Date(date);
      var day = newdate.getDate();
      var month = newdate.getMonth();
      var year = newdate.getFullYear();
      return (`${​​​​​​​​day}/${​​​​​​​​month + 1}/${​​​​​​​​year}​​​​​​​​`);
    }​​​​​​​​
}

function getChaining(data: any){
  if(((data.toString()).toLowerCase()) === 'true'){
    return 'true active';
  } else if (((data.toString()).toLowerCase()) === 'false'){
    return 'false inactive';
  } else {
    return (data.toString()).toLowerCase();
  }
}


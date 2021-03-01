import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import * as XLSX from 'xlsx';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslationService } from 'src/app/services/translation.service';
import { FileValidator } from 'ngx-material-file-input';
import { MatTableDataSource } from '@angular/material/table';


@Component({
  selector: 'app-translation-data-upload',
  templateUrl: './translation-data-upload.component.html',
  styleUrls: ['./translation-data-upload.component.less']
})
export class TranslationDataUploadComponent implements OnInit {
  grpTitleVisible : boolean = false;
  userCreatedMsg : any;
  accountOrganizationId: any = 0;
  dataSource: any;
  initData: any = [];
  displayedColumns: string[] = ['fileName','uploadedDate','fileSize','description','action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  uploadTranslationDataFormGroup: FormGroup;
  templateFileUrl: string = 'assets/docs/driverTemplate.xlsx';
  templateFileName: string = 'driver-Template.xlsx';
  //@ViewChild('UploadFileInput') uploadFileInput: ElementRef;
  readonly maxSize = 104857600;
  rowData: any;
  file: any;
  arrayBuffer: any;
  filelist: any;
  translationData: any;
  localStLanguage: any;
  type: any = '';
  showLoadingIndicator: any;
  isTranslationDataUploaded: boolean = false;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private translationService: TranslationService) { 
      this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      // lblSearch: "Search",
      // lblConsent: "Consent",
      // lblAction: "Action", 
      // lblCancel: "Cancel",
      // lblConfirm: "Confirm",
      // lblReset: "Reset",
      // lblNew: "New",
      // lblSave: "Save",
      // lblDriverManagement: "Driver Management",
      // lblImportNewDrivers: "Import New Drivers",
      // lblDownloadaTemplate: "Download a Template",
      // lblDownloadaTemplateMessage: "You can enter multiple driver records. New Driver IDs records will be added and existing Driver ID records will be updated. Few fields are mandatory; the rest optional – please see the template for details. All the fields (with the exception of the Driver ID) can be edited later from the Driver Management screen shown below.",
      // lblUploadupdateddriverdetailsandselectgroupfordefiningcategory: "Upload updated driver details and select group for defining category",
      // lblSelectUserGroupOptional: "Select User Group (Optional)",
      // lblUploadUpdatedExcelFile: "Upload Updated Excel File",
      // lblBrowse: "Browse",
      // lblImport: "Import",
      // lblSelectUserGroup: "Select User Group",
      // lblDriverDetails: "Driver Details",
      // lblDrivers: "Drivers",
      // lblDriverID: "Driver ID",
      // lblDriverName: "Driver Name",
      // lblEmailID: "Email ID",
      // lblUserGroup: "User Group",
      // lblOptInAll: "Opt-In All",
      // lblOptOutAll: "Opt-Out All",
      // lblOptIn: "Opt-In",
      // lblOptOut: "Opt-Out",
      // lblImportedFileDetails: "Imported File Details",
      // lblImportedUpdateddriverrecords: "Imported/Updated '$' driver records",
      // lblRejecteddriverrecordsduetofollowingerrors: "Rejected '$' driver records due to following errors",
      // lblRole: "Role",
      // lblnewdrivers: "new drivers",
      // lblEditDriverDetails: "Edit Driver Details",
      // lblDriverIDConsentStatus: "Driver ID Consent Status",
      // lblAlldetailsaremandatory: "All details are mandatory",
      // lblSalutation: "Salutation",
      // lblFirstName: "First Name",
      // lblLastName: "Last Name",
      // lblBirthDate: "Birth Date",
      // lblLanguage: "Language",
      // lblUnits: "Units", 
      // lblTimeZone: "Time Zone",
      // lblCurrency: "Currency",
      // lblDriverIDConsent: "Driver ID Consent",
      // lblOrganisation: "Organisation",
      // lblTotalDrivers: "Total Drivers",
      // lblCurrentConsentStatusForSubscriber: "Current Consent Status For Subscriber ",
      // lblOptOutMessage: "Now you are proceeding with Driver ID Consent Opt-Out operation!, Click 'Confirm' to change the consent status.",
      // lblOptInOutChangeMessage: "You are currently in '$' mode. This means no personal data from your driver(s) such as the driver ID are visible in the DAF CONNECT portal.",
      // lblConsentExtraMessage: "By selecting and confirming '$' mode (i.e. by checking the opt-in checkbox) personal data such as the driver ID from your driver(s) will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.",
      // lblConsentNote: "Please also refer to the DAF CONNECT terms & conditions for more information.",
      // lblName: "Name",
      // lblDriverrecordupdated: "Driver record updated",
      // lblErrorinupdatingdriverrecordPleasetryagain: "Error in updating driver record '$'. Please try again.",
      // lblDeleteDriver: "Delete Driver ",
      // lblAreyousureyouwanttodeletedriver: "Are you sure you want to delete driver '$'?",
      // lblDriversuccessfullydeleted: "Driver '$' successfully deleted",
      // lblErrordeletingdriver: "Error deleting driver",
      // lblThedriverwasoptedinsuccessfully: "The driver '$' was opted-in successfully",
      // lblThedrivercouldnobeoptedin: "The driver could not be opted-in '$'",
      // lblThedriverwasoptedoutsuccessfully: "The driver '$' was opted-out successfully",
      // lblThedrivercouldnobeoptedout: "The driver could not be opted-out '$'"
    }
  }

  ngOnInit(){
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.uploadTranslationDataFormGroup = this._formBuilder.group({
      uploadFile: [
        undefined,
        [Validators.required, FileValidator.maxContentSize(this.maxSize)]
      ]
    });

    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 0 //-- for common & user preference. menuid for driver will be add later
    }

    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      //this.mockData();
      //this.loadUsersData();
    });
  }

  mockData(){
    this.initData = [
      {
        driverId: "IN 0000000000000001",
        firstName: "Driver",
        lastName: "1",
        birthDate: "01/01/2001",
        consentStatus: 'Opt-In',
        salutation: "Mr"
      },
      {
        driverId: "IN 0000000000000002",
        firstName: "Driver",
        lastName: "2",
        birthDate: "02/02/2002",
        consentStatus: 'Opt-Out',
        salutation: "Ms"
      },
      {
        driverId: "IN 0000000000000003",
        firstName: "Driver",
        lastName: "3",
        birthDate: "03/03/2003",
        consentStatus: 'Opt-In',
        salutation: "Mrs"
      }
    ];
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  updateGridData(tableData: any){
    this.initData = tableData; 
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  uploadTranslationData(){ 
    this.validateExcelFileField();
    //TODO : Read file, parse into JSON and send to API
    this.isTranslationDataUploaded = true;

  }

  validateExcelFileField(){
    console.log("filelist:: ", this.filelist)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.userCreatedMsg = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  addfile(event)     
  {    
    this.file = event.target.files[0];     
    let fileReader = new FileReader();    
    fileReader.readAsArrayBuffer(this.file);     
    fileReader.onload = (e) => {    
        this.arrayBuffer = fileReader.result;    
        var data = new Uint8Array(this.arrayBuffer);    
        var arr = new Array();    
        for(var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);    
        var bstr = arr.join("");    
        var workbook = XLSX.read(bstr, {type:"binary"});    
        var first_sheet_name = workbook.SheetNames[0];    
        var worksheet = workbook.Sheets[first_sheet_name];    
        //console.log(XLSX.utils.sheet_to_json(worksheet,{raw:true}));    
          var arraylist = XLSX.utils.sheet_to_json(worksheet,{raw:true});     
              this.filelist = [];
              this.filelist = arraylist;    
    }    
  }

  onClose(){
    this.isTranslationDataUploaded = false;
  }

  onDownloadExcel(row: any){

  }

  onCloseMsg(){
    this.grpTitleVisible = false;
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }


}

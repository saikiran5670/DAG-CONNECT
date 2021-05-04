import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { FileValidator } from 'ngx-material-file-input';
import { TranslationService } from 'src/app/services/translation.service';
import * as FileSaver from 'file-saver';
import { base64ToFile } from 'ngx-image-cropper';

@Component({
  selector: 'app-terms-conditions-management',
  templateUrl: './terms-conditions-management.component.html',
  styleUrls: ['./terms-conditions-management.component.less']
})
export class TermsConditionsManagementComponent implements OnInit {

  grpTitleVisible : boolean = false;
  fileUploadedMsg : any;
  accountOrganizationId: any = 0;
  dataSource: any;
  initData: any = [];
  displayedColumns: string[] = ['firstName','versionno','action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  uploadTermsConditionsFormGroup: FormGroup;
  readonly maxSize = 104857600;
  rowData: any;
  file: any;
  arrayBuffer: any;
  filelist: any= [];
  translationData: any;
  localStLanguage: any;
  type: any = '';
  showLoadingIndicator: any;
  isTranslationDataUploaded: boolean = false;
  pdfEmptyMsg: boolean = false;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  uploadFileErrorCode: number;
  downloadPDFErrorCode: number;
  greaterVersionPresentMsg: string;
  
  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private translationService: TranslationService) { 
      this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      
    }
  }

  ngOnInit(){
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.uploadTermsConditionsFormGroup = this._formBuilder.group({
      uploadFile: [
        undefined,
        [Validators.required, FileValidator.maxContentSize(this.maxSize)]
      ],
      fileDescription: []
    });

    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 41 
    }

    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadInitData();
    })
  }

  loadInitData(){
    let objData= {
      OrganizationId : this.accountOrganizationId
    }
    
    this.showLoadingIndicator = true;
    this.translationService.getUserAcceptedTC(objData).subscribe((data: any) => {
      this.hideloader();
      if(data){
        this.initData = data;
        this.updateGridData(this.initData);
      }
    }, (error) => {
      this.hideloader();
      this.initData = [];
    //  this.updateGridData(this.initData);
    })
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

  uploadTermsAndConditions(){ 
    let languageData = [];
    
   
    let tncObj= {
      "start_date": "",
      "end_date": "",
      "created_by": parseInt(localStorage.getItem("accountId")),
      "_data": this.filelist
    }

    this.translationService.uploadTermsAndConditions(tncObj).subscribe(data => {
      if(data){
        if(data[0].action.includes("No action")){
          let latestVersion = (data[0]["action"].split(":")[1]).split("_")[0];
          this.greaterVersionPresentMsg = "T&C with greater version is already present for this language. Latest version is : "+latestVersion;
        }
        else{
          let msg= this.translationData.lblTermsAndConditionsFileSuccessfullyUploaded ? this.translationData.lblTermsAndConditionsFileSuccessfullyUploaded : "Terms and Conditions file successfully uploaded";
          this.successMsgBlink(msg);
          this.filelist= [];
          this.uploadTermsConditionsFormGroup.controls.uploadFile.setValue("");
        }
      }
    }, (error) => {
      this.uploadFileErrorCode = error.status;
      this.filelist= [];
    });   
    
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.fileUploadedMsg = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  addfile(event)     
  {    
    this.greaterVersionPresentMsg= "";
    this.uploadFileErrorCode = 0;
    for(let i= 0; i < event.target.files.length; i++){

      const reader = new FileReader();
      reader.readAsDataURL(event.target.files[i]);
      reader.onload = () => {
        let fileSelected= reader.result.toString();
          if(fileSelected.length > 5) //By default value in  fileSelected variable is "data:". Hence minlength is 5
        {
          this.pdfEmptyMsg = false;
          this.filelist.push({"fileName": (this.uploadTermsConditionsFormGroup.controls.uploadFile.value._fileNames).replace(".pdf", ""), "description": fileSelected.split(",")[1]})
        } 
        else{
          this.pdfEmptyMsg = true;
        } 
      };  
    }
           
  }   

  onClose(){
    this.isTranslationDataUploaded = false;
  }

  onDownloadPdf(row: any){
    let objData= {
      versionNo: row.versionno,
      languageCode: JSON.parse(localStorage.getItem("language")).code
    }
    this.translationService.getTCForVersionNo(objData).subscribe(response => {
      let arrayBuffer= response[0].description;
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      const linkSource = 'data:application/pdf;base64,' + base64File;
      const downloadLink = document.createElement("a");
      const fileName = "TermsConditions_"+row.versionno+"_"+(JSON.parse(localStorage.getItem("language")).code).split("-")[0]+".pdf";

      downloadLink.href = linkSource;
      downloadLink.download = fileName;
      downloadLink.click();
    }, (error) => {
      this.downloadPDFErrorCode= error.status;
    });
    
  }

  onCloseMsg(){
    this.grpTitleVisible = false;
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }
}

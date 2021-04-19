import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { FileValidator } from 'ngx-material-file-input';
import { TranslationService } from 'src/app/services/translation.service';
import * as FileSaver from 'file-saver';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

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
  displayedColumns: string[] = ['fileName','createdAt','fileSize','description','action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  uploadTermsConditionsFormGroup: FormGroup;
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
  pdfEmptyMsg: boolean = false;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  
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
    let data = [{
      userName: "User1 Test1",
      version: "V1"
    },
    {
      userName: "User2 Test2",
      version: "V2"
    },
    {
      userName: "User3 Test3",
      version: "V3"
    }]
    this.showLoadingIndicator = true;
   // this.translationService.getTranslationUploadDetails().subscribe((data: any) => {
      this.hideloader();
      if(data){
        this.initData = data;
        this.updateGridData(this.initData);
      }
    //}, (error) => {
    //   this.hideloader();
    //   this.initData = [];
    //   this.updateGridData(this.initData);
    // })
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
    let languageData = [];
    console.log("filelist:: ", this.filelist);
    //TODO : Convert PDF to byte array and send to API
   

    let langObj = {
      "file_name": this.uploadTermsConditionsFormGroup.controls.uploadFile.value._fileNames,
      "file": languageData,
    }

    // this.translationService.importTranslationData(langObj).subscribe(data => {
    //   if(data){
    //     let msg= this.translationData.lblTranslationFileSuccessfullyUploaded ? this.translationData.lblTranslationFileSuccessfullyUploaded : "Translation file successfully uploaded";
    //     this.successMsgBlink(msg);
    //   }
    // }, (error) => {
      
    // });
    
    
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
    if(event.target.files[0]){
      this.pdfEmptyMsg = false;
      this.file = event.target.files[0];     
      let fileReader = new FileReader();    
      fileReader.readAsArrayBuffer(this.file);     
      fileReader.onload = (e) => {    
        this.arrayBuffer = fileReader.result;    
        var data = new Uint8Array(this.arrayBuffer);    
        var arr = new Array();    
        // for(var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);    
        // var bstr = arr.join("");    
        // var workbook = XLSX.read(bstr, {type:"binary"});    
        // var first_sheet_name = workbook.SheetNames[0];    
        // var worksheet = workbook.Sheets[first_sheet_name];    
        // var arraylist = XLSX.utils.sheet_to_json(worksheet,{raw:true});     
        this.filelist = [];
        //this.filelist = arraylist;  
        if(this.filelist.length > 0){
          this.pdfEmptyMsg = false;
        } 
        else{
          this.pdfEmptyMsg = true;
        }        
      }   
    }
  }

  onClose(){
    this.isTranslationDataUploaded = false;
  }

  onDownloadPdf(row: any){
    
  }


  private saveAsPDFFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });
    FileSaver.saveAs(data, fileName);
  }

  onCloseMsg(){
    this.grpTitleVisible = false;
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  uploadTermsAndConditions(){
    
  }






}

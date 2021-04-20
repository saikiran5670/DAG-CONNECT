import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { TranslationService } from 'src/app/services/translation.service';
import { FileValidator } from 'ngx-material-file-input';
import * as FileSaver from 'file-saver';
import { Workbook } from 'exceljs';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-dtc-translation',
  templateUrl: './dtc-translation.component.html',
  styleUrls: ['./dtc-translation.component.less']
})

export class DtcTranslationComponent implements OnInit {
  dtcTranslationFormGroup: FormGroup;
  translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  excelEmptyMsg: boolean = false;
  readonly maxSize = 104857600;
  file: any;
  arrayBuffer: any;
  filelist: any = [];
  loginAccountId: any;
  serverError: any = false;
  successMsg: any = false;
  errorMsg: any = '';

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private translationService: TranslationService) { 
      this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblImportDTCTranslation: "Import DTC Translation",
      lblDownloadaTemplateMessage: 'You can enter multiple dtc translation records.'
    } 
  }

  ngOnInit() {
    this.loginAccountId = parseInt(localStorage.getItem("accountId"));
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.dtcTranslationFormGroup = this._formBuilder.group({
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
      menuId: 42 //-- DTC Translation
    }

    this.translationService.getMenuTranslations(translationObj).subscribe( (data: any) => {
      this.processTranslation(data);
    });
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  addfile(event: any){ 
    this.excelEmptyMsg = false;   
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
        //console.log("this.filelist:: ", this.filelist);
    }    
  }

  importDTCTranslation(clearInput: any){ 
    if(this.filelist.length > 0){
      this.excelEmptyMsg = false;
      this.serverError = false;
      let transUploadData: any = [];
      this.filelist.forEach(element => {
        transUploadData.push(this.getImportData(element));
      });
      this.translationService.importDTCTranslationData({ dtcWarningToImport: transUploadData }).subscribe((uploadedData: any) => {
        console.log("uploadedData:: ", uploadedData);
        this.successMsg = true;
        setTimeout(() => {  
          this.successMsg = false;
          clearInput.clear();
        }, 5000);
      }, (error) => {
        this.serverError = true;
        if(error.status == 400){
          this.errorMsg = this.translationData.lblImportingViolationError || 'Violates foreign key constraint for Icon_ID, Please enter valid data for Warning_Class and Warning_Number';
        }else{
          this.errorMsg = this.translationData.lblImportingError || 'Importing Error';
        }
      });
    }else{
      console.log("Empty Excel File...");
      this.excelEmptyMsg = true;
      clearInput.clear();
    }
  }

  onClose(){
    this.successMsg = false;
  }

  getImportData(item: any){
    if(item['Warning Advice']){
      item['Warning Advice'] = item['Warning Advice'].replace("\"", "\\u022");
    }
    if(item['Warning Description']){
      item['Warning Description'] = item['Warning Description'].replace("\"", "\\u022");
    }
    let obj = {
      id: 0,
      code: item['Warning Language'],
      type: "D",
      veh_type: "",
      warning_class: Number(item['Warning Class']),
      number: Number(item['Warning Number']),
      description: item['Warning Description'],
      advice: item['Warning Advice'],
      icon_id: 0,
      expires_at: 0,
      created_at: 0,
      created_by: this.loginAccountId,
      modify_at: 0,
      modify_by: 0
    }
    return obj;
  }

  onBrowse(){
    this.serverError = false;
  }
}

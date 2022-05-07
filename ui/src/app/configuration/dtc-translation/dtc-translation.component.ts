import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TranslationService } from 'src/app/services/translation.service';
import { FileValidator } from 'ngx-material-file-input';
import * as FileSaver from 'file-saver';
import { Workbook } from 'exceljs';
import * as XLSX from 'xlsx';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

@Component({
  selector: 'app-dtc-translation',
  templateUrl: './dtc-translation.component.html',
  styleUrls: ['./dtc-translation.component.less']
})

export class DtcTranslationComponent implements OnInit {
  dtcTranslationFormGroup: FormGroup;
  translationData: any ={};
  localStLanguage: any;
  accountOrganizationId: any;
  excelEmptyMsg: boolean = false;
  svgEmptyMsg: boolean = false;
  readonly maxSize = 104857600;
  file: any;
  arrayBuffer: any;
  filelist: any = [];
  loginAccountId: any;
  serverError: any = false;
  serverIconError: any = false;
  successMsg: any = false;
  successIconMsg: any = false;
  errorMsg: any = '';
  errorIconMsg: any = '';
  downloadTransData: any=[];
  loadTransData: any=[];
  templateFileName: string = 'DTC-Translation-Template.xlsx';
  zip: any;
  unzipfiles: any = [];
  uploadIconList: any ;
  requiredIcon: any = false;
  requiredFile: any = false;

  constructor(private _formBuilder: FormBuilder, private dialog: MatDialog, private translationService: TranslationService) { }

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
    
    let menuId = 'menu_42_' + this.localStLanguage.code;
    if(!localStorage.getItem(menuId)){
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
        this.loadTranslationData();
      });
    } else{
      this.translationData = JSON.parse(localStorage.getItem(menuId));
      this.loadTranslationData();
    }
      
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_42_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
  } 

  loadTranslationData(){ 
    this.translationService.getdtcWarningDetails().subscribe((getTransData: any) => {
      this.loadTransData = getTransData;  
    });
  }
  addDtcDatafile(event: any){ 
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
        var arraylist = XLSX.utils.sheet_to_json(worksheet,{raw:true});     
        this.filelist = [];
        this.filelist = arraylist;
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

    if(transUploadData.length > 0){
       this.translationService.importDTCTranslationData({ dtcWarningToImport: transUploadData }).subscribe((importedData: any) => {
        this.successMsg = true;
        this.loadTranslationData();
        setTimeout(() => {  
          this.successMsg = false;
          clearInput.clear();
        }, 5000);
      }, (error) => {
        this.serverError = true;
        if(error.status == 400){
          if(error.error.title== "One or more validation errors occurred.")
          {
            this.errorMsg = this.translationData.lblImportingViolationError || 'The "Warning Language" field is required for each row of data'
          }
          else{
          this.errorMsg = this.translationData.lblImportingViolationError || 'Violates foreign key constraint for Icon_ID, Please enter valid data for Warning_Class and Warning_Number';
        }
      }else{
          this.errorMsg = this.translationData.lblImportingError || 'Importing Error';
        }
      });
    }
    }else{
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
  onIconBrowse(){
    this.serverIconError = false;
  }

  downloadTranslatedData(){
    const header = [this.translationData.lblWarningClass || 'Warning Class', this.translationData.lblWarningNumber || 'Warning Number', this.translationData.lblWarningLanguage || 'Warning Language', this.translationData.lblWarningDescription || 'Warning Description', this.translationData.lblWarningAdvice || 'Warning Advice'];
    let data = [];
    this.downloadTransData = this.loadTransData.dtcGetDataResponse;  
    if(this.downloadTransData.length > 0){
      this.downloadTransData.forEach(item => {
        data.push([item.warningClass, item.number, item.code, item.description, item.advice]);
      });      
    }
    else{
      data = [
        ['4', '5', "BG", "Незабавно угасете двигателя", "Това предупреждение може да предлага следните текстови опис dsffd \\u022 sdfafd dsfdsf."]
      ];
    }

    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Translated Data Template');
    //Add Header Row
    let headerRow = worksheet.addRow(header);
    // Cell Style : Fill and Border
    headerRow.eachCell((cell, number) => {
      if(number != 6){
        cell.fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FF0A3175' },
          bgColor: { argb: 'FF0000FF' }
        }
        cell.font = {
          color: { argb: 'FFFFFFFF'},
          bold: true
        }
      }
      cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    }); 
    // Add Data and Conditional Formatting
    data.forEach(d => {
      let row = worksheet.addRow(d);
    });
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: EXCEL_TYPE });
      FileSaver.saveAs(blob, this.templateFileName);
    }); 
  }
  
  getuploadIconData(item: any){ 
   let obj = {
      name: item['FileName'],
      icon: String (item['FileSize']),
      modifiedAt: 0,
      modifiedBy: 0      
    }   
    return obj;
  }
  getZipContent(files) {   
    this.singleIconSvgFile(files);
    if(this.unzipfiles.length == 0){
    const jsZip = require('jszip');
    jsZip.loadAsync(files[0]).then((zip) => { // <----- HERE
      Object.keys(zip.files).forEach((filename) => { // <----- HERE  
        let newFileSize= zip.files[filename]._data.uncompressedSize;
        let newFileName=zip.files[filename].name;
        if(newFileName.includes('.svg')){
        this.unzipfiles.push({'FileName': newFileName, 'FileSize':newFileSize});
        }       
      });
    }); 
  }  
  this.uploadIconList = []; 
  this.uploadIconList =  this.unzipfiles;  
}
  singleIconSvgFile(files) {     
    Object.keys(files).forEach((filename)=> { 
      let newFileSize= files[filename].size;
      let newFileName= files[filename].name;
      if(newFileName.includes('.svg')){
       this.unzipfiles.push({'FileName': newFileName, 'FileSize':newFileSize});
        } 
   });    
  }
  uploadIconTranslation(clearInput: any){ 
    if(this.uploadIconList.length > 0){
      this.svgEmptyMsg = false;
      this.serverIconError = false;
      let transUploadData: any = [];
      this.uploadIconList.forEach(element => {
        transUploadData.push(this.getuploadIconData(element));
      });
      this.translationService.updatedtcIconDetails({ dtcWarningUpdateIcon: transUploadData }).subscribe((uploadedData: any) => {     
        this.successIconMsg = true;        
        setTimeout(() => {         
          this.successIconMsg = false;        
          clearInput.clear();
        }, 5000);
      }, (error) => {
        this.serverIconError = true;
        if(error.status == 400){
          this.errorIconMsg = this.translationData.lblImportingViolationError || 'The svg file value could not be converted to Byte[ ]. Please enter valid svg files';
        }else{
          this.errorIconMsg = this.translationData.lblImportingError || 'Importing Error';
        }
      });
    }else{
      alert(`${this.translationData.lblsvgfilenotfound || 'svg file not found.'}`);
      clearInput.clear();
    }
  }
}



import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import * as XLSX from 'xlsx';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslationService } from 'src/app/services/translation.service';
import { FileValidator } from 'ngx-material-file-input';
import { MatTableDataSource } from '@angular/material/table';
import * as FileSaver from 'file-saver';
import { LanguageSelectionComponent } from './language-selection/language-selection.component';
import { stringify } from '@angular/compiler/src/util';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

@Component({
  selector: 'app-translation-data-upload',
  templateUrl: './translation-data-upload.component.html',
  styleUrls: ['./translation-data-upload.component.less']
})
export class TranslationDataUploadComponent implements OnInit {
  grpTitleVisible : boolean = false;
  fileUploadedMsg : any;
  accountOrganizationId: any = 0;
  dataSource: any;
  initData: any = [];
  displayedColumns: string[] = ['fileName','createdAt','fileSize','description','action'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  uploadTranslationDataFormGroup: FormGroup;
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
  dialogRef: MatDialogRef<LanguageSelectionComponent>;
  excelEmptyMsg: boolean = false;

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
    this.uploadTranslationDataFormGroup = this._formBuilder.group({
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
      menuId: 0 //-- for common & user preference. menuid for driver will be add later
    }

    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
      this.processTranslation(data);
      this.loadInitData();
    })
  }

  loadInitData(){
    this.translationService.getTranslationUploadDetails().subscribe(data => {
      if(data){
        data["translationupload"].forEach(element => {
          var date = new Date(element.createdAt);
          var year = date.getFullYear();
          var month = ("0" + (date.getMonth() + 1)).slice(-2);
          var day = ("0" + date.getDate()).slice(-2);
      
         element.createdAt= `${day}/${month}/${year}`;   
        });
        

        this.initData = data["translationupload"];
        this.updateGridData(this.initData);
      }
    }, (error) => {

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

  uploadTranslationData(){ 
    let languageData = [];
    console.log("filelist:: ", this.filelist);
    //TODO : Read file, parse into JSON and send to API
    this.filelist.forEach(element => {
      let tempArr = this.manipulateObjectForXLSXToJSON(element);
      if(tempArr.length > 0){
        tempArr.forEach(element1 => {
          languageData.push(element1)
        });
      }
    });

    let langObj = {
      "file_name": this.uploadTranslationDataFormGroup.controls.uploadFile.value._fileNames,
      "description": this.uploadTranslationDataFormGroup.controls.fileDescription.value? this.uploadTranslationDataFormGroup.controls.fileDescription.value : "",
      "file_size": this.file.size,
      "failure_count": 0,
      "file": languageData,
      "added_count": 0,
      "updated_count": ""
    }

    this.translationService.importTranslationData(langObj).subscribe(data => {
      if(data){
        console.log(data);
        let msg= this.translationData.lblTranslationFileSuccessfullyUploaded ? this.translationData.lblTranslationFileSuccessfullyUploaded : "Translation file successfully uploaded";
        this.successMsgBlink(msg);
        this.isTranslationDataUploaded = true;
      }
    }, (error) => {
      
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
    if(event.target.files[0]){
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
        if(this.filelist.length > 0){
          this.excelEmptyMsg = false;
        } 
        else{
          this.excelEmptyMsg = true;
        }        
      }   
    }
  }

  onClose(){
    this.isTranslationDataUploaded = false;
  }

  onDownloadExcel(row: any){
    let languageMap = new Map();
      this.translationService.getTranslationUploadDetails(row.id).subscribe(fileData => {
        if(fileData){
          let count = 1;
          fileData.forEach(element => {
              if(languageMap.get(element.name)){
                let tempObj = languageMap.get(element.name);
                tempObj[element.code] = element.value;
                languageMap.set(element.name, tempObj);
              }
              else{
                languageMap.set(element.name, this.manipulateObjectForJSONToXLSX(element, count++, []));
              }
          });
          let jsonData = [];
          for(let i of languageMap.values()){
            jsonData.push(i);
          }
          this.convertJSONtoXLSX(jsonData, row.fileName);
        }
      })
    }

  convertJSONtoXLSX(data: any, fileName: string){
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(data);
    const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    this.saveAsExcelFile(excelBuffer, fileName);
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });
    FileSaver.saveAs(data, fileName);
  }

  onCloseMsg(){
    this.grpTitleVisible = false;
  }

  openLanguageSelectionPopup(){
    let tableHeader: any = this.translationData.lblDownloadTemplate || 'Download Template';
    let colsList: any = ['select', 'name'];
    let colsName: any = [this.translationData.lblAll || 'All', this.translationData.lbllanguage || 'Language'];

    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.maxHeight = '90vh';
    dialogConfig.data = {
      colsList: colsList,
      colsName: colsName,
      translationData: this.translationData,
      tableHeader: tableHeader,
    }
    this.dialogRef = this.dialog.open(LanguageSelectionComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe(response => {
      let languageMap = new Map();
      if(response.languagesSelected){
        let enGBSearch = response.languagesSelected.filter(item => item == "EN-GB");
        if(enGBSearch.length == 0)
          response.languagesSelected.push("EN-GB");
        this.translationService.getTranslations().subscribe(translationsData => {
          if(translationsData){
            let count = 1;
            translationsData.forEach(element => {
              let search = response.languagesSelected.filter(item => item == element.code);
              if(search.length > 0){
                if(languageMap.get(element.name)){
                  let tempObj = languageMap.get(element.name);
                  tempObj[element.code] = element.value;
                  languageMap.set(element.name, tempObj);
                }
                else{
                  
                  languageMap.set(element.name, this.manipulateObjectForJSONToXLSX(element, count++, response.languagesSelected));
                }
              }
            });
            let jsonData = [];
            for(let i of languageMap.values()){
              jsonData.push(i);
            }
            this.convertJSONtoXLSX(jsonData, "TranslationUploadTemplate.xlsx");
          }
        })
      }
    });
  }

  manipulateObjectForJSONToXLSX(langObj: any, count: number, languagesSelected: any): any{
    let languages= [];
    languagesSelected.forEach(element => {
      languages.push(element);
    });
    let tempObj = {};
    tempObj["Sr.No."] = count;
    tempObj["Labels"] = langObj.name;
    if(langObj.code == "EN-GB"){
      tempObj[langObj.code] = langObj.value;
      languages.pop();
      languages.forEach(element => {
        tempObj[element]= "";
      });
    }
    else{
      languages.forEach(element => {
        tempObj[element]= "";
      });
      tempObj[langObj.code] = langObj.value;
    }
    return tempObj;
  }

  manipulateObjectForXLSXToJSON(langObj: any): any{
    let tempArray = [];
    
    //TODO : manipulate object to convert in required format
    for(let key in langObj){
      if(key != "Labels" && key != "Sr.No."){
        if(langObj[key] != ""){
          let tempObj = {};
          tempObj["code"]=key;
          tempObj["type"]="L";
          tempObj["name"]=langObj.Labels;
          tempObj["value"]=langObj[key];
          tempArray.push(tempObj);
        }
      }
    }
    return tempArray;
  }

  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }


}

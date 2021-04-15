import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FileValidator } from 'ngx-material-file-input';
import * as FileSaver from 'file-saver';
import { Workbook } from 'exceljs';
import * as XLSX from 'xlsx';
import { packageModel } from '../../models/package.model';
import { PackageService } from '../../services/package.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { CommonTableComponent } from '../.././shared/common-table/common-table.component';



@Component({
  selector: 'app-common-import',
  templateUrl: './common-import.component.html',
  styleUrls: ['./common-import.component.css']
})
export class CommonImportComponent implements OnInit {
  importClicked : boolean = false;
  importPackageFormGroup : FormGroup;
  translationData = [];
  @Output() showImportCSV : EventEmitter<any> = new EventEmitter();
  readonly maxSize = 104857600;
  templateFileUrl: string = 'assets/docs/packageTemplate.csv';
  templateFileName: string = 'packageFile.csv';
  excelEmptyMsg: boolean = false;
  file: any;
  arrayBuffer: any;
  filelist: any = [];
  rejectedPackageList : any = [];
  importedPackagesCount : number = 0;
  rejectedPackagesCount : number = 0;
  showImportStatus : boolean = false;
  packageCodeError : boolean = false;
  packageCodeErrorMsg : string = "";
  driverDialogRef: MatDialogRef<CommonTableComponent>;
  @Input() importTranslationData : any;
  @Input() importFileComponent : string;
  @Input() templateTitle : any;
  @Input() templateValue : any;
  @Input() tableColumnList : any;
  @Input() tableColumnName : any;
  @Input() tableTitle : string;
  constructor(private _formBuilder: FormBuilder, private packageService: PackageService ,private dialog: MatDialog) { }

  ngOnInit(): void {
    this.importPackageFormGroup = this._formBuilder.group({
      uploadFile: [
        undefined,
        [Validators.required, FileValidator.maxContentSize(this.maxSize)]
      ]
    });
  }

  importPackageCSV(){
    this.importClicked = true;
    this.showImportCSV.emit(true);
  }
  closeImport(){
    this.showImportCSV.emit(false);
  }

  downloadTemplate(){
    const header = this.templateTitle;//['PackageCode','PackageName','Description','PackageType','PackageStatus','FeatureId'];
    const data = this.templateValue;
    
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Template');
    //Add Header Row
    let headerRow = worksheet.addRow(header);
    // Cell Style : Fill and Border
    headerRow.eachCell((cell, number) => {
      //console.log(cell)
      if(number != 5){
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
      }else{
        //cell.alignment = { wrapText: true, vertical: 'justify', horizontal: 'justify' }
      }
      cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    });
    // Add Data and Conditional Formatting
    data.forEach(d => {
      let row = worksheet.addRow(d);
    });

    //let csvData = XLSX.utils.sheet_to_csv(data);  
    // const csvFile: Blob = new Blob([csvData], { type: 'text/csv;charset=utf-8;' });  
    // FileSaver.saveAs(csvFile, this.templateFileName);  
    workbook.csv.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'text/csv;charset=utf-8;' });
      FileSaver.saveAs(blob, this.templateFileName);
    });
    
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
        var arraylist = XLSX.utils.sheet_to_json(worksheet,{raw:true});     
        this.filelist = [];
        this.filelist = arraylist;    
    }    
  }
  
  importNewFile(removableInput){
    if(this.filelist.length > 0){
      this.excelEmptyMsg = false;
      if(this.importFileComponent === 'package'){
        this.preparePackageDataToImport(removableInput);
      }
      //removableInput.clear();
    }
    else{
      this.excelEmptyMsg = true;
      removableInput.clear();
    }
  }

  preparePackageDataToImport(removableInput){
    let packagesToImport = [];//new packageModel().importPackage;
    for(let i = 0; i < this.filelist.length ; i++){
      packagesToImport.push(
        {
          "id": 0,
          "code": this.filelist[i]["PackageCode"],
          "featureSetID" : 0,
          "features": this.filelist[i]["FeatureId"],
          "name": this.filelist[i]["PackageName"],
          "type": this.filelist[i]["PackageType"], //=== "VIN" ? "V" : "O"
          "description": this.filelist[i]["Description"],
          "state": this.filelist[i]["PackageStatus"],
          "status": this.filelist[i]["PackageStatus"], //=== "Inactive" ? "I" : "A",
          "createdAt":0
        }
      )
    }
    //console.log(packagesToImport)
    this.validateImportData(packagesToImport,removableInput)
  }

  validateImportData(packagesToImport,removableInput){
    let validData: any = [];
    let invalidData: any = [];
    let codeFlag : boolean;
    let nameFlag : boolean;
    let typeFlag : boolean;
    let statFlag : boolean;
    let stateFlag : boolean;
    let descFlag : boolean;
    let featureFlag : boolean;
    packagesToImport.forEach((item: any) => {
      for (const [key, value] of Object.entries(item)) {
        switch (key) {
          case 'code':{
            let objData: any = this.codeValidation(value); 
            codeFlag = objData.status;
            if(!codeFlag){
              item.returnMessage = objData.reason;
            }
            break;
          }
          case 'features':{
            let objData: any = this.featureValidation(value); 
            featureFlag = objData.status;
            if(!featureFlag){
              item.returnMessage = objData.reason;
            }
            item.features = objData.featureArray;
            break;
          }
            case 'name':{
              let objData: any = this.nameValidation(value,50,'packagename');  
              nameFlag = objData.status;
              if(!nameFlag){
                item.returnMessage = objData.reason;
              }
              break;
            }
            case 'type':{
              let objData: any = this.typeValidation(value,'type');  
              typeFlag = objData.status;
              if(!typeFlag){
                item.returnMessage = objData.reason;
              }
              else{
                item.type = value === "VIN" ? "V" : "O";
              }
              break;
            }
            case 'description':{
              let objData: any = this.descValidation(value);  
              descFlag = objData.status;
              if(!descFlag){
                item.returnMessage = objData.reason;
              }
              break;
            }
            case 'state':{
              let objData: any = this.typeValidation(value,'status');  
              stateFlag = objData.status;
              if(!stateFlag){
                item.returnMessage = objData.reason;
              }
              else{
                item.state = value === "Inactive" ? "I" : "A";
              }
              break;
            }
            case 'status':{
              let objData: any = this.typeValidation(value,'status');  
              statFlag = objData.status;
              if(!statFlag){
                item.returnMessage = objData.reason;
              }
              else{
                item.status = value === "Inactive" ? "I" : "A";
              }
              break;
            }
          default:
            break;
        }
      }
      
    if(statFlag && codeFlag && descFlag && nameFlag && typeFlag && featureFlag && stateFlag){
      validData.push(item);
    }
    else{
      invalidData.push(item);
    }
    });
    this.callImportAPI(validData,invalidData,removableInput)
  
    //console.log(validData , invalidData)
    return { validDriverList: validData, invalidDriverList: invalidData };
   
  }

  callImportAPI(validData,invalidData,removableInput){
    this.rejectedPackageList = invalidData;
    this.rejectedPackagesCount = invalidData.length;
    this.importedPackagesCount = validData.length;
    this.packageCodeError = false;
    if(validData.length > 0){
        this.packageService.importPackage(validData).subscribe((resultData)=>{
          this.showImportStatus = true;
          removableInput.clear();
          if(resultData){
            this.importedPackagesCount = resultData.packageList.length;
          }
        },
        (err)=>{
          this.showImportStatus = true;

          if(err.status === 500){
            this.rejectedPackageList = this.rejectedPackageList + this.importedPackagesCount;
            this.importedPackagesCount = 0
            this.packageCodeError = true;
            this.packageCodeErrorMsg = this.importTranslationData.existError;
          }
        })
    }
  }

  onClose(){
    this.showImportStatus = false;
  }

  codeValidation(value: any){
    let obj: any = { status: true, reason: 'correct data'};
    const regx = /[A-Z]{1,1}[A-Z\s]{1,1}[\s]{1,1}[A-Z0-9]{13,13}[0-9]{3,3}/;
    if(!value || value == '' || value.trim().length == 0){
      obj.status = false;
      obj.reason = this.importTranslationData.input1mandatoryReason;
      return obj;  
    }
    return obj;
  }

 
  nameValidation(value: any, maxLength: any, type: any){
    let obj: any = { status: true, reason: 'correct data'};
    let numberRegex = /[^0-9]+$/;
    let SpecialCharRegex = /[^!@#\$%&*]+$/;
   
    if(!value || value == '' || value.trim().length == 0){ 
      obj.status = false;
      obj.reason = this.importTranslationData.input2mandatoryReason;
      return obj; 
    }
    else{

      if(value.length > maxLength){
        obj.status = false;
        obj.reason = this.getValidateMsg(type, this.importTranslationData.maxAllowedLengthReason, maxLength) 
        return obj;
      }
      if(!SpecialCharRegex.test(value)){
        obj.status = false;
        obj.reason = this.getValidateMsg(type,  this.importTranslationData.specialCharNotAllowedReason);
        return obj;
      }
      return obj;
    }
  }

  descValidation(value:any){
    let obj: any = { status: true, reason: 'correct data'};
    if(value.length > 100){
      obj.status = false;
      obj.reason = this.importTranslationData.packageDescriptionCannotExceedReason;

    }
    return obj;
  }

  typeValidation(value: any, type:any){
    let obj: any = { status: true, reason: 'correct data'};
    if(!value || value == '' || value.trim().length == 0){ 
      obj.status = false;
      if(type === 'type')
      obj.reason = this.importTranslationData.packageTypeMandateReason;
      if(type === 'status')
      obj.reason = this.importTranslationData.packageStatusMandateReason;
      return obj; 
    }
    else{
      switch (type) {
        case 'type':
          if(value.toLowerCase() != "vin"){
            if(value.toLowerCase() != "organization" ){
              obj.status = false;
              obj.reason = this.importTranslationData.packageTypeReason;

            }
          }
          break;
          case 'status':
          if(value.toLowerCase() != "active" ){
            if(value.toLowerCase() != "inactive"){
            obj.status = false;
            obj.reason = this.importTranslationData.packageStatusReason;
            }
          }
          break;
        default:
          break;
      }
      return obj; 
    }

  }

  featureValidation(value: any){
    let obj: any = { status: true, reason: 'correct data',featureArray : []};
    let featureArray = [];
    if(!value || value == '' || value.trim().length == 0){ 
      obj.status = false;
      obj.reason = this.importTranslationData.featureemptyReason;
      obj.featureArray = [];
    }
    else{
      featureArray = value.split(",");
      for(var i in featureArray){
        if(featureArray[i] === null || featureArray[i] === undefined || featureArray[i].trim() === ''){ 
          obj.status = false;
          obj.reason =  this.importTranslationData.featureinvalidReason;
        }
        else{
          featureArray[i] = featureArray[i].trim();
        }
      }
      obj.featureArray = featureArray;

    }
    return obj;
  }

  getValidateMsg(type: any, typeTrans: any, maxLength?: any){
    if(typeTrans){
      if(maxLength){
        typeTrans = typeTrans.replace('$', type); 
        return typeTrans.replace('#', maxLength)
      }
      else{
        return typeTrans.replace('$', type);
      }
    }
  }

  showRejectedPopup(rejectedPackageList){
    let populateRejectedList=[];
    if(this.importFileComponent === 'package'){
      for(var i in rejectedPackageList){
        populateRejectedList.push(
          {
            "packageCode":this.rejectedPackageList[i]["code"],
            "packageName": this.rejectedPackageList[i]["name"],
            "packageDescription" :this.rejectedPackageList[i]["description"],
            "packageType" : this.rejectedPackageList[i]["type"],
            "packageStatus" :this.rejectedPackageList[i]["status"],
            "packageFeature" :this.rejectedPackageList[i]["features"],
            "returnMessage" :this.rejectedPackageList[i]["returnMessage"]
          }
        )
      }
    }
    this.displayPopup(populateRejectedList);
    
     
  }

  displayPopup(populateRejectedList){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: populateRejectedList,
      colsList: this.tableColumnList,
      colsName: this.tableColumnName,
      tableTitle: this.tableTitle
    }
    this.driverDialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }
}

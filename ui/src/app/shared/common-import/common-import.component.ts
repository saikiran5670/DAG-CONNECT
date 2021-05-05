import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FileValidator } from 'ngx-material-file-input';
import * as FileSaver from 'file-saver';
import { Workbook } from 'exceljs';
import * as XLSX from 'xlsx';
import { packageModel } from '../../models/package.model';
import { PackageService } from '../../services/package.service';
import { POIService } from '../../services/poi.service';

import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { CommonTableComponent } from '../.././shared/common-table/common-table.component';
import { NgxXml2jsonService } from 'ngx-xml2json';


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
  rejectedList : any = [];
  importedCount : number = 0;
  rejectedCount : number = 0;
  showImportStatus : boolean = false;
  packageCodeError : boolean = false;
  packageCodeErrorMsg : string = "";
  rejectedDialogRef: MatDialogRef<CommonTableComponent>;
  @Input() importTranslationData : any;
  @Input() importFileComponent : string;
  @Input() templateTitle : any;
  @Input() templateValue : any;
  @Input() tableColumnList : any;
  @Input() tableColumnName : any;
  @Input() tableTitle : string;
  @Input() defaultGpx:any;
  fileExtension = '.csv';
  parsedGPXData : any;

  constructor(private _formBuilder: FormBuilder, private packageService: PackageService ,private dialog: MatDialog, 
    private poiService: POIService,private ngxXml2jsonService : NgxXml2jsonService) { }

  ngOnInit(): void {
    if(this.importFileComponent === 'poi'){
      this.fileExtension = '.xlsx';
    }
    else if(this.importFileComponent === 'geofence'){
      this.fileExtension = '.gpx';
    }
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
    const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';

    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Template');
    //Add Header Row
    let headerRow = worksheet.addRow(header);
    // Cell Style : Fill and Border
    // headerRow.eachCell((cell, number) => {
    //   //console.log(cell)
    //   if(number != 5){
    //     cell.fill = {
    //       type: 'pattern',
    //       pattern: 'solid',
    //       fgColor: { argb: 'FF0A3175' },
    //       bgColor: { argb: 'FF0000FF' }
    //     }
    //     cell.font = {
    //       color: { argb: 'FFFFFFFF'},
    //       bold: true
    //     }
    //   }else{
    //     //cell.alignment = { wrapText: true, vertical: 'justify', horizontal: 'justify' }
    //   }
    //   cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
    // });
    // Add Data and Conditional Formatting
    data.forEach(d => {
      let row = worksheet.addRow(d);
    });

    //let csvData = XLSX.utils.sheet_to_csv(data);  
    // const csvFile: Blob = new Blob([csvData], { type: 'text/csv;charset=utf-8;' });  
    // FileSaver.saveAs(csvFile, this.templateFileName);  
   
    if(this.importFileComponent === 'poi'){
      this.templateFileName = 'poiData.xlsx';
      workbook.xlsx.writeBuffer().then((data) => {
        let blob = new Blob([data], { type: EXCEL_TYPE });
        FileSaver.saveAs(blob, this.templateFileName);
      });
    }
    else if(this.importFileComponent === 'geofence'){
      this.templateFileName = 'geofenceData.gpx';
      let blob = new Blob([this.defaultGpx], { type: 'xml;charset=utf-8;' });
      FileSaver.saveAs(blob, this.templateFileName);
    }
    else{
      workbook.csv.writeBuffer().then((data) => {
        let blob = new Blob([data], { type: 'text/csv;charset=utf-8;' });
        FileSaver.saveAs(blob, this.templateFileName);
      });
    }

    
  }


  addfile(event: any){ 
    if (this.fileExtension === 'csv' || this.fileExtension === 'xlsx') {
      this.excelEmptyMsg = false;
      this.file = event.target.files[0];
      let fileReader = new FileReader();
      fileReader.readAsArrayBuffer(this.file);
      fileReader.onload = (e) => {
        this.arrayBuffer = fileReader.result;
        var data = new Uint8Array(this.arrayBuffer);
        var arr = new Array();
        for (var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);
        var bstr = arr.join("");
        var workbook = XLSX.read(bstr, { type: "binary" });
        var first_sheet_name = workbook.SheetNames[0];
        var worksheet = workbook.Sheets[first_sheet_name];
        var arraylist = XLSX.utils.sheet_to_json(worksheet, { raw: true });
        this.filelist = [];
        this.filelist = arraylist;
      }
    }
    else{
      this.file = event.target.files[0];

      let fileReader = new FileReader();
      fileReader.readAsText(this.file);
      let text : any;
      fileReader.onload = (e) => {
        text = fileReader.result;
        const parser = new DOMParser();
        const xml = parser.parseFromString(text, 'text/xml');
        this.parsedGPXData = this.ngxXml2jsonService.xmlToJson(xml);
        this.formatGPXData();

      }
    }
  }
  
  importNewFile(removableInput){
    if(this.filelist.length > 0){
      this.excelEmptyMsg = false;
      if(this.importFileComponent === 'package'){
        this.preparePackageDataToImport(removableInput);
      }
      else if(this.importFileComponent === 'poi'){
        this.preparePOIDataToImport(removableInput);

      }
      else if(this.importFileComponent === 'geofence'){
        this.prepareGeofenceDataToImport(removableInput);
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
            let objData: any = this.codeValidation(value,'code'); 
            codeFlag = objData.status;
            if(!codeFlag){
              item.returnMessage = objData.reason;
            }
            if(validData.length > 0){
              for(var i in validData){
                if(validData[i]["code"] === value){
                  codeFlag = false;
                  item.returnMessage = "Duplicate Package Code"
                }
              }
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
    this.rejectedList = invalidData;
    this.rejectedCount = invalidData.length;
    this.importedCount = 0;
    this.packageCodeError = false;
    if(validData.length > 0){
        this.packageService.importPackage(validData).subscribe((resultData)=>{
          this.showImportStatus = true;
          removableInput.clear();
          if(resultData){
            this.importedCount = resultData.packageList.length;
          }
        },
        (err)=>{
          removableInput.clear();
          this.showImportStatus = true;

          if(err.status === 409){
            this.rejectedList = this.rejectedList + this.importedCount;
            this.importedCount = 0
            this.packageCodeError = true;
            this.packageCodeErrorMsg = this.importTranslationData.existError;
          }
        })
    }
    else{
      removableInput.clear();
      this.showImportStatus = true;
    }
  }


  // POI import functions
  preparePOIDataToImport(removableInput){
    let packagesToImport = [];//new packageModel().importPackage;
    for(let i = 0; i < this.filelist.length ; i++){
      packagesToImport.push(
        {
          
            "organizationId": this.filelist[i]["OrganizationId"],
            "categoryId": this.filelist[i]["CategoryId"],
            "categoryName":this.filelist[i]["CategoryName"],
            "subCategoryId":this.filelist[i]["SubCategoryId"],
            "subCategoryName": this.filelist[i]["SubCategoryName"],
            "name": this.filelist[i]["POIName"],
            "address": this.filelist[i]["Address"],
            "city": this.filelist[i]["City"],
            "country": this.filelist[i]["Country"],
            "zipcode": this.filelist[i]["Zipcode"],
            "latitude": this.filelist[i]["Latitude"],
            "longitude": this.filelist[i]["Longitude"],
            "distance": this.filelist[i]["Distance"],
            "state": this.filelist[i]["State"],
            "type": this.filelist[i]["Type"]
        
        }
      )
    }
 
    this.validatePOIData(packagesToImport,removableInput);
  }

  validatePOIData(packagesToImport,removableInput){
    let validData: any = [];
    let invalidData: any = [];
    let orgFlag = false, categoryFlag = false, subcategoryFlag = false,nameFlag= false,longitudeFlag=false,latitudeFlag=false;
    packagesToImport.forEach((item: any) => {
      for (const [key, value] of Object.entries(item)) {
        switch (key) {
          case 'organizationId':{
            let objData: any = this.basicValidation(value,'code'); 
            orgFlag = objData.status;
            if(!orgFlag){
              item.returnMessage = objData.reason;
            }
            break;
          }
          case 'categoryId':{
            let objData: any = this.basicValidation(value,'categoryId'); 
            categoryFlag = objData.status;
            if(!categoryFlag){
              item.returnMessage = objData.reason;
            }
            break;
          }
          case 'subCategoryId':{
            let objData: any = this.basicValidation(value,'subCategoryId'); 
            subcategoryFlag = objData.status;
            if(!subcategoryFlag){
              item.returnMessage = objData.reason;
            }
            break;
          }
          case 'name':{
            let objData: any = this.basicValidation(value,'name'); 
            nameFlag = objData.status;
            if(!nameFlag){
              item.returnMessage = objData.reason;
            }
            // if(validData.length > 0){
            //   for(var i in validData){
            //     if(validData[i]["name"] === value){
            //       nameFlag = false;
            //       item.returnMessage = "Duplicate POI"
            //     }
            //   }
            // }
            break;
          }
          case 'latitude':{
            let objData: any = this.basicValidation(value,'latitude'); 
            latitudeFlag = objData.status;
            if(!latitudeFlag){
              item.returnMessage = objData.reason;
            }
            break;
          }
          case 'longitude':{
            let objData: any = this.basicValidation(value,'longitude'); 
            longitudeFlag = objData.status;
            if(!longitudeFlag){
              item.returnMessage = objData.reason;
            }
            break;
          }
          default:
            break;
        }
      }
      
         
    if(orgFlag && categoryFlag && subcategoryFlag && nameFlag && longitudeFlag && latitudeFlag){
      validData.push(item);
    }
    else{
      invalidData.push(item);
    }
    });
   
    
    this.callPOIImportAPI(validData,invalidData,removableInput)
  
    //console.log(validData , invalidData)
    //return { validDriverList: validData, invalidDriverList: invalidData };
  }

  
  callPOIImportAPI(validData,invalidData,removableInput){
    this.rejectedList = invalidData;
    this.rejectedCount = invalidData.length;
    this.importedCount = 0;
    this.packageCodeError = false;
    if(validData.length > 0){
        this.poiService.importPOIExcel(validData).subscribe((resultData)=>{
          this.showImportStatus = true;
          removableInput.clear();
          if(resultData["poiUploadedList"].length >0){
            this.importedCount = resultData["poiUploadedList"].length;
          }
          if(resultData["poiDuplicateList"].length >0){
            this.rejectedList.push(...resultData["poiDuplicateList"]);
            this.updateDuplicateErrorMsg()
            this.rejectedCount =  this.rejectedList.length;  
          }
        },
        (err)=>{
          removableInput.clear();
          this.showImportStatus = true;

          if(err.status === 409){
           
          }
        })
    }
    else{
      removableInput.clear();
      this.showImportStatus = true;
    }
  }

  //import Geofence function

  formatGPXData(){
    let gpxData = this.parsedGPXData;
    let gpxInfo = gpxData["gpx"]["metadata"];
    let nodeInfo = gpxData["gpx"]["trk"];
    console.log(gpxInfo);
    console.log(nodeInfo)
    let organizedGPXData = [];
    let nodeArray = [],nodeObj ={};
      
    for(var i in nodeInfo){
      nodeArray.push(nodeInfo[i]["trkseg"]["trkpt"]);
    }
    let nodeArraySet = [];

    for(let i = 0; i < nodeArray.length ; i++){
      let nodeArrayForEach = [];
      for(let j = 0; j < nodeArray[i].length ; j++){
        nodeArrayForEach.push({
            "id": 0,
            "landmarkId": 0,
            "seqNo": j+1,
            "latitude": nodeArray[i][j]["@attributes"]["lat"],
            "longitude": nodeArray[i][j]["@attributes"]["lon"],
            "createdBy": 0

          })
      }
      nodeArraySet.push(nodeArrayForEach)
    }

    console.log("nodeArraySet");
    console.log(nodeArraySet)
    for(let i = 0; i < gpxInfo.length ; i++){
      
      organizedGPXData.push(
        {
          "id": gpxInfo[i].id,
          "organizationId": gpxInfo[i].organizationId,
          "categoryId": gpxInfo[i].categoryId,
          "subCategoryId": gpxInfo[i].subCategoryId,
          "name": gpxInfo[i].geofencename,
          "type": gpxInfo[i].type,
          "address": gpxInfo[i].address,
          "city": gpxInfo[i].city,
          "country": gpxInfo[i].country,
          "zipcode": gpxInfo[i].zipcode,
          "latitude": gpxInfo[i].latitude,
          "longitude": gpxInfo[i].longitude,
          "distance": gpxInfo[i].distance,
          "tripId": gpxInfo[i].tripId,
          "createdBy":gpxInfo[i].createdBy,
          "nodes": nodeArraySet[i]
        })
      }
      
      // this.poiService.importGeofenceGpx(organizedGPXData).subscribe((resultData)=>{

      // })
      console.log(organizedGPXData)
      console.log(nodeArray)
  }

  prepareGeofenceDataToImport(removableInput){
    console.log(removableInput)
    const parser = new DOMParser();
    const xml = parser.parseFromString(removableInput, 'text/xml');
    const obj = this.ngxXml2jsonService.xmlToJson(xml);
    console.log(obj)

  }

  updateDuplicateErrorMsg(){

  }
  onClose(){
    this.showImportStatus = false;
  }

  // Package Validation
  codeValidation(value: any,type:any){
    let obj: any = { status: true, reason: 'correct data'};
    let SpecialCharRegex = /[^!@#\$%&*]+$/;
    if(!value || value == '' || value.trim().length == 0){
      obj.status = false;
      obj.reason = this.importTranslationData.input1mandatoryReason;
      return obj;
    }
    if(!SpecialCharRegex.test(value)){
      obj.status = false;
      obj.reason = this.importTranslationData.specialCharNotAllowedReason;
      return obj;
    }
    if(value.length > 20){
      obj.status = false;
      obj.reason = this.getValidateMsg(type, this.importTranslationData.maxAllowedLengthReason, 20) 
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
    }
    return obj;
  }

  descValidation(value:any){
    let obj: any = { status: true, reason: 'correct data'};
    let SpecialCharRegex = /[^!@#\$%&*]+$/;
    if(value && value != ""){
      if (value.length > 100) {
        obj.status = false;
        obj.reason = this.importTranslationData.packageDescriptionCannotExceedReason;
        return obj;
      }
      if (!SpecialCharRegex.test(value)) {
        obj.status = false;
        obj.reason = this.importTranslationData.specialCharNotAllowedReason;
        return obj;
      }
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

  //POI validation
  basicValidation(value: any,type:any){
    let obj: any = { status: true, reason: 'correct data'};
    let SpecialCharRegex = /[^!@#\$%&*]+$/;
    if(!value || value == ''){
      obj.status = false;
      obj.reason = this.importTranslationData.input1mandatoryReason;
      return obj;
    }
    if(!SpecialCharRegex.test(value)){
      obj.status = false;
      obj.reason = this.importTranslationData.specialCharNotAllowedReason;
      return obj;
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

  showRejectedPopup(rejectedList){
    let populateRejectedList=[];
    if(this.importFileComponent === 'package'){
      for(var i in rejectedList){
        populateRejectedList.push(
          {
            "packageCode":this.rejectedList[i]["code"],
            "packageName": this.rejectedList[i]["name"],
            "packageDescription" :this.rejectedList[i]["description"],
            "packageType" : this.rejectedList[i]["type"],
            "packageStatus" :this.rejectedList[i]["status"],
            "packageFeature" :this.rejectedList[i]["features"],
            "returnMessage" :this.rejectedList[i]["returnMessage"]
          }
        )
      }
    }
    else if(this.importFileComponent === 'poi'){
      for(var i in rejectedList){
        populateRejectedList.push(
          {
            "OrganizationId":this.rejectedList[i]["organizationId"],
            "CategoryId": this.rejectedList[i]["categoryId"],
            "CategoryName" :this.rejectedList[i]["categoryName"],
            "SubCategoryId" : this.rejectedList[i]["subCategoryId"],
            "SubCategoryName" :this.rejectedList[i]["subCategoryName"],
            "POIName" :this.rejectedList[i]["name"],
            "Address":this.rejectedList[i]["address"],
            "City": this.rejectedList[i]["city"],
            "Country" :this.rejectedList[i]["country"],
            "Zipcode" : this.rejectedList[i]["zipcode"],
            "Latitude" :this.rejectedList[i]["latitude"],
            "Longitude" :this.rejectedList[i]["longitude"],
            "Distance" :this.rejectedList[i]["distance"],
            "State" :this.rejectedList[i]["state"],
            "Type" :this.rejectedList[i]["type"],
            "returnMessage" :this.rejectedList[i]["returnMessage"]
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
    dialogConfig.data = {};
   
    dialogConfig.data = {
      tableData: populateRejectedList,
      colsList: this.tableColumnList,
      colsName: this.tableColumnName,
      tableTitle: this.tableTitle
    }
    this.rejectedDialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }
}

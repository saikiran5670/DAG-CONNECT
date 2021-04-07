import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FileValidator } from 'ngx-material-file-input';
import * as FileSaver from 'file-saver';
import { Workbook } from 'exceljs';
import * as XLSX from 'xlsx';
import { packageModel } from '../../models/package.model'

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

  constructor(private _formBuilder: FormBuilder) { }

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

  downloadPackageTemplate(){

    const header = ['PackageCode','PackageName','Description','PackageType','PackageStatus','FeatureId'];
    const data = [
      ['P100', 'Package1', "Package Template", "VIN", "Active","10075, 1"]
    ];
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Package Template');
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
  
  importPackage(removableInput){
    if(this.filelist.length > 0){
      this.excelEmptyMsg = false;
      let featureIdArray : [] = [];
      let packagesToImport = [];//new packageModel().importPackage;
      for(let i = 0; i < this.filelist.length ; i++){
        packagesToImport.push(
          {
            "id": 0,
            "code": this.filelist[i]["PackageCode"],
            "featureSetID" : 24,
            "features": this.filelist[i]["FeatureId"].split(","),
            "name": this.filelist[i]["PackageName"],
            "type": this.filelist[i]["PackageType"] === "VIN" ? "V" : "O",
            "description": this.filelist[i]["Description"],
            "isActive": true,
            "status": this.filelist[i]["Status"]=== "Inactive" ? "I" : "A",
            "createdAt":0
          }
        )
      }
    console.log(packagesToImport)
    }
    else{
      this.excelEmptyMsg = true;
    }
  }

}

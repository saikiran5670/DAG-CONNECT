import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

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
  constructor() { }

  ngOnInit(): void {
  }

  importPackageCSV(){
    this.importClicked = true;
    this.showImportCSV.emit(true);
  }

  downloadTemplate(){
    
  }
}

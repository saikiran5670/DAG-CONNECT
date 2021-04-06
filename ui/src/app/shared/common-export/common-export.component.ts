import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-common-export',
  templateUrl: './common-export.component.html',
  styleUrls: ['./common-export.component.less']
})
export class CommonExportComponent implements OnInit {

  @Input() initData : any;
  @Input() translationData: any;
  
  localData : any;
  @Output() exportValues : EventEmitter<any> = new EventEmitter();
 

  constructor() { }

  ngOnInit(): void {
    this.localData = this.initData;
    console.log("export has been called");
  }

}

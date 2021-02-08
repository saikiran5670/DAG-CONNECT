import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-edit-driver-details',
  templateUrl: './edit-driver-details.component.html',
  styleUrls: ['./edit-driver-details.component.less']
})
export class EditDriverDetailsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<boolean>();
  @Input() rowData: any;
  @Input() translationData: any;
  firstFormGroup: FormGroup;
  selectList: any = [
    {
      name: 'Mr'
    },
    {
      name: 'Mrs'
    },
    {
      name: 'Ms'
    }
  ];
  data: any = {
    optValue: 'opt-in'
  };
  optVal: string = '';

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() {
    this.firstFormGroup = this._formBuilder.group({
      driverId: ['', [Validators.required]],
      emailId: ['', []],
      consentStatus: ['', [Validators.required]],
      salutation: ['', [Validators.required]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      birthDate: ['', []],
      language: ['', []],
      unit: ['', []],
      timeZone: ['', []],
      currency: ['', []]
    });
    //this.optVal = this.rowData.isActive ? this.data.optValue : 'opt-out';
    this.optVal = this.data.optValue;
    //console.log("rowData:: ", this.rowData)
  }

  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    let now = new Date();
    now.setDate(now.getDate() - 1);
    return date > now;
  }

  onCancel(){
    this.backToPage.emit(false);
  }
  
  onReset(){}
  
  onConfirm(){
    //console.log(this.firstFormGroup.controls)
    this.backToPage.emit(false);
  }

  onChange(event){
    //console.log(event.value)
  }

  numericOnly(event): boolean {    
    let patt = /^([0-9])$/;
    let result = patt.test(event.key);
    return result;
  }
  
}

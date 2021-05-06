import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-edit-view-alerts',
  templateUrl: './create-edit-view-alerts.component.html',
  styleUrls: ['./create-edit-view-alerts.component.less']
})
export class CreateEditViewAlertsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() actionType: any;
  @Input() translationData: any = [];
  @Input() selectedRowData: any;
  groupCreatedMsg: any = '';
  breadcumMsg: any = '';
  alertForm: FormGroup;
  accountOrganizationId: number;
  accountId: number;
  userType: string;
  selectedApplyOn: string;
  openAdvancedFilter: boolean= false;
  
  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.alertForm = this._formBuilder.group({
      alertName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      alertCategory: ['', [Validators.required]],
      alertType: ['', [Validators.required]],
      applyOn: ['vehicle_group', [Validators.required]],
      vehicleGroup: ['', [Validators.required]],
      vehicle: [''],
      statusMode: ['active', [Validators.required]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('alertName')  
      ]
    });
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onApplyOnChange(event){
    this.selectedApplyOn = event.value;
  }

  onClickAdvancedFilter(){
    this.openAdvancedFilter = !this.openAdvancedFilter;
  }

}

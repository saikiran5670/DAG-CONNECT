import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-notifications-alert',
  templateUrl: './create-notifications-alert.component.html',
  styleUrls: ['./create-notifications-alert.component.css']
})
export class CreateNotificationsAlertComponent implements OnInit {
  @Input() translationData: any = [];
  notificationForm: FormGroup;
  @Input() alert_category_selected: any;
  @Input() alertTypeName: string;
  @Input() isCriticalLevelSelected :any;
  @Input() labelForThreshold :any;
  @Input() alert_type_selected: string;
  @Input() actionType: any;
  addFlag: boolean = false;
  contactModeType: any;
  radioButtonVal: any;
  openAdvancedFilter: boolean= false;
 contactModes : any = [
  {
    id : 0,
    value: 'Web Service'
  },
  {
    id : 1,
    value: 'Email'
  }
];

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.notificationForm = this._formBuilder.group({
      recipientLabel: ['', [ Validators.required ]],
      contactMode: ['', [Validators.required]],
      emailAddress: ['', [Validators.required, Validators.email]],
      mailSubject: ['', [Validators.required]],
      mailDescription: ['', [Validators.required]],
      wsDescription: ['', [Validators.required]],
      authentication:['', [Validators.required]],
      loginId: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      webURL:['', [Validators.required]],
      wsTextDescription:[''],
      criticalLevel: [''],
      warningLevel: [''],
      advisoryLevel: ['']
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('recipientLabel'),
      ]
    });
  }

  onNotificationAdd(){
    this.addFlag = true;
  }

  onChangeContactMode(event :any){
   this.contactModeType = event.value;
  }

  onRadioButtonChange(event: any){
    this.radioButtonVal = event.value;

  }

  onClickAdvancedFilter(){
    this.openAdvancedFilter = !this.openAdvancedFilter;
  }

  onChangeCriticalLevel(event: any){

  }

  onChangeWarningLevel(event: any){

  }

  onChangeAdvisoryLevel(event: any){

  }

}

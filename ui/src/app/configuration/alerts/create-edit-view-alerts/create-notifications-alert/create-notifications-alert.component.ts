import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { EmailValidator, FormArray, FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-notifications-alert',
  templateUrl: './create-notifications-alert.component.html',
  styleUrls: ['./create-notifications-alert.component.less']
})
export class CreateNotificationsAlertComponent implements OnInit {
  @Input() translationData: any = [];
  notificationForm: FormGroup;
  FormArrayItems : FormArray;
  ArrayList : any =[];
  @Input() alert_category_selected: any;
  @Input() alertTypeName: string;
  @Input() isCriticalLevelSelected :any;
  @Input() isWarningLevelSelected :any;
  @Input() isAdvisoryLevelSelected :any;
  @Input() labelForThreshold :any;
  @Input() alert_type_selected: string;
  @Input() actionType: any;
  localStLanguage: any;
  organizationId: number;
  addFlag: boolean = false;
  contactModeType: any;
  radioButtonVal: any;
  notificationPayload: any;
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
recipientLabel: any;
contactMode: any;
webURL: any;
wsDescription: any;
authentication: any;
loginId: any;
password: any;
emailAddress: any;
mailSubject: any;
mailDescription: any;

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.notificationForm = this._formBuilder.group({
      // recipientLabel: ['', [ Validators.required ]],
      // contactMode: ['', [Validators.required]],
      // emailAddress: ['', [Validators.required, Validators.email]],
      // mailSubject: ['', [Validators.required]],
      // mailDescription: ['', [Validators.required]],
      // wsDescription: ['', [Validators.required]],
      // authentication:['', [Validators.required]],
      // loginId: ['', [Validators.required, Validators.email]],
      // password: ['', [Validators.required]],
      // webURL:['', [Validators.required]],
      // wsTextDescription:[''],
      // criticalLevel: [''],
      // warningLevel: [''],
      // advisoryLevel: ['']
      FormArrayItems : this._formBuilder.array([this.initItems()]),
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('recipientLabel', 'FormArrayItems'),
      ]
    });
    
    }

    initItems(): FormGroup{
      return this._formBuilder.group({
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
        });
    }

//     get formArr(): FormArray{
// return this.notificationForm.get("FormArrayItems") as FormArray;
//     }

    addMultipleItems() :void{
      // const creds = this.notificationForm.controls.credentials as FormArray;
      // this.formArr.push(this.initItems());
      this.ArrayList = this.notificationForm.get("FormArrayItems") as FormArray;
      this.ArrayList.push(this.initItems());
    }

  setDefaultValueForws(){
    this.webURL = "";
    this.wsDescription = "";
    this.authentication = "";
    this.loginId = "";
    this.password = "";
  }

  setDefaultValueForemail(){
    this.emailAddress = "";
    this.mailSubject = ""
    this.mailDescription = "";
  }
  
  onNotificationAdd(){
    this.addFlag = true;
  }

  onChangeContactMode(event :any){
   this.contactModeType = event.value;
   this.addFlag = false;
   //for Web service
  //  if(this.contactModeType == 0){
  //   this.recipientLabel = this.notificationForm.controls.recipientLabel.value;
  //   this.contactMode = this.notificationForm.controls.contactMode.value
  //   this.webURL = this.notificationForm.controls.webURL.value;
  //   this.wsDescription = this.notificationForm.controls.wsDescription.value;
  //   this.authentication = this.notificationForm.controls.authentication.value;
  //   this.loginId = this.notificationForm.controls.loginId.value;
  //   this.password = this.notificationForm.controls.password.value;
  //   this.setDefaultValueForemail()
  //  }
  //  // for Email
  //  if(this.contactModeType == 1){
  //  this.recipientLabel = this.notificationForm.controls.recipientLabel.value;
  //  this.contactMode = this.notificationForm.controls.contactMode.value
  //   this.emailAddress = this.notificationForm.controls.emailAddress.value;
  //   this.mailSubject = this.notificationForm.controls.mailSubject.value;
  //   this.mailDescription = this.notificationForm.controls.mailDescription.value;
  //   this.setDefaultValueForws();
  //  }
  //  this.notificationPayload = {
  //    recipientLabel: this.recipientLabel,
  //        accountGroupId: this.organizationId,
  //        notificationModeType: this.contactMode,
  //        phoneNo: "",
  //        sms: "",
  //        emailId: this.emailAddress,
  //        emailSub: this.mailSubject,
  //        emailText: this.mailDescription,
  //        wsUrl: this.webURL,
  //        wsType: this.authentication,
  //        wsText: this.wsDescription,
  //        wsLogin: this.loginId,
  //        wsPassword: this.password
  //  }
  //  console.log(this.notificationPayload);
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

  getNotificationDetails() : any{
    let tempObj= {
      name: "ABCD"
    }
    return tempObj;
  }

}

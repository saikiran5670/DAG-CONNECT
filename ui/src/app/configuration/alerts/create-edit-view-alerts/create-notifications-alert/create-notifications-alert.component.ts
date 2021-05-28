import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { EmailValidator, FormArray, FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { element } from 'protractor';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-notifications-alert',
  templateUrl: './create-notifications-alert.component.html',
  styleUrls: ['./create-notifications-alert.component.less']
})
export class CreateNotificationsAlertComponent implements OnInit {
  @Input() translationData: any = [];
  @Input() selectedRowData: any;
  notificationForm: FormGroup;
  FormArrayItems : FormArray;
  FormEmailArray : FormArray;
  FormWebArray : FormArray;
  ArrayList : any =[];
  emailIndex : any = 0;
  wsIndex : any = 0;
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
  addEmailFlag: boolean = false;
  addWsFlag : boolean = false;
  contactModeType: any;
  radioButtonVal: any = 'N';
  notificationReceipients: any = [];
  notifications : any = [];
  openAdvancedFilter: boolean= false;
 contactModes : any = [
  {
    id : 'W',
    value: 'Web Service'
  },
  {
    id : "E",
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
notifyPeriod: any;
emailCount : number = 0;
wsCount : number = 0;
emailLabel : any;
wsLabel: any;
  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    console.log("action type=" +this.actionType);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.notificationForm = this._formBuilder.group({
      recipientLabel: ['', [ Validators.required ]],
      contactMode: ['', [Validators.required]],
      criticalLevel: [''],
      warningLevel: [''],
      advisoryLevel: [''],
      // FormArrayItems : this._formBuilder.array([this.initItems()]),
      FormEmailArray : this._formBuilder.array([this.initEmailItems()]),
      FormWebArray : this._formBuilder.array([this.initWebItems()]),
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('recipientLabel'),
      ]
    });
    console.log(this.selectedRowData);
    if(this.actionType == 'edit' || this.actionType == 'duplicate')
    {
      this.setDefaultValues();
    }
  }


    initEmailItems(): FormGroup{
      return this._formBuilder.group({
        emailAddress: ['', [Validators.required, Validators.email]],
        mailSubject: ['This is default subject for ' +this.alertTypeName, [Validators.required]],
        mailDescription: ['This is default text for ' +this.alertTypeName, [Validators.required]],
        notifyPeriod: ['A'],
        emailRecipientLabel: [''],
        emailContactModes: ['']
      });

    }

    initWebItems(): FormGroup{
      return this._formBuilder.group({
        wsDescription: ['This is default text for ' +this.alertTypeName, [Validators.required]],
        authentication:['N', [Validators.required]],
        loginId: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]],
        webURL:['', [Validators.required]],
        wsTextDescription:['This is default text for ' +this.alertTypeName],
        notifyPeriodweb: ['A'],        
        webRecipientLabel: [''],
        webContactModes: [''],
      });

    }


    setDefaultValues(){
      
this.selectedRowData.notifications[0].notificationRecipients.forEach(element => {
  this.addMultipleItems(false,element);
});
    }

    addMultipleItems(isButtonClicked: boolean, data? :any) :void{
if(isButtonClicked){
      this.contactModeType = this.notificationForm.get("contactMode").value;
      //this is for email
      if (this.contactModeType == 'E') {
        this.addEmailFlag = true;
        this.emailCount = this.emailCount + 1;
        if (!this.FormEmailArray) {
          this.FormEmailArray = this.notificationForm.get("FormEmailArray") as FormArray;
          this.emailLabel = this.notificationForm.get("recipientLabel").value;
          this.FormEmailArray.at(this.emailIndex).get("emailRecipientLabel").setValue(this.emailLabel);
          this.FormEmailArray.at(this.emailIndex).get("emailContactModes").setValue(this.contactModeType);
        }
        else {
          this.emailIndex = this.emailIndex+1;
          this.FormEmailArray.push(this.initEmailItems());
          this.emailLabel = this.notificationForm.get("recipientLabel").value;
          this.FormEmailArray.at(this.emailIndex).get("emailRecipientLabel").setValue(this.emailLabel);
          this.FormEmailArray.at(this.emailIndex).get("emailContactModes").setValue(this.contactModeType);
        }
      }
      //this is for web service
      else if (this.contactModeType == 'W') {
        this.addWsFlag = true;
        this.wsCount = this.wsCount + 1;
        if (!this.FormWebArray) {
          this.FormWebArray = this.notificationForm.get("FormWebArray") as FormArray;
          this.wsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormWebArray.at(this.wsIndex).get("webRecipientLabel").setValue(this.wsLabel);
          this.FormWebArray.at(this.wsIndex).get("webContactModes").setValue(this.contactModeType);
        }
        else {
          this.wsIndex = this.wsIndex+1;
          this.FormWebArray.push(this.initWebItems());
          this.wsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormWebArray.at(this.wsIndex).get("webRecipientLabel").setValue(this.wsLabel);
          this.FormWebArray.at(this.wsIndex).get("webContactModes").setValue(this.contactModeType);
        }
     }
    }
    //for edit or duuplicate functionality
    else{
      this.contactModeType = data.notificationModeType;
      //this is for email
      if (this.contactModeType == 'E') {
        this.addEmailFlag = true;
        this.emailCount = this.emailCount + 1;
        if (!this.FormEmailArray) {
          this.FormEmailArray = this.notificationForm.get("FormEmailArray") as FormArray;
          this.FormEmailArray.at(this.emailIndex).get("emailAddress").setValue(data.emailId);
          this.FormEmailArray.at(this.emailIndex).get("mailDescription").setValue(data.emailText);
          this.FormEmailArray.at(this.emailIndex).get("emailRecipientLabel").setValue(data.recipientLabel);
          this.FormEmailArray.at(this.emailIndex).get("mailSubject").setValue(data.emailSub);
        }
        else {
          this.emailIndex = this.emailIndex+1;
          this.FormEmailArray.push(this.initEmailItems());
          this.FormEmailArray.at(this.emailIndex).get("emailAddress").setValue(data.emailId);
          this.FormEmailArray.at(this.emailIndex).get("mailDescription").setValue(data.emailText);
          this.FormEmailArray.at(this.emailIndex).get("emailRecipientLabel").setValue(data.recipientLabel);
          this.FormEmailArray.at(this.emailIndex).get("mailSubject").setValue(data.emailSub);
        }
      }
      //this is for web service
      else if (this.contactModeType == 'W') {
        this.addWsFlag = true;
        this.wsCount = this.wsCount + 1;
        if (!this.FormWebArray) {
          this.FormWebArray = this.notificationForm.get("FormWebArray") as FormArray;
          this.FormWebArray.at(this.wsIndex).get("webURL").setValue(data.wsUrl);
          this.FormWebArray.at(this.wsIndex).get("wsDescription").setValue(data.wsText);
          this.FormWebArray.at(this.wsIndex).get("authentication").setValue(data.wsType);
          this.FormWebArray.at(this.wsIndex).get("webRecipientLabel").setValue(data.recipientLabel);     
          if(data.wsType == 'A'){
          this.FormWebArray.at(this.wsIndex).get("loginId").setValue(data.wsLogin);
          this.FormWebArray.at(this.wsIndex).get("password").setValue(data.wsPassword);
          this.FormWebArray.at(this.wsIndex).get("wsTextDescription").setValue(data.wsText);
          }
        }
        else {
          this.wsIndex = this.wsIndex+1;
          this.FormWebArray.push(this.initWebItems());
          this.FormWebArray.at(this.wsIndex).get("webURL").setValue(data.wsUrl);
          this.FormWebArray.at(this.wsIndex).get("wsDescription").setValue(data.wsText);
          this.FormWebArray.at(this.wsIndex).get("authentication").setValue(data.wsType);
          this.FormWebArray.at(this.wsIndex).get("webRecipientLabel").setValue(data.recipientLabel);
          if(data.wsType == 'A'){
          this.FormWebArray.at(this.wsIndex).get("loginId").setValue(data.wsLogin);
          this.FormWebArray.at(this.wsIndex).get("password").setValue(data.wsPassword);
          this.FormWebArray.at(this.wsIndex).get("wsTextDescription").setValue(data.wsText);
          }
        }
     }
    }
     }

     deleteWebNotificationRow(index :number){
      this.FormWebArray.removeAt(index);
      console.log("deleted");
      this.wsIndex = this.wsIndex - 1;
      this.wsCount = this.wsCount - 1;
     }

     deleteEmailNotificationRow(index :number){
      this.FormEmailArray.removeAt(index);
      console.log("deleted");
      this.emailIndex = this.emailIndex - 1;
      this.emailCount = this.emailCount - 1;
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
   this.notificationReceipients= [];


     let WsData;
    let EmailData;
   if(this.FormWebArray && this.FormWebArray.length > 0){
   this.FormWebArray.controls.forEach((element, index) => {
     WsData = element['controls'];
  let webPayload = {
  recipientLabel: WsData.webRecipientLabel.value,
  accountGroupId: this.organizationId,
  notificationModeType: WsData.webContactModes.value,
  phoneNo: "",
  sms: "",
  emailId: "",
  emailSub: "",
  emailText: "",
  wsUrl: WsData.webURL.value,
  wsType: WsData.authentication.value,
  wsText: WsData.wsDescription.value,
  wsLogin: WsData.loginId.value,
  wsPassword: WsData.password.value
}
this.notificationReceipients.push(webPayload);

   });

}

  if(this.FormEmailArray && this.FormEmailArray.length > 0)
  {
  this.FormEmailArray.controls.forEach((item,index)=>{
    EmailData = item['controls'];
    let emailPayload = {
          recipientLabel: EmailData.emailRecipientLabel.value,
          accountGroupId: this.organizationId,
          notificationModeType: EmailData.emailContactModes.value,
          phoneNo: "",
          sms: "",
          emailId: EmailData.emailAddress.value,
          emailSub: EmailData.mailSubject.value,
          emailText: EmailData.mailDescription.value,
          wsUrl: "",
          wsType: "",
          wsText: "",
          wsLogin: "",
          wsPassword: ""
    }
    this.notificationReceipients.push(emailPayload);
  });

}   

this.notifications = [
  {
    "alertUrgencyLevelType": "C",
    "frequencyType": "O",
    "frequencyThreshholdValue": 0,
    "validityType": "A",
    "createdBy": 0,
    "notificationRecipients": this.notificationReceipients,
    "notificationLimits": [],
    "notificationAvailabilityPeriods": []
  }
  ]

  return this.notifications;
}

}

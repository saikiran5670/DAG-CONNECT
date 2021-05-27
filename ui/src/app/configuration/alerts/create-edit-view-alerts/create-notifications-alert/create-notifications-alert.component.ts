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
  notificationForm: FormGroup;
  FormArrayItems : FormArray;
  FormEmailArray : FormArray;
  FormWebArray : FormArray;
  ArrayList : any =[];
  notificationIndex : any = 0;
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
  radioButtonVal: any;
  notificationPayload: any = [];
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
notifyPeriod: any;
emailCount : number = 0;
wsCount : number = 0;
emailLabel : any;
wsLabel: any;
  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
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
    
    }

    initItems(): FormGroup{
      return this._formBuilder.group({
        emailAddress: ['', [Validators.required, Validators.email]],
        mailSubject: ['', [Validators.required]],
        mailDescription: ['', [Validators.required]],
        wsDescription: ['', [Validators.required]],
        authentication:['', [Validators.required]],
        loginId: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]],
        webURL:['', [Validators.required]],
        wsTextDescription:[''],
        notifyPeriod: ['A']
        });
    }

    initEmailItems(): FormGroup{
      return this._formBuilder.group({
        emailAddress: ['', [Validators.required, Validators.email]],
        mailSubject: ['', [Validators.required]],
        mailDescription: ['', [Validators.required]],
        notifyPeriod: ['A'],
        emailRecipientLabel: [''],
        emailContactModes: ['']
      });

    }

    initWebItems(): FormGroup{
      return this._formBuilder.group({
        wsDescription: ['', [Validators.required]],
        authentication:['', [Validators.required]],
        loginId: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]],
        webURL:['', [Validators.required]],
        wsTextDescription:[''],
        notifyPeriodweb: ['A'],        
        webRecipientLabel: [''],
        webContactModes: ['']
      });

    }


    addMultipleItems(data) :void{

      this.contactModeType = this.notificationForm.get("contactMode").value;
      //this is for email
      if (this.contactModeType == 1) {
        this.addEmailFlag = true;
        this.emailCount = this.emailCount + 1;
        if (!this.FormEmailArray) {
          this.FormEmailArray = this.notificationForm.get("FormEmailArray") as FormArray;
          this.emailLabel = this.notificationForm.get("recipientLabel").value;
          this.FormEmailArray.at(this.notificationIndex).get("emailRecipientLabel").setValue(this.emailLabel);
          this.FormEmailArray.at(this.notificationIndex).get("emailContactModes").setValue(this.contactModeType);
        }
        else {
          this.notificationIndex = this.notificationIndex+1;
          this.FormEmailArray.push(this.initEmailItems());
          this.emailLabel = this.notificationForm.get("recipientLabel").value;
          this.FormEmailArray.at(this.notificationIndex).get("emailRecipientLabel").setValue(this.emailLabel);
          this.FormEmailArray.at(this.notificationIndex).get("emailContactModes").setValue(this.contactModeType);
        }
        console.log("FormEmailArray=" +this.notificationForm['controls']['FormEmailArray']['controls'][0]['controls']);
      }
      //this is for web service
      else if (this.contactModeType == 0) {
        this.addWsFlag = true;
        this.wsCount = this.wsCount + 1;
        if (!this.FormWebArray) {
          this.FormWebArray = this.notificationForm.get("FormWebArray") as FormArray;
          this.wsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormWebArray.at(this.notificationIndex).get("webRecipientLabel").setValue(this.wsLabel);
          this.FormWebArray.at(this.notificationIndex).get("webContactModes").setValue(this.contactModeType);
        }
        else {
          this.notificationIndex = this.notificationIndex+1;
          this.FormWebArray.push(this.initWebItems());
          this.wsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormWebArray.at(this.notificationIndex).get("webRecipientLabel").setValue(this.wsLabel);
          this.FormWebArray.at(this.notificationIndex).get("webContactModes").setValue(this.contactModeType);
        }
        console.log("FormWebArray= " +this.notificationForm['controls']['FormEmailArray']['controls'][0]['controls']);
      }
      console.log("----------------last details--------");
      console.log("formArray= "+JSON.stringify(data));
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
    // let tempObj= {
    //   name: "ABCD"
    // }
    // return tempObj;

   let WsData;
   let EmailData;
   if(this.contactModeType == 0){
   this.FormWebArray.controls.forEach((element, index) => {
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
this.notificationPayload.push(webPayload);
console.log("this is web=" +JSON.stringify(this.notificationPayload));
   });

  }

  if(this.contactModeType == 1)
  {
  this.FormEmailArray.controls.forEach((item,index)=>{
    let emailPayload = {
          recipientLabel: WsData.emailRecipientLabel,
          accountGroupId: this.organizationId,
          notificationModeType: WsData.emailContactModes,
          phoneNo: "",
          sms: "",
          emailId: EmailData.emailAddress,
          emailSub: EmailData.mailSubject,
          emailText: EmailData.mailDescription,
          wsUrl: "",
          wsType: "",
          wsText: "",
          wsLogin: "",
          wsPassword: ""
    }
    this.notificationPayload.push(emailPayload);
    console.log("this is email=" +this.notificationPayload);
  });
}

  }

}

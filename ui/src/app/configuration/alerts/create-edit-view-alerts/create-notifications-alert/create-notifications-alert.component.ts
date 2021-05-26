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
          this.FormEmailArray.at(0).get("emailRecipientLabel").setValue(this.emailLabel);
          this.FormEmailArray.at(0).get("emailContactModes").setValue(this.contactModeType);
        }
        else {
          let i = 0;
          i = i+1;
          this.FormEmailArray.push(this.initEmailItems());
          this.emailLabel = this.notificationForm.get("recipientLabel").value;
          this.FormEmailArray.at(i).get("emailRecipientLabel").setValue(this.emailLabel);
          this.FormEmailArray.at(i).get("emailContactModes").setValue(this.contactModeType);
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
          this.FormWebArray.at(0).get("webRecipientLabel").setValue(this.wsLabel);
          this.FormWebArray.at(0).get("webContactModes").setValue(this.contactModeType);
        }
        else {
          let i = 0;
          i = i+1;
          this.FormWebArray.push(this.initWebItems());
          this.wsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormWebArray.at(i).get("webRecipientLabel").setValue(this.wsLabel);
          this.FormWebArray.at(i).get("webContactModes").setValue(this.contactModeType);
        }
        console.log("FormWebArray= " +this.notificationForm['controls']['FormEmailArray']['controls'][0]['controls']);
      }
      console.log("----------------last details--------");
      console.log("formArray= "+data);
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
  

  onChangeContactMode(event :any){
   this.contactModeType = event.value;
  //  this.addFlag = false;
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

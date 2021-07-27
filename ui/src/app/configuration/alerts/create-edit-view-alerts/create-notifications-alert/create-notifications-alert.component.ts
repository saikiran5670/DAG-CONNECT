import { ElementRef, Input, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { EmailValidator, FormArray, FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { SearchCountryField, CountryISO } from 'ngx-intl-tel-input';
import { element } from 'protractor';
import { AlertService } from 'src/app/services/alert.service';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { NotificationAdvancedFilterComponent } from './notification-advanced-filter/notification-advanced-filter.component';

@Component({
  selector: 'app-create-notifications-alert',
  templateUrl: './create-notifications-alert.component.html',
  styleUrls: ['./create-notifications-alert.component.less']
})
export class CreateNotificationsAlertComponent implements OnInit {
  @Input() translationData: any = [];
  @Input() selectedRowData: any;
  notificationForm: FormGroup;
  FormArrayItems: FormArray;
  FormEmailArray: FormArray;
  FormWebArray: FormArray;
  FormSMSArray: FormArray;
  ArrayList: any = [];
  emailIndex: any = 0;
  wsIndex: any = 0;
  smsIndex: any= 0;
  @Input() alert_category_selected: any;
  @Input() alertTypeName: string;
  @Input() isCriticalLevelSelected: any;
  @Input() isWarningLevelSelected: any;
  @Input() isAdvisoryLevelSelected: any;
  @Input() labelForThreshold: any;
  @Input() alert_type_selected: string;
  @Input() actionType: any;
  localStLanguage: any;
  organizationId: number;
  accountId: number;
  addEmailFlag: boolean = false;
  addWsFlag: boolean = false;
  addSmsFlag: boolean= false;
  contactModeType: any;
  radioButtonVal: any = 'N';
  notificationReceipients: any = [];
  notifications: any = [];
  openAdvancedFilter: boolean = false;
  contactModes: any = [
    {
      id: 'W',
      value: 'Web Service'
    },
    {
      id: "E",
      value: 'Email'
    },
    {
      id: "S",
      value: 'SMS'
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
  emailCount: number = 0;
  wsCount: number = 0;
  smsCount: number= 0;
  emailLabel: any;
  wsLabel: any;
  smsLabel: any;
  limitButton: any;
  weblimitButton: any;
  smsLimitButton: any;
  notificationRecipients = [];
  keyword = 'recipientLabel';
  timeList: any = [
    {
      id: 'M',
      value: 'Minutes'
    },
    {
      id: 'H',
      value: 'Hours'
    },
    {
      id: 'D',
      value: 'Days'
    },
    {
      id: 'W',
      value: 'Weeks'
    },
  ];
  emailTimeList: any = [
    {
      id: 'M',
      value: "Minutes"
    },
    {
      id: 'H',
      value: "Hours"
    },
    {
      id: 'D',
      value: 'Days'
    },
    {
      id: 'W',
      value: 'Weeks'
    },
  ];
  timeUnitValue: any;
  emailtimeUnitValue: any;
  smsTimeUnitValue: any;
  SearchCountryField;
  // TooltipLabel = TooltipLabel;
  CountryISO;
  preferredCountries: CountryISO[];
  isDuplicateRecipientLabel: boolean= false;

  @ViewChild(NotificationAdvancedFilterComponent)
  notificationAdvancedFilterComponent: NotificationAdvancedFilterComponent;

  constructor(private _formBuilder: FormBuilder, private alertService: AlertService, private el: ElementRef) { }

  ngOnInit(): void {
    console.log("action type=" + this.actionType);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.notificationForm = this._formBuilder.group({
      recipientLabel: ['', [Validators.required]],
      contactMode: ['', [Validators.required]],
      criticalLevel: [''],
      warningLevel: [''],
      advisoryLevel: [''],
      // FormArrayItems : this._formBuilder.array([this.initItems()]),
      FormEmailArray: this._formBuilder.array([this.initEmailItems()]),
      FormWebArray: this._formBuilder.array([this.initWebItems()]),
      FormSMSArray: this._formBuilder.array([this.initSMSItems()])
    },
      {
        validator: [
          CustomValidators.specialCharValidationForName('recipientLabel'),
        ]
      });
    console.log(this.selectedRowData);

    this.SearchCountryField = SearchCountryField;
    // TooltipLabel = TooltipLabel;
    this.CountryISO = CountryISO;
    this.preferredCountries = [CountryISO.India];

    if (this.actionType == 'create' || this.actionType == 'edit' || this.actionType == 'duplicate') {
      this.alertService.getNotificationRecipients(this.organizationId).subscribe(data => {
        this.notificationReceipients = data;
      })
    }
    if ((this.actionType == 'edit' || this.actionType == 'duplicate') &&
      this.selectedRowData.notifications.length > 0 &&
      this.selectedRowData.notifications[0].notificationRecipients.length > 0) {
      this.setDefaultValues();
    }

    // if (this.actionType == 'view') {
    //   this.openAdvancedFilter = true;
    // }
  }


  initEmailItems(): FormGroup {
    return this._formBuilder.group({
      emailAddress: ['', [Validators.required, Validators.email]],
      mailSubject: ['This is default subject for ' + this.alertTypeName, [Validators.required]],
      mailDescription: ['This is default text for ' + this.alertTypeName, [Validators.required]],
      notifyPeriod: ['A'],
      emailRecipientLabel: [''],
      emailContactModes: [''],
      receipientId: [],
      retrictTo: ['1'],
      emailEach: ['1'],
      minutes: ['1'],
      emailllimitId: []
    });

  }

  initWebItems(): FormGroup {
    return this._formBuilder.group({
      wsDescription: ['This is default text for ' + this.alertTypeName, [Validators.required]],
      authentication: ['N', [Validators.required]],
      loginId: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      webURL: ['', [Validators.required]],
      wsTextDescription: ['This is default text for ' + this.alertTypeName],
      notifyPeriodweb: ['A'],
      webRecipientLabel: [''],
      webContactModes: [''],
      receipientId: [],
      widthInput: [''],
      webretrictTo: ['1'],
      webEach: ['1'],
      webminutes: ['1'],
      weblimitId: []
    });

  }

  initSMSItems(): FormGroup {
    return this._formBuilder.group({
      mobileNumber: new FormControl('', [Validators.required]),
      smsDescription: ['This is default text for ' + this.alertTypeName, [Validators.required]],
      notifyPeriodSms: ['A'],
      smsRecipientLabel: [''],
      smsContactModes: [''],
      smsReceipientId: [],
      smsRetrictTo: ['1'],
      smsEach: ['1'],
      smsMinutes: ['1'],
      smslimitId: []
    });

  }

  onReset() {
    this.setDefaultValues();
    if (this.notificationAdvancedFilterComponent) {
      this.notificationAdvancedFilterComponent.setDefaultValues();
    }
  }

  setAlertType(alertType: any) {
    this.notificationAdvancedFilterComponent.setAlertType(alertType);
  }

  onChangeTimeUnit(event: any) {
    this.timeUnitValue = event.value;
  }

  onChangeTimeUnitForEmail(event: any) {
    this.emailtimeUnitValue = event.value;
  }

  onChangeTimeUnitForSMS(event: any) {
    this.smsTimeUnitValue = event.value;
  }


  setDefaultValues() {
    if (this.FormWebArray && this.FormWebArray.length != 0) {
      for (let i = 0; i < this.FormWebArray.length; i++) {
        this.deleteWebNotificationRow(i);
      }
    }
    if (this.FormEmailArray && this.FormEmailArray.length != 0) {
      for (let i = 0; i < this.FormEmailArray.length; i++) {
        this.deleteEmailNotificationRow(i);
      }
    }
    if (this.FormSMSArray && this.FormSMSArray.length != 0) {
      for (let i = 0; i < this.FormSMSArray.length; i++) {
        this.deleteSMSNotificationRow(i);
      }
    }
    this.selectedRowData.notifications[0].notificationRecipients.forEach(element => {
      this.addMultipleItems(false, element);
    });
  }

  addMultipleItems(isButtonClicked: boolean, data?: any): void {
    if (isButtonClicked) {
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
          this.FormEmailArray.at(this.emailIndex).get("minutes").setValue(this.emailtimeUnitValue);
          this.FormEmailArray.at(this.emailIndex).get("emailllimitId").setValue(0);
          this.notificationForm.get("recipientLabel").reset();
          this.notificationForm.get("contactMode").reset();
        }
        else {
          this.emailIndex = this.emailIndex + 1;
          this.FormEmailArray.push(this.initEmailItems());
          this.emailLabel = this.notificationForm.get("recipientLabel").value;
          this.FormEmailArray.at(this.emailIndex).get("emailRecipientLabel").setValue(this.emailLabel);
          this.FormEmailArray.at(this.emailIndex).get("emailContactModes").setValue(this.contactModeType);
          this.FormEmailArray.at(this.emailIndex).get("minutes").setValue(this.emailtimeUnitValue);
          this.FormEmailArray.at(this.emailIndex).get("emailllimitId").setValue(0);
          this.notificationForm.get("recipientLabel").reset();
          this.notificationForm.get("contactMode").reset();
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
          this.FormWebArray.at(this.wsIndex).get("webminutes").setValue(this.timeUnitValue);
          this.FormWebArray.at(this.wsIndex).get("weblimitId").setValue(0);
          this.notificationForm.get("recipientLabel").reset();
          this.notificationForm.get("contactMode").reset();
        }
        else {
          this.wsIndex = this.wsIndex + 1;
          this.FormWebArray.push(this.initWebItems());
          this.wsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormWebArray.at(this.wsIndex).get("webRecipientLabel").setValue(this.wsLabel);
          this.FormWebArray.at(this.wsIndex).get("webContactModes").setValue(this.contactModeType);
          this.FormWebArray.at(this.wsIndex).get("webminutes").setValue(this.timeUnitValue);
          this.FormWebArray.at(this.wsIndex).get("weblimitId").setValue(0);
          this.notificationForm.get("recipientLabel").reset();
          this.notificationForm.get("contactMode").reset();
        }
      }
      // For sms
      else if (this.contactModeType == 'S') {
        this.addSmsFlag = true;
        this.smsCount = this.smsCount + 1;
        if (!this.FormSMSArray) {
          this.FormSMSArray = this.notificationForm.get("FormSMSArray") as FormArray;
          this.smsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormSMSArray.at(this.smsIndex).get("smsRecipientLabel").setValue(this.smsLabel);
          this.FormSMSArray.at(this.smsIndex).get("smsContactModes").setValue(this.contactModeType);
          this.FormSMSArray.at(this.smsIndex).get("smsMinutes").setValue(this.smsTimeUnitValue);
          this.FormSMSArray.at(this.smsIndex).get("smslimitId").setValue(0);
          this.notificationForm.get("recipientLabel").reset();
          this.notificationForm.get("contactMode").reset();
        }
        else {
          this.smsIndex = this.smsIndex + 1;
          this.FormSMSArray.push(this.initSMSItems());
          this.smsLabel = this.notificationForm.get("recipientLabel").value;
          this.FormSMSArray.at(this.smsIndex).get("smsRecipientLabel").setValue(this.smsLabel);
          this.FormSMSArray.at(this.smsIndex).get("smsContactModes").setValue(this.contactModeType);
          this.FormSMSArray.at(this.smsIndex).get("smsMinutes").setValue(this.smsTimeUnitValue);
          this.FormSMSArray.at(this.smsIndex).get("smslimitId").setValue(0);
          this.notificationForm.get("recipientLabel").reset();
          this.notificationForm.get("contactMode").reset();
        }
      }
    }
    //for edit or duplicate functionality
    else {
      this.notificationForm.get("recipientLabel").reset();
      this.contactModeType = data.notificationModeType;
      this.weblimitButton = data.notificationLimits[0].notificationModeType;
      this.limitButton = data.notificationLimits[0].notificationModeType;
      this.smsLimitButton= data.notificationLimits[0].notificationModeType;

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
          this.FormEmailArray.at(this.emailIndex).get("receipientId").setValue(data.id);
          this.FormEmailArray.at(this.emailIndex).get("emailContactModes").setValue(data.notificationModeType);
          this.FormEmailArray.at(this.emailIndex).get("notifyPeriod").setValue(data.notificationLimits[0].notificationModeType);
          this.FormEmailArray.at(this.emailIndex).get("minutes").setValue(data.notificationLimits[0].notificationPeriodType);
          this.FormEmailArray.at(this.emailIndex).get("retrictTo").setValue(data.notificationLimits[0].maxLimit);
          this.FormEmailArray.at(this.emailIndex).get("emailEach").setValue(data.notificationLimits[0].periodLimit);
          this.FormEmailArray.at(this.emailIndex).get("emailllimitId").setValue(data.notificationLimits[0].id);
        }
        else {
          this.emailIndex = this.emailIndex + 1;
          this.FormEmailArray.push(this.initEmailItems());
          this.FormEmailArray.at(this.emailIndex).get("emailAddress").setValue(data.emailId);
          this.FormEmailArray.at(this.emailIndex).get("mailDescription").setValue(data.emailText);
          this.FormEmailArray.at(this.emailIndex).get("emailRecipientLabel").setValue(data.recipientLabel);
          this.FormEmailArray.at(this.emailIndex).get("mailSubject").setValue(data.emailSub);
          this.FormEmailArray.at(this.emailIndex).get("receipientId").setValue(data.id);
          this.FormEmailArray.at(this.emailIndex).get("emailContactModes").setValue(data.notificationModeType);
          this.FormEmailArray.at(this.emailIndex).get("notifyPeriod").setValue(data.notificationLimits[0].notificationModeType);
          this.FormEmailArray.at(this.emailIndex).get("minutes").setValue(data.notificationLimits[0].notificationPeriodType);
          this.FormEmailArray.at(this.emailIndex).get("retrictTo").setValue(data.notificationLimits[0].maxLimit);
          this.FormEmailArray.at(this.emailIndex).get("emailEach").setValue(data.notificationLimits[0].periodLimit);
          this.FormEmailArray.at(this.emailIndex).get("emailllimitId").setValue(data.notificationLimits[0].id);
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
          this.FormWebArray.at(this.wsIndex).get("receipientId").setValue(data.id);
          this.FormWebArray.at(this.wsIndex).get("webContactModes").setValue(data.notificationModeType);
          this.FormWebArray.at(this.wsIndex).get("notifyPeriodweb").setValue(data.notificationLimits[0].notificationModeType);
          this.FormWebArray.at(this.wsIndex).get("webminutes").setValue(data.notificationLimits[0].notificationPeriodType);
          this.FormWebArray.at(this.wsIndex).get("webretrictTo").setValue(data.notificationLimits[0].maxLimit);
          this.FormWebArray.at(this.wsIndex).get("webEach").setValue(data.notificationLimits[0].periodLimit);
          this.FormWebArray.at(this.wsIndex).get("weblimitId").setValue(data.notificationLimits[0].id);
          if (data.wsType == 'A') {
            this.FormWebArray.at(this.wsIndex).get("loginId").setValue(data.wsLogin);
            this.FormWebArray.at(this.wsIndex).get("password").setValue(data.wsPassword);
            this.FormWebArray.at(this.wsIndex).get("wsTextDescription").setValue(data.wsText);
          }
        }
        else {
          this.wsIndex = this.wsIndex + 1;
          this.FormWebArray.push(this.initWebItems());
          this.FormWebArray.at(this.wsIndex).get("webURL").setValue(data.wsUrl);
          this.FormWebArray.at(this.wsIndex).get("wsDescription").setValue(data.wsText);
          this.FormWebArray.at(this.wsIndex).get("authentication").setValue(data.wsType);
          this.FormWebArray.at(this.wsIndex).get("webRecipientLabel").setValue(data.recipientLabel);
          this.FormWebArray.at(this.wsIndex).get("receipientId").setValue(data.id);
          this.FormWebArray.at(this.wsIndex).get("webContactModes").setValue(data.notificationModeType);
          this.FormWebArray.at(this.wsIndex).get("notifyPeriodweb").setValue(data.notificationLimits[0].notificationModeType);
          this.FormWebArray.at(this.wsIndex).get("webminutes").setValue(data.notificationLimits[0].notificationPeriodType);
          this.FormWebArray.at(this.wsIndex).get("webretrictTo").setValue(data.notificationLimits[0].maxLimit);
          this.FormWebArray.at(this.wsIndex).get("webEach").setValue(data.notificationLimits[0].periodLimit);
          this.FormWebArray.at(this.wsIndex).get("weblimitId").setValue(data.notificationLimits[0].id);
          if (data.wsType == 'A') {
            this.FormWebArray.at(this.wsIndex).get("loginId").setValue(data.wsLogin);
            this.FormWebArray.at(this.wsIndex).get("password").setValue(data.wsPassword);
            this.FormWebArray.at(this.wsIndex).get("wsTextDescription").setValue(data.wsText);
          }
        }
      }
      //this is for sms
      else if (this.contactModeType == 'S') {
        this.addSmsFlag = true;
        this.smsCount = this.smsCount + 1;
        if (!this.FormSMSArray) {
          this.FormSMSArray = this.notificationForm.get("FormSMSArray") as FormArray;
          this.FormSMSArray.at(this.smsIndex).get("mobileNumber").setValue(data.phoneNo);
          this.FormSMSArray.at(this.smsIndex).get("smsRecipientLabel").setValue(data.recipientLabel);
          this.FormSMSArray.at(this.smsIndex).get("smsDescription").setValue(data.sms);
          this.FormSMSArray.at(this.smsIndex).get("smsReceipientId").setValue(data.id);
          this.FormSMSArray.at(this.smsIndex).get("smsContactModes").setValue(data.notificationModeType);
          this.FormSMSArray.at(this.smsIndex).get("notifyPeriodSms").setValue(data.notificationLimits[0].notificationModeType);
          this.FormSMSArray.at(this.smsIndex).get("smsMinutes").setValue(data.notificationLimits[0].notificationPeriodType);
          this.FormSMSArray.at(this.smsIndex).get("smsRetrictTo").setValue(data.notificationLimits[0].maxLimit);
          this.FormSMSArray.at(this.smsIndex).get("smsEach").setValue(data.notificationLimits[0].periodLimit);
          this.FormSMSArray.at(this.smsIndex).get("smslimitId").setValue(data.notificationLimits[0].id);
        }
        else {
          this.smsIndex = this.smsIndex + 1;
          this.FormSMSArray.push(this.initSMSItems());
          this.FormSMSArray.at(this.smsIndex).get("mobileNumber").setValue(data.phoneNo);
          this.FormSMSArray.at(this.smsIndex).get("smsRecipientLabel").setValue(data.recipientLabel);
          this.FormSMSArray.at(this.smsIndex).get("smsDescription").setValue(data.sms);
          this.FormSMSArray.at(this.smsIndex).get("smsReceipientId").setValue(data.id);
          this.FormSMSArray.at(this.smsIndex).get("smsContactModes").setValue(data.notificationModeType);
          this.FormSMSArray.at(this.smsIndex).get("notifyPeriodSms").setValue(data.notificationLimits[0].notificationModeType);
          this.FormSMSArray.at(this.smsIndex).get("smsMinutes").setValue(data.notificationLimits[0].notificationPeriodType);
          this.FormSMSArray.at(this.smsIndex).get("smsRetrictTo").setValue(data.notificationLimits[0].maxLimit);
          this.FormSMSArray.at(this.smsIndex).get("smsEach").setValue(data.notificationLimits[0].periodLimit);
          this.FormSMSArray.at(this.smsIndex).get("smslimitId").setValue(data.notificationLimits[0].id);
        }
      }
    }
  }

  deleteWebNotificationRow(index: number) {
    this.FormWebArray.removeAt(index);
    console.log("deleted");
    this.wsIndex = this.wsIndex - 1;
    this.wsCount = this.wsCount - 1;
  }

  deleteEmailNotificationRow(index: number) {
    this.FormEmailArray.removeAt(index);
    console.log("deleted");
    this.emailIndex = this.emailIndex - 1;
    this.emailCount = this.emailCount - 1;
  }

  deleteSMSNotificationRow(index: number) {
    this.FormSMSArray.removeAt(index);
    console.log("deleted");
    this.smsIndex = this.smsIndex - 1;
    this.smsCount = this.smsCount - 1;
  }

  setDefaultValueForws() {
    this.webURL = "";
    this.wsDescription = "";
    this.authentication = "";
    this.loginId = "";
    this.password = "";
  }

  setDefaultValueForemail() {
    this.emailAddress = "";
    this.mailSubject = ""
    this.mailDescription = "";
  }

  onRadioButtonChange(event: any) {
    this.radioButtonVal = event.value;
  }

  onLimitationButtonChange(event: any) {
    this.limitButton = event.value;
  }

  onWebLimitationButtonChange(event: any) {
    this.weblimitButton = event.value;
  }

  onSMSLimitationButtonChange(event: any) {
    this.smsLimitButton = event.value;
  }

  onClickAdvancedFilter() {
    this.openAdvancedFilter = !this.openAdvancedFilter;
  }

  onChangeCriticalLevel(event: any) {

  }

  onChangeWarningLevel(event: any) {

  }

  onChangeAdvisoryLevel(event: any) {

  }

  duplicateRecipientLabel(){
    this.isDuplicateRecipientLabel= true;
    const invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'contactMode' + '"]');
    invalidControl.focus();
  }

  getNotificationDetails(): any {
    this.isDuplicateRecipientLabel= false;
    this.notificationReceipients = [];
    let WsData;
    let EmailData;
    let smsData;
    let webPayload = {};

    if (this.FormWebArray && this.FormWebArray.length > 0) {
      if (this.actionType == 'create' || this.actionType == 'duplicate') {
        this.FormWebArray.controls.forEach((element, index) => {
          let webNotificationLimits = [];
          let obj = {};
          WsData = element['controls'];
          let webrestrict = parseInt(WsData.webretrictTo.value);
          let limitVal = parseInt(WsData.webEach.value);
          if (WsData.notifyPeriodweb.value == 'A') {
            obj = {
              "id": 0,
              "recipientId": 0,
              "notificationId": 0,
              "notificationModeType": "A",
              "maxLimit": 0,
              "notificationPeriodType": "D",
              "periodLimit": 0
            }
          }
          else if (WsData.notifyPeriodweb.value == 'C') {
            obj =
            {
              "id": 0,
              "recipientId": 0,
              "notificationId": 0,
              "notificationModeType": 'C',
              "maxLimit": webrestrict,
              "notificationPeriodType": WsData.webminutes.value,
              "periodLimit": limitVal
            }
          }
          webNotificationLimits.push(obj);

          webPayload = {
            id: WsData.receipientId.value ? WsData.receipientId.value : 0,
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
            wsPassword: WsData.password.value,
            notificationLimits: webNotificationLimits
          }
          this.notificationReceipients.push(webPayload);

        });
      }
      else if (this.actionType == 'edit') {
        this.FormWebArray.controls.forEach((element, index) => {
          let webNotificationLimits = [];
          let obj = {};
          WsData = element['controls'];
          let webrestrict = parseInt(WsData.webretrictTo.value);
          let limitVal = parseInt(WsData.webEach.value);
          if (WsData.notifyPeriodweb.value == 'A') {
            obj = {
              "id": WsData.weblimitId.value,
              "recipientId":WsData.receipientId.value,
              "notificationId": this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
              "notificationModeType": "A",
              "maxLimit": 0,
              "notificationPeriodType": "D",
              "periodLimit": 0
            }
          }
          else if (WsData.notifyPeriodweb.value == 'C') {
            obj =
            {
              "id": WsData.weblimitId.value,
              "recipientId":WsData.receipientId.value,
              "notificationId": this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
              "notificationModeType": 'C',
              "maxLimit": webrestrict,
              "notificationPeriodType": WsData.webminutes.value,
              "periodLimit": limitVal
            }
          }
          webNotificationLimits.push(obj);
          webPayload = {
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
            wsPassword: WsData.password.value,
            id: WsData.receipientId.value ? WsData.receipientId.value : 0,
            notificationId: this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
            notificationLimits: webNotificationLimits
          }
          this.notificationReceipients.push(webPayload);
        });
      }

    }

    if (this.FormEmailArray && this.FormEmailArray.length > 0) {
      let emailPayload = {};

      if (this.actionType == 'create' || this.actionType == 'duplicate') {
        this.FormEmailArray.controls.forEach((item, index) => {
          let emailNotificationLimits = [];
          let obj = {};
          EmailData = item['controls'];
          let restrictTo = parseInt(EmailData.retrictTo.value);
          let limitVal = parseInt(EmailData.emailEach.value);
          if (EmailData.notifyPeriod.value == 'A') {
            obj = {
              "id": 0,
              "recipientId": 0,
              "notificationId": 0,
              "notificationModeType": "A",
              "maxLimit": 0,
              "notificationPeriodType": "D",
              "periodLimit": 0
            }
          }
          else if (EmailData.notifyPeriod.value == 'C') {
            obj =
            {
              "id": 0,
              "recipientId": 0,
              "notificationId": 0,
              "notificationModeType": 'C',
              "maxLimit": restrictTo,
              "notificationPeriodType": EmailData.minutes.value,
              "periodLimit": limitVal
            }
          }
          emailNotificationLimits.push(obj);
          emailPayload = {
            id: EmailData.receipientId.value ? EmailData.receipientId.value : 0,
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
            wsPassword: "",
            notificationLimits: emailNotificationLimits
          }
          this.notificationReceipients.push(emailPayload);
        });
      }
      else if (this.actionType == 'edit') {
        this.FormEmailArray.controls.forEach((item, index) => {
          let emailNotificationLimits = [];
          let obj = {};
          EmailData = item['controls'];
          let restrictTo = parseInt(EmailData.retrictTo.value);
          let limitVal = parseInt(EmailData.emailEach.value);
          if (EmailData.notifyPeriod.value == 'A') {
            obj = {
              "id": EmailData.emailllimitId.value,
              "recipientId": EmailData.receipientId.value ? EmailData.receipientId.value : 0,
              "notificationId": this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
              "notificationModeType": "A",
              "maxLimit": 0,
              "notificationPeriodType": "D",
              "periodLimit": 0
            }
          }
          else if (EmailData.notifyPeriod.value == 'C') {
            obj =
            {
              "id": EmailData.emailllimitId.value,
              "recipientId": EmailData.receipientId.value ? EmailData.receipientId.value : 0,
              "notificationId": this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
              "notificationModeType": 'C',
              "maxLimit": restrictTo,
              "notificationPeriodType": EmailData.minutes.value,
              "periodLimit": limitVal
            }
          }
          emailNotificationLimits.push(obj);
          emailPayload = {
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
            wsPassword: "",
            id: EmailData.receipientId.value ? EmailData.receipientId.value : 0,
            notificationId: this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
            notificationLimits: emailNotificationLimits
          }
          this.notificationReceipients.push(emailPayload);
        });
      }
    }

    if (this.FormSMSArray && this.FormSMSArray.length > 0) {
      let smsPayload = {};
      
      if (this.actionType == 'create' || this.actionType == 'duplicate') {
        this.FormSMSArray.controls.forEach((item, index) => {
          let smsNotificationLimits = [];
          let obj = {};
          smsData = item['controls'];
          let restrictTo = parseInt(smsData.smsRetrictTo.value);
          let limitVal = parseInt(smsData.smsEach.value);
          if (smsData.notifyPeriodSms.value == 'A') {
            obj = {
              "id": 0,
              "recipientId": 0,
              "notificationId": 0,
              "notificationModeType": "A",
              "maxLimit": 0,
              "notificationPeriodType": "D",
              "periodLimit": 0
            }
          }
          else if (smsData.notifyPeriodSms.value == 'C') {
            obj =
            {
              "id": 0,
              "recipientId": 0,
              "notificationId": 0,
              "notificationModeType": 'C',
              "maxLimit": restrictTo,
              "notificationPeriodType": smsData.smsMinutes.value,
              "periodLimit": limitVal
            }
          }
          smsNotificationLimits.push(obj);
          smsPayload = {
            id: smsData.smsReceipientId.value ? smsData.smsReceipientId.value : 0,
            recipientLabel: smsData.smsRecipientLabel.value,
            accountGroupId: this.organizationId,
            notificationModeType: smsData.smsContactModes.value,
            phoneNo: smsData.mobileNumber.value.internationalNumber,
            sms: smsData.smsDescription.value,
            emailId: "",
            emailSub: "",
            emailText: "",
            wsUrl: "",
            wsType: "",
            wsText: "",
            wsLogin: "",
            wsPassword: "",
            notificationLimits: smsNotificationLimits
          }
          this.notificationReceipients.push(smsPayload);
        });
      }
      else if (this.actionType == 'edit') {
        this.FormSMSArray.controls.forEach((item, index) => {
          let smsNotificationLimits = [];
          let obj = {};
          smsData = item['controls'];
          let restrictTo = parseInt(smsData.smsRetrictTo.value);
          let limitVal = parseInt(smsData.smsEach.value);
          if (smsData.notifyPeriodSms.value == 'A') {
            obj = {
              "id": smsData.smslimitId.value,
              "recipientId": smsData.smsReceipientId.value ? smsData.smsReceipientId.value : 0,
              "notificationId": this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
              "notificationModeType": "A",
              "maxLimit": 0,
              "notificationPeriodType": "D",
              "periodLimit": 0
            }
          }
          else if (smsData.notifyPeriodSms.value == 'C') {
            obj =
            {
              "id": smsData.smslimitId.value,
              "recipientId": smsData.smsReceipientId.value ? smsData.smsReceipientId.value : 0,
              "notificationId": this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
              "notificationModeType": 'C',
              "maxLimit": restrictTo,
              "notificationPeriodType": smsData.smsMinutes.value,
              "periodLimit": limitVal
            }
          }
          smsNotificationLimits.push(obj);
          smsPayload = {
            recipientLabel: smsData.smsRecipientLabel.value,
            accountGroupId: this.organizationId,
            notificationModeType: smsData.smsContactModes.value,
            phoneNo: smsData.mobileNumber.value.internationalNumber,
            sms: smsData.smsDescription.value,
            emailId: "",
            emailSub: "",
            emailText: "",
            wsUrl: "",
            wsType: "",
            wsText: "",
            wsLogin: "",
            wsPassword: "",
            id: smsData.smsReceipientId.value ? smsData.smsReceipientId.value : 0,
            notificationId: this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
            notificationLimits: smsNotificationLimits
          }
          this.notificationReceipients.push(smsPayload);
        });
      }
    }

    let notificationAdvancedFilterObj;
    if (this.openAdvancedFilter) {
      notificationAdvancedFilterObj = this.notificationAdvancedFilterComponent.getNotificationAdvancedFilter();
    }
    if (this.actionType == 'create' || this.actionType == 'duplicate') {
      this.notifications = [
        {
          "alertUrgencyLevelType": "C",
          "frequencyType": notificationAdvancedFilterObj ? notificationAdvancedFilterObj.frequencyType : 'T',
          "frequencyThreshholdValue": 0,
          "validityType": notificationAdvancedFilterObj ? notificationAdvancedFilterObj.validityType : 'A',
          "createdBy": this.accountId,
          "notificationRecipients": this.notificationReceipients,
          "alertTimingDetails": notificationAdvancedFilterObj ? notificationAdvancedFilterObj.alertTimingRef : []
        }
      ]
    }
    else if (this.actionType == 'edit') {
      this.notifications = [
        {
          "alertUrgencyLevelType": "C",
          "frequencyType": notificationAdvancedFilterObj ? notificationAdvancedFilterObj.frequencyType : 'T',
          "frequencyThreshholdValue": 0,
          "validityType": notificationAdvancedFilterObj ? notificationAdvancedFilterObj.validityType : 'A',
          "createdBy": this.selectedRowData.createdBy,
          "id": this.selectedRowData.notifications.length > 0 ? this.selectedRowData.notifications[0].id : 0,
          "alertId": this.selectedRowData.id,
          "modifiedBy": this.accountId,
          "notificationRecipients": this.notificationReceipients,
          "alertTimingDetails": notificationAdvancedFilterObj ? notificationAdvancedFilterObj.alertTimingRef : []
        }
      ]
    }

    return this.notifications;
  }

}

import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { AuthService } from '../../services/auth.service';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { LoginDialogComponent } from './login-dialog/login-dialog.component';
import { CookieService } from 'ngx-cookie-service';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';
import { TermsConditionsPopupComponent } from 'src/app/terms-conditions-content/terms-conditions-popup.component';
import { TranslationService } from 'src/app/services/translation.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { TermsAndConditionPopupComponent } from './terms-and-condition-popup/terms-and-condition-popup.component';

export interface Organization {
  id: number ;
  name: string;
}
export interface Role {
  id: number;
  name: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})

export class LoginComponent implements OnInit {
  public loginForm: FormGroup;
  public forgotPasswordForm: FormGroup;
  hide: boolean = true;
  invalidUserMsg: boolean = false;
  errorMsg: string= '';
  cookiesFlag: boolean = true;
  forgotPwdFlag: boolean = false;
  resetPwdFlag: boolean = false;
  dialogRefLogin: MatDialogRef<LoginDialogComponent>;
  maintenancePopupFlag: boolean = false;
  loginClicks = 0;
  dialogRefTerms: MatDialogRef<TermsConditionsPopupComponent>;
  translationData: any = {};
  showLoadingIndicator: any = false;
  resetPwdOnedayFlag : boolean = false;
  resetPwdOnedayMsg : string = '';
  dialogRef: MatDialogRef<TermsAndConditionPopupComponent>;

  constructor(private cookieService: CookieService, public fb: FormBuilder, public router: Router, public authService: AuthService, private dialogService: ConfirmDialogService, private dialog: MatDialog, private accountService: AccountService, private dataInterchangeService: DataInterchangeService, private translationService: TranslationService, private organizationService: OrganizationService) {
    this.loginForm = this.fb.group({
      // 'username': [null, Validators.compose([Validators.required, Validators.email])],
      // 'password': [null, Validators.compose([Validators.required, Validators.minLength(6)])]
      'username': [null, Validators.compose([Validators.required])],
      'password': [null, Validators.compose([Validators.required])]
    });
    this.forgotPasswordForm = this.fb.group({
      'emailId': [null, Validators.compose([Validators.required, Validators.maxLength(120), Validators.pattern("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+[.]+[a-zA-Z]{2,4}$")])]
    });

    this.cookiesFlag = this.cookieService.get('cookiePolicy') ? false : true;
  }

  ngOnInit(): void {
  }


  public onLogin(values: Object) {
    this.showLoadingIndicator = true;
    this.errorMsg= '';
    this.invalidUserMsg = false;
    if (this.loginForm.valid) { 
      ////console.log("values:: ", values)
      // if(this.loginClicks == 0){//commenting this for facing issue multiple times login ,popup is not getting displayed.
        this.loginClicks = 1;
       this.authService.signIn(this.loginForm.value).subscribe((data:any) => {
         localStorage.setItem('isLoginSetUser', 'true'); //For checking user login, to remove unwanted setuserselection call.
        this.hideLoader();
         ////console.log("data:: ", data)
         if(data.status === 200){
           
            if(data.body.accountInfo){
              localStorage.setItem('accountId', data.body.accountInfo.id ? data.body.accountInfo.id : 0);
            }
        
            if(data.body.accountOrganization.length > 0){
              localStorage.setItem('accountOrganizationId', data.body.accountOrganization[0].id);
              localStorage.setItem("organizationName", data.body.accountOrganization[0].name);
            }
      
            if(data.body.accountRole.length > 0){
              localStorage.setItem('accountRoleId', data.body.accountRole[0].id);
              localStorage.setItem('roleLevel', data.body.accountRole[0].level);
            }
            
            if(data.body && data.body.accountInfo && data.body.accountInfo.id) {
              if(data.body.accountInfo.preferenceId && data.body.accountInfo.preferenceId != 0){
                this.accountService.getAccountPreference(data.body.accountInfo.preferenceId).subscribe(accPref => {
                  localStorage.setItem("liveFleetTimer", (accPref['pageRefreshTime']*60).toString());  // default set
                  this.showOrganizationRolePopup(data.body, data.body.accountInfo, accPref); 
                })
              }
              else{
                if(data.body.accountOrganization.length > 0){
                  this.organizationService.getOrganizationPreference(data.body.accountOrganization[0].id).subscribe((orgPref: any)=>{
                    localStorage.setItem("orgPref", JSON.stringify(orgPref));
                    this.openOrgRolePopup(data);
                  }, (error) => {
                    this.openOrgRolePopup(data);
                  });
                } else {
                  this.openOrgRolePopup(data);
                }
              } 
            }else{
              this.loginClicks = 0;
              this.invalidUserMsg= true;
            }

               //this.cookiesFlag = true;
            // let sessionObject: any = {
            //   accountId: data.body.accountInfo.id,
            //   orgId:  data.body.accountOrganization[0].id,
            //   roleId: data.body.accountRole[0].id
            // }
            // this.accountService.setUserSelection(sessionObject).subscribe((data) =>{
            // });
         }
         else if(data.status === 401){
          this.invalidUserMsg = true;
          this.loginClicks = 0;
        }
        else if(data.status == 302){
          this.router.navigate(['/auth/resetpassword/'+data["processToken"]]);
        }
       },
       (error)=> {
         this.hideLoader();
         this.loginClicks = 0;
          //console.log("Error: " + error);
          if(error.status === 401){
            this.errorMsg = error.error;
            // this.invalidUserMsg = true;
          }
          else if(error.status == 404  || error.status == 403 || error.status == 500){
            this.errorMsg = error.error;
          }
          else if(error.status == 302){
            this.router.navigate(['/auth/resetpassword/'+error.error.processToken]);
          }
        })
      // }

       //--------- For Mock------//
      //  if(this.loginForm.value.username === 'testuser@atos.net' && this.loginForm.value.password === '123456'){
      //   this.invalidUserMsg = false;
      //   this.acceptCookies();
      //  }
      //  else{
      //   this.invalidUserMsg = true;
      //  }
       //------------------------//
    }
  }

  termsAndConditionPopup(){
    let cookieData=[{
      cookie: 'Local Storage',
      source: 'This Website',
      expiry: 'Session',
      purpose: 'To support the website performance'
    },
    {
      cookie: 'Account',
      source: 'This Website',
      expiry: 'Session',
      purpose: 'To support the website performance'
    },
    {
      cookie: 'AspNetCore.Session',
      source: 'This Website',
      expiry: 'Session',
      purpose: 'To support the website performance'
    },
    {
      cookie: '_4623f',
      source: 'This Website',
      expiry: 'Session',
      purpose: 'To support the website performance'
    },
    {
      cookie: '_28d01',
      source: 'This Website',
      expiry: 'Session',
      purpose: 'To support the website performance'
    },
    {
      cookie: 'cookiePolicy',
      source: 'This Website',
      expiry: 'Session',
      purpose: 'To support the website performance'
    },
  ];
    const colsList = ['cookie','source','expiry','purpose'];
    const colsName =['Cookie name', 'Source', 'Expiry', 'Purpose'];
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.maxHeight = '90vh';
    dialogConfig.data = {
      tableData: cookieData,
      colsList: colsList,
      colsName:colsName,
    }
    this.dialogRef = this.dialog.open(TermsAndConditionPopupComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
        if(res){
          this.acceptCookies();
        }
    });
  }

  openOrgRolePopup(data){
    localStorage.setItem("liveFleetTimer", (1*60).toString()); // default timer set 
    this.showOrganizationRolePopup(data.body, data.body.accountInfo, "");
  }

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  openTermsConditionsPopup(data: any, accountDetails: any, accountPreference: any){
    let objData= {
      AccountId: data.accountInfo.id,
      OrganizationId: Number(localStorage.getItem("accountOrganizationId"))
    }  
    this.translationService.getLatestTermsConditions(objData).subscribe((response)=>{

      let arrayBuffer= response[0].description;
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      let latestTCData= {
        id: 0,
        organization_Id: Number(localStorage.getItem("accountOrganizationId")),
        account_Id: data.accountInfo.id,
        terms_And_Condition_Id: response[0].id,
        version_no: response[0].versionno
      }
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = true;
      dialogConfig.autoFocus = true;
      dialogConfig.data = {
        translationData: this.translationData,
        base64File: base64File,
        latestTCData: latestTCData
      }

      this.dialogRefTerms = this.dialog.open(TermsConditionsPopupComponent, dialogConfig);
      this.dialogRefTerms.afterClosed().subscribe(res => {
        if(res.termsConditionsAgreeFlag){
          this.gotoDashBoard();
        } 
        else{
          this.loginClicks= 0;        
        } 
      });
     }, (error) => {
      this.gotoDashBoard();
     });
  }

  checkTermsAndConditions(data, accountDetails, accountPreference){
    this.showLoadingIndicator=true;
    let sessionObject: any = {
      accountId: data.accountInfo.id,
      orgId:  Number(localStorage.getItem('accountOrganizationId')),
      roleId: Number(localStorage.getItem('accountRoleId'))
    }
    this.accountService.setUserSelection(sessionObject).subscribe(() =>{
      this.translationService.getLanguageCodes().subscribe(languageCodes => {
        let objData = {
          AccountId: data.accountInfo.id,
          OrganizationId: localStorage.getItem("accountOrganizationId")
        }  
        this.translationService.checkUserAcceptedTaC(objData).subscribe(response => {
          if(!response){ 
            let langCode;
            if(accountPreference && accountPreference !== ''){
              let filterLang = languageCodes.filter(item => item.id == accountPreference["languageId"]);
              langCode = filterLang[0].code;
            } else {
              langCode ='EN-GB';
            }
              let translationObj = {
                id: 0,
                code: langCode, //-- TODO: Lang code based on account 
                type: "Menu",
                name: "",
                value: "",
                filter: "",
                menuId: 0 //-- for common & user preference
              }
            this.translationService.getMenuTranslations(translationObj).subscribe( (resp) => {
              this.processTranslation(resp);
              this.openTermsConditionsPopup(data, accountDetails, accountPreference);
              });
          }
          else{
            if(this.result){
            this.gotoDashBoard();
            }
            else{
              this.dialogRefLogin.close();
            }
          }
          this.hideLoader();
        }, (error) => {
          this.hideLoader();
          this.gotoDashBoard();
        })  
      }, (error) => {
        this.hideLoader();
      });
    }, (error) => {
      this.hideLoader();
    });
  }

  result: any;
  data: any;
  gotoDashBoard(){
    //  if (this.loginDialogForm.valid) {
      localStorage.setItem('accountOrganizationId', this.result.organization);
      localStorage.setItem('accountRoleId', this.result.role);
      let orgName = this.data.organization.filter(item => parseInt(item.id) === parseInt(this.result.organization));
      if(orgName.length > 0){
        localStorage.setItem("organizationName", orgName[0].name);
      }
      let loginDetailsObj: any = {
        organization: this.data.organization,
        role: this.data.role,
        accountDetail: this.data.accountDetail,
        accountPreference: this.data.accountPreference
      }
      // let sessionObject: any = {
      //   accountId:  this.data.accountDetail.id,
      //   orgId: this.result.organization,
      //   roleId: this.result.role
      // }
      // this.accountService.setUserSelection(sessionObject).subscribe((data) =>{
      // });
      localStorage.setItem("accountInfo", JSON.stringify(loginDetailsObj));
      this.dataInterchangeService.getDataInterface(true);
      this.dataInterchangeService.getOrgRoleInterface(this.data);
      // this.router.navigate(['/dashboard']);
    // }
  }

  public onResetPassword(values: object): void {
    values['emailId'] = values['emailId'].toLowerCase(); 
    this.resetPwdOnedayFlag = false;
    //console.log("values:: ", values)
    if (this.forgotPasswordForm.valid) {
      this.accountService.resetPasswordInitiate(values).subscribe((data:any) => {
        this.forgotPwdFlag = false;
        this.resetPwdFlag = true;
      },(error)=> {
        this.forgotPwdFlag = false;
        this.resetPwdFlag = true;
        if(error.status === 409){
          this.resetPwdOnedayFlag = true;
          this.resetPwdOnedayMsg = error.error;
        }
      })
    }
  }

  public acceptCookies(){
    this.cookiesFlag = false;
    this.cookieService.set('cookiePolicy', 'true', 365);
  }

  public showOrganizationRolePopup(data: any, accountDetails: any, accountPreference: any) {
    if(data.accountInfo.id){
      data.accountId = data.accountInfo.id;
    }
    else{
      data.accountId = 0;
    }
    let organization: Organization[] = data.accountOrganization;
    if(data.accountRole == null){
      data.accountRole = [];
    }
    let role: Role[] = data.accountRole;
    //let accountPreference: any = data.accountPreference ? data.accountPreference : '';

    if(data.accountOrganization.length > 1 || data.accountRole.length > 1){ //-- show popup
      const options = {
        title: `Welcome to DAF Connect ${accountDetails.salutation} ${accountDetails.firstName} ${accountDetails.lastName}`,
        cancelText: 'Cancel',
        confirmText: 'Confirm',
        organization: organization,
        role: role
      };
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = true;
      dialogConfig.autoFocus = true;
      dialogConfig.data = {
        title: options.title,
        cancelText: options.cancelText,
        confirmText: options.confirmText,
        organization: options.organization,
        role: options.role,
        accountDetail: accountDetails,
        accountPreference: accountPreference
      }
      this.data = dialogConfig.data;
      this.dialogRefLogin = this.dialog.open(LoginDialogComponent, dialogConfig);
      // this.dialogRefLogin.afterClosed().subscribe(res => {
      //   this.loginClicks = 0;
      // });
     
      this.dialogRefLogin.disableClose = true;//disable default close operation
      this.dialogRefLogin.beforeClosed().subscribe(result => {
        this.result = result;
        if(result){
        this.checkTermsAndConditions(data, accountDetails, accountPreference);}
        this.dialogRefLogin.close();
      });
    }
    else{ //-- skip popup
      this.data = {
        organization: organization,
        role: role,
        accountDetail: accountDetails,
        accountPreference: accountPreference
      }
      
      this.result ={   
        organization: organization[0].id,   
        role: role[0].id,
        accountDetail: accountDetails,
        accountPreference: accountPreference
      }
      this.checkTermsAndConditions(data, accountDetails, accountPreference);
    }
  }

  onForgetPassword() {
    this.errorMsg = '';
    this.forgotPwdFlag = true;
    this.invalidUserMsg= false;
  }

  onBackToLogin() {
    this.forgotPasswordForm.controls.emailId.setValue("");
    this.forgotPwdFlag = false;
    this.resetPwdFlag = false;
  }

  onCancel(){
    this.maintenancePopupFlag = false;
  }

  hideLoader(){
    this.showLoadingIndicator = false;
  }

}

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
  translationData: any;

  constructor(private cookieService: CookieService, public fb: FormBuilder, public router: Router, public authService: AuthService, private dialogService: ConfirmDialogService, private dialog: MatDialog, private accountService: AccountService, private dataInterchangeService: DataInterchangeService, private translationService: TranslationService) {
    this.loginForm = this.fb.group({
      // 'username': [null, Validators.compose([Validators.required, Validators.email])],
      // 'password': [null, Validators.compose([Validators.required, Validators.minLength(6)])]
      'username': [null, Validators.compose([Validators.required])],
      'password': [null, Validators.compose([Validators.required])]
    });
    this.forgotPasswordForm = this.fb.group({
      'emailId': [null, Validators.compose([Validators.required, Validators.email])]
    });

    this.cookiesFlag = this.cookieService.get('cookiePolicy') ? false : true;
  }

  ngOnInit(): void {
  }


  public onLogin(values: Object) {
    this.errorMsg= '';
    this.invalidUserMsg = false;
    if (this.loginForm.valid) {
      //console.log("values:: ", values)
      if(this.loginClicks == 0){
        this.loginClicks = 1;
       this.authService.signIn(this.loginForm.value).subscribe((data:any) => {
        
         //console.log("data:: ", data)
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
            }

            let loginObj = {
              id: data.body.accountInfo.id,
              organizationId: 0,
              email: "",
              accountIds: "",
              name: "",
              accountGroupId: 0,
              dataBody: data.body
            }
            
              this.accountService.getAccount(loginObj).subscribe(getAccresp => {
                if(getAccresp[0].preferenceId != 0){
                  this.accountService.getAccountPreference(getAccresp[0].preferenceId).subscribe(accPref => {
                      this.translationService.getLanguageCodes().subscribe(languageCodes => {
                      let objData = {
                        AccountId: data.body.accountInfo.id,
                        OrganizationId: data.body.accountOrganization[0].id
                      }  
                      this.translationService.checkUserAcceptedTaC(objData).subscribe(response => {
                        if(!response){
                          let filterLang = languageCodes.filter(item => item.id == accPref["languageId"]);
                          let translationObj = {
                            id: 0,
                            code: filterLang[0].code, //-- TODO: Lang code based on account 
                            type: "Menu",
                            name: "",
                            value: "",
                            filter: "",
                            menuId: 0 //-- for common & user preference
                          }
                          this.translationService.getMenuTranslations(translationObj).subscribe( (resp) => {
                            this.processTranslation(resp);
                            this.openTermsConditionsPopup(data.body, getAccresp[0], accPref);
                          });
                        }
                        else{
                          this.showOrganizationRolePopup(data.body, getAccresp[0], accPref);
                        }
                      }, (error) => {
                        this.showOrganizationRolePopup(data.body, getAccresp[0], accPref);
                      })  
                    });
                  })
                }
                else{
                  let objData = {
                    AccountId: data.body.accountInfo.id,
                    OrganizationId: data.body.accountOrganization[0].id
                  }  
                  this.translationService.checkUserAcceptedTaC(objData).subscribe(response => {
                    if(!response){
                      let translationObj = {
                        id: 0,
                        code: "EN-GB", //-- TODO: Lang code based on account 
                        type: "Menu",
                        name: "",
                        value: "",
                        filter: "",
                        menuId: 0 //-- for common & user preference
                      }
                      this.translationService.getMenuTranslations(translationObj).subscribe( (resp) => {
                        this.processTranslation(resp);
                        this.openTermsConditionsPopup(data.body, getAccresp[0], "");
                      });
                    }
                    else{
                      this.showOrganizationRolePopup(data.body, getAccresp[0], "");
                    }
                  }, (error) => {
                    this.showOrganizationRolePopup(data.body, getAccresp[0], "");
                  })  
                } 
              }, (error) => {
                this.loginClicks = 0;
                this.invalidUserMsg= true;
              });

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
         this.loginClicks = 0;
          console.log("Error: " + error);
          if(error.status == 404  || error.status == 403){
            this.errorMsg= error.error;
          }
          else if(error.status === 401){
            this.invalidUserMsg = true;
            this.loginClicks = 0;
          }
          else if(error.status == 302){
            this.router.navigate(['/auth/resetpassword/'+error.error.processToken]);
          }
          else if(error.status == 500)
            this.invalidUserMsg = true;
          //this.cookiesFlag = false;
        })
      }

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

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  openTermsConditionsPopup(data: any, accountDetails: any, accountPreference: any){
    let objData= {
      AccountId: data.accountInfo.id,
      OrganizationId: data.accountOrganization[0].id
    }  
    this.translationService.getLatestTermsConditions(objData).subscribe((response)=>{

      let arrayBuffer= response[0].description;
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      let latestTCData= {
        id: 0,
        organization_Id: data.accountOrganization[0].id,
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
          this.showOrganizationRolePopup(data, accountDetails, accountPreference);
        } 
        else{
          this.loginClicks= 0;        
        } 
      });
     }, (error) => {
      this.showOrganizationRolePopup(data, accountDetails, accountPreference);
     });

     
  }


  public onResetPassword(values: object): void {
    console.log("values:: ", values)
    if (this.forgotPasswordForm.valid) {
      this.accountService.resetPasswordInitiate(values).subscribe(data => {
        if(data){
          this.forgotPwdFlag = false;
          this.resetPwdFlag = true;
        }
      },(error)=> {

      })
    }
  }

  public acceptCookies(){
    this.cookiesFlag = false;
    this.cookieService.set('cookiePolicy', 'true');
  }

  public showOrganizationRolePopup(data: any, accountDetails: any, accountPreference: any) {
    if(data.accountInfo.id){
      data.accountId = data.accountInfo.id;
    }
    else{
      data.accountId = 0;
    }

  //---Test scenario -----//
  //  org  role  action
  //  0     0    popup skip - dashboard with no data
  //  1     0    popup skip - dashboard with org data only
  //  1     1    popup skip - dashboard with both role & org data
  //  2     0    popup show w/o role - dashboard with org data only
  //  2	    1    popup show with both data - dashboard with both role & org data
  //  1     2    popup show with both data - dashboard with both role & org data

  //--- Test data-------------
    //data.accountOrganization.push({id: 1, name: 'Org01'});
    //data.accountRole.push({id: 1, name: 'Role01'});
    //data.accountOrganization = [];
    //data.accountRole = [];
  //----------------------

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
      this.dialogRefLogin = this.dialog.open(LoginDialogComponent, dialogConfig);
      this.dialogRefLogin.afterClosed().subscribe(res => {
        this.loginClicks = 0;
      });
    }
    else{ //-- skip popup
      let loginDetailsObj: any = {
        organization: organization,
        role: role,
        accountDetail: accountDetails,
        accountPreference: accountPreference
      }
      localStorage.setItem("accountInfo", JSON.stringify(loginDetailsObj));
      this.dataInterchangeService.getDataInterface(true);
      this.router.navigate(['/dashboard']);
    }
  }

  onForgetPassword() {
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

}

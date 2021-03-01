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
  cookiesFlag: boolean = true;
  forgotPwdFlag: boolean = false;
  dialogRefLogin: MatDialogRef<LoginDialogComponent>;
  maintenancePopupFlag: boolean = false;
  loginClicks = 0;

  constructor(private cookieService: CookieService, public fb: FormBuilder, public router: Router, public authService: AuthService, private dialogService: ConfirmDialogService, private dialog: MatDialog, private accountService: AccountService, private dataInterchangeService: DataInterchangeService) {
    this.loginForm = this.fb.group({
      // 'username': [null, Validators.compose([Validators.required, Validators.email])],
      // 'password': [null, Validators.compose([Validators.required, Validators.minLength(6)])]
      'username': [null, Validators.compose([Validators.required])],
      'password': [null, Validators.compose([Validators.required])]
    });
    this.forgotPasswordForm = this.fb.group({
      'email': [null, Validators.compose([Validators.required, Validators.email])]
    });
    
    this.cookiesFlag = this.cookieService.get('cookiePolicy') ? false : true;
  }

  ngOnInit(): void {
  }

  public onLogin(values: Object) {
    
    if (this.loginForm.valid) {
      //console.log("values:: ", values)
       this.authService.signIn(this.loginForm.value).subscribe((data:any) => {
         //console.log("data:: ", data)
         if(data.status === 200){
           this.invalidUserMsg = false;
            //this.cookiesFlag = true;

            let loginObj = {
              "id": data.body.accountId,
              "organizationId": 0,
              "email": "",
              "accountIds": "",
              "name": "",
              "accountGroupId": 0
            }
            if(this.loginClicks == 0){
              this.accountService.getAccount(loginObj).subscribe(resp => {
                this.showOrganizationRolePopup(data.body, resp[0]);
              }, (error) => {});
            }
            this.loginClicks = 1;
         }
         else if(data.status === 401){
          this.invalidUserMsg = true;
        }
       },
       (error)=> {
          console.log("Error: " + error);
          this.invalidUserMsg = true;
          //this.cookiesFlag = false;
        }) 

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

  public onResetPassword(values: object): void {
    console.log("values:: ", values)
    if (this.forgotPasswordForm.valid) {

    }
  }

  public acceptCookies(){
    this.cookiesFlag = false;
    this.cookieService.set('cookiePolicy', 'true');
  }

  public showOrganizationRolePopup(data: any, accountDetails: any) {
    if(data.accountId){
      data.accountId = data.accountId;
    }
    else{
      data.accountId = 0;
    }
    localStorage.setItem('accountId', data.accountId);
    
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
    let role: Role[] = data.accountRole;
    let accountPreference: any = data.accountPreference ? data.accountPreference : '';

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
      if(data.accountOrganization.length > 0){
        localStorage.setItem('accountOrganizationId', data.accountOrganization[0].id);
        localStorage.setItem("organizationName", data.accountOrganization[0].name);
      }

      if(data.accountRole.length > 0){
        localStorage.setItem('accountRoleId', data.accountRole[0].id);
      }

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
    //this.forgotPwdFlag = true;
  }

  onBackToLogin() {
    this.forgotPwdFlag = false;
  }

  onCancel(){
    this.maintenancePopupFlag = false;
  }

}

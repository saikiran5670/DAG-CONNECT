import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { LoginDialogComponent } from './login-dialog/login-dialog.component';

export interface Organization {
  value: string;
  viewValue: string;
}
export interface Role {
  value: string;
  viewValue: string;
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

  constructor(public fb: FormBuilder, public router: Router, public authService: AuthService, private dialogService: ConfirmDialogService, private dialog: MatDialog) {
    this.loginForm = this.fb.group({
      'username': [null, Validators.compose([Validators.required])],
      'password': [null, Validators.compose([Validators.required, Validators.minLength(6)])]
    });
    this.forgotPasswordForm = this.fb.group({
      'email': [null, Validators.compose([Validators.required])]
    });
  }

  ngOnInit(): void {
  }

  public onLogin(values: Object) {
    if (this.loginForm.valid) {
      //console.log("values:: ", values)
      /* this.authService.signIn(this.loginForm.value).subscribe((data:any) => {
         console.log("data:: ", data)
         if(data.status === 200){
           this.invalidUserMsg = false;
            //this.cookiesFlag = true;
            this.showOrganizationPopup();
         }
       },
       (error)=> {
         console.log("Error: " + error);
       }) */

       //--------- For Mock------//
       if(this.loginForm.value.username === 'testuser@atos.net' && this.loginForm.value.password === '123456'){
        this.invalidUserMsg = false;
        this.acceptCookies();
       }
       else{
        this.invalidUserMsg = true;
       }
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
  }

  public showOrganizationPopup() {
    let organization: Organization[] = [
      { value: 'daf-0', viewValue: 'DAF Connect' },
      { value: 'conti-1', viewValue: 'Conti' },
      { value: 'daf-2', viewValue: 'DAF CT 2.0' }
    ];

    let role: Role[] = [
      { value: 'fleetadmin-0', viewValue: 'Fleet Admin' },
      { value: 'fleetmanager-1', viewValue: 'Fleet Manager' },
      { value: 'user-2', viewValue: 'Fleet User' }
    ];

    const options = {
      title: 'Welcome to DAF Connect Mr. John Rutherford',
      cancelText: 'Cancel',
      confirmText: 'Confirm',
      organization: organization,
      role: role
    };
    this.loginPopupOpen(options);
  }

  public loginPopupOpen(options: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      title: options.title,
      cancelText: options.cancelText,
      confirmText: options.confirmText,
      organization: options.organization,
      role: options.role
    }
    this.dialogRefLogin = this.dialog.open(LoginDialogComponent, dialogConfig);
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

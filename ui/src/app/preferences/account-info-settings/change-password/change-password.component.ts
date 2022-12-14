import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CustomValidators } from '../../../shared/custom.validators';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.less']
})

export class ChangePasswordComponent implements OnInit {
  public changePasswordForm : FormGroup
  password: string;
  minCharacterTxt: any;
  errorMsg: string = '';
  errorCode: number = 0;
  curPwdHide: boolean = true;
  newPwdHide: boolean = true;
  confirmPwdHide: boolean = true;
  isCurrentPwdMismatch: boolean = false;
  isPwdInValid: boolean = false;
  showLoadingIndicator: boolean = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    accountInfo: any
  }, private mdDialogRef: MatDialogRef<ChangePasswordComponent>, public router: Router, public fb: FormBuilder, private accountService: AccountService) {
    this.changePasswordForm = this.fb.group({
      'currentPassword': [null, [Validators.required]],
      'newPassword': [null, [Validators.required, Validators.minLength(10), Validators.maxLength(256)]],
      'confirmPassword': [null, [Validators.required]],
    },{
      validator : [
        CustomValidators.mustMatchNewAndConfirmPassword('newPassword', 'confirmPassword'), CustomValidators.validatePassword('newPassword')
      ]
    });
    if(data.translationData.lblcharactersmin)
      this.minCharacterTxt = data.translationData.lblcharactersmin.replace('$', '10');
    else
      this.minCharacterTxt =  ("'$' characters min").replace('$', '10');
  }

  ngOnInit() { }

  public cancel() {
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  public onChangePassword(formValue) {
    this.isCurrentPwdMismatch = false;
    this.isPwdInValid = false;
    if (this.changePasswordForm.valid) {
      let objData: any = {
        emailId: this.data.accountInfo.emailId,
        password: formValue.newPassword,
        oldPassword: formValue.currentPassword
      }
      this.showLoadingIndicator = true;
      this.accountService.changeAccountPassword(objData).subscribe((data)=>{
        if(data){
          this.close(false);  
          this.mdDialogRef.close({editText : 'Password'}); 
        }
        this.showLoadingIndicator=false;
      },(error)=> {
        this.showLoadingIndicator=false;
        this.errorCode = error.status;
        if(error.status == 400){
          if(error.error.indexOf("Entered password incorrect") !== -1)
            this.isCurrentPwdMismatch = true;
          else
            this.isPwdInValid = true;
          this.errorMsg= "Password must not be equal to any of last 6 passwords."
        }
        else if(error.status == 403){
          this.errorMsg= "Your password needs to be at least one day old before it can be changed."
        }
        else if(error.status == 404){
          this.errorMsg= "Wrong email id."
        }
        else if(error.status == 500){
          this.errorMsg= "Something went wrong! Please try again."
        }
      });
    }
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }

}
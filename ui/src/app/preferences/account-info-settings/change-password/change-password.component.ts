import { Component, EventEmitter, HostListener, Inject, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { EmployeeService } from 'src/app/services/employee.service';
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

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    accountInfo: any
  },private mdDialogRef: MatDialogRef<ChangePasswordComponent>, public router: Router, public fb: FormBuilder, private userService: EmployeeService, private accountService: AccountService) {
    this.changePasswordForm = this.fb.group({
     // 'currentPassword': [null, Validators.compose([Validators.required])],
      'newPassword': [null, Validators.compose([Validators.required, Validators.minLength(8)])],
      'confirmPassword': [null, Validators.compose([Validators.required])],
    },{
      validator : [CustomValidators.mustMatchNewAndConfirmPassword('newPassword', 'confirmPassword'), CustomValidators.validatePassword('newPassword')//, CustomValidators.checkForCurrentPassword('currentPassword', this.password, this.userService)
    ]
    });
    
    if(data.translationData.lblcharactersmin)
      this.minCharacterTxt = data.translationData.lblcharactersmin.replace('$', '8');
    else
      this.minCharacterTxt =  ("'$' characters min").replace('$', '8');
  }

  ngOnInit(): void {
    
  }

  public cancel() {
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  public onChangePassword(formValue) {
    if (this.changePasswordForm.valid) {
      let selectedValues = formValue;
      let objData: any = {
        emailId: this.data.accountInfo.emailId,
        password: formValue.newPassword
      }
      this.accountService.changeAccountPassword(objData).subscribe(()=>{
        this.close(false);  
        this.mdDialogRef.close({editText : 'Password'}); 
      });
    }
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }
}

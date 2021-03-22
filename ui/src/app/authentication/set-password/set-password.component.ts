import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-set-password',
  templateUrl: './set-password.component.html',
  styleUrls: ['./set-password.component.less']
})
export class SetPasswordComponent implements OnInit {

  public setPasswordForm : FormGroup
  password: string;
  minCharacterTxt: any;
  translationData: any = [];  
  buttonName: string;
  currentRoute: string= '';

  constructor(public router: Router, public fb: FormBuilder, private accountService: AccountService) {
    this.setPasswordForm = this.fb.group({
      'newPassword': [null, Validators.compose([Validators.required, Validators.minLength(8)])],
      'confirmPassword': [null, Validators.compose([Validators.required])],
    },{
      validator : [
        CustomValidators.mustMatchNewAndConfirmPassword('newPassword', 'confirmPassword'), CustomValidators.validatePassword('newPassword')
      ]
    });

    this.minCharacterTxt =  ("'$' characters min").replace('$', '8');
  }

  ngOnInit() { 
    this.currentRoute = this.router.url;
    if(this.currentRoute.includes("createpassword")){
      this.buttonName = "Create";
    }
    else if(this.currentRoute.includes("resetpassword")){
      this.buttonName = "Reset";
    }
  }

  public onCreatePassword(formValue) {
    if (this.setPasswordForm.valid) {
      let routeArray = this.currentRoute.split("/");
      let objData: any = {
        processToken: routeArray.pop(),
        password: formValue.newPassword
      }
      if(this.buttonName == "Create"){
        this.accountService.createpassword(objData).subscribe(()=>{
          
        }, (error) => {

        });
      }
      else if(this.buttonName == "Reset"){
        this.accountService.resetPassword(objData).subscribe(()=>{
          
        }, (error) => {
          
        });
      }
    }
  }

  public cancel() {
    this.router.navigate(['/auth/login']);
  }
  

}

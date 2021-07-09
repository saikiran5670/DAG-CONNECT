import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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
  token: string= '';
  isChangePwdSuccess: boolean = false;
  isResetPwdInvalidate: boolean= false;
  errorMsg: string= '';
  errorCode: number= 0;
  isLinkActive: boolean= true;

  constructor(public router: Router, private route: ActivatedRoute, public fb: FormBuilder, private accountService: AccountService) {
    this.setPasswordForm = this.fb.group({
      'newPassword': [null, Validators.compose([Validators.required, Validators.minLength(10), Validators.maxLength(256)])],
      'confirmPassword': [null, Validators.compose([Validators.required])],
    },{
      validator : [
        CustomValidators.mustMatchNewAndConfirmPassword('newPassword', 'confirmPassword'), CustomValidators.validatePassword('newPassword')
      ]
    });

    this.minCharacterTxt =  ("'$' characters min").replace('$', '10');
  }

  ngOnInit() { 
    this.token=  this.route.snapshot.paramMap.get('token');
    this.accountService.getResetPasswordTokenStatus(this.token).subscribe(data => {
      if(data){
        this.getButtonName();
      }
    }, (error)=> {
      if(error.status == 404){
        this.isLinkActive= false;
      }
      else if(error.status == 200){
        this.getButtonName();
      }
    })
  }

  getButtonName(){
    this.isLinkActive= true;
      this.currentRoute = this.router.url;
      if(this.currentRoute.includes("/createpassword/")){
        this.buttonName = "Create";
      }
      else if(this.currentRoute.includes("/resetpassword/")){
        this.buttonName = "Reset";
      }
      else if(this.currentRoute.includes("/resetpasswordinvalidate/")){
        let objData= {
          resetToken: this.token
        }
        this.accountService.resetPasswordInvalidate(objData).subscribe(data => {
          this.isResetPwdInvalidate= true;
        },(error)=> {
          this.isResetPwdInvalidate= true;
          this.errorMsg= 'InvalidateFailed'
        })
      }
  }

  public onCreatePassword(formValue) {
    if (this.setPasswordForm.valid) {
      let objData: any = {
        processToken: this.token,
        password: formValue.newPassword
      }
      if(this.buttonName == "Create"){
        this.accountService.createpassword(objData).subscribe((data)=>{
          if(data){
            this.isChangePwdSuccess= true;
          }
        }, (error) => {
            this.errorCode = error.status;
            if(error.status == 400 && error.error.includes("last 6 passwords")){
              this.errorMsg= "Password must not be equal to any of last 6 passwords."
            }
            else if(error.status == 404){
              this.errorMsg= "Activation link is expired or invalidated. Please try again"
            }
            else{
              this.errorMsg= "Something went wrong! Please try again."
            }
        });
      }
      else if(this.buttonName == "Reset"){
        this.accountService.resetPassword(objData).subscribe((data)=>{
          if(data){
            this.isChangePwdSuccess= true;
          }
        }, (error) => {
            this.errorCode = error.status;
            if(error.status == 400 && error.error.includes("last 6 passwords")){
              this.errorMsg= "Password must not be equal to any of last 6 passwords."
            }
            else if(error.status == 404){
              this.errorMsg= "Activation link is expired or invalidated. Please try again!"
            }
            else{
              this.errorMsg= "Something went wrong! Please try again."
            }
        });
      }
    }
  }

  public cancel() {
    this.isChangePwdSuccess= false;
    this.isResetPwdInvalidate= false;
    this.router.navigate(['/auth/login']);
  }
  

}

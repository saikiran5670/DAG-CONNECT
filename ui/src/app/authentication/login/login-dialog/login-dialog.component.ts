import { Component, HostListener, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';

@Component({
  selector: 'app-login-dialog',
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.less']
})
export class LoginDialogComponent {
  public loginDialogForm: FormGroup;
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    title: string,
    cancelText: string,
    confirmText: string,
    organization: any,
    role: any,
    accountDetail: any
  }, private mdDialogRef: MatDialogRef<LoginDialogComponent>, public router: Router, public fb: FormBuilder, private dataInterchangeService: DataInterchangeService) {
    this.loginDialogForm = this.fb.group({
      'organization': [null, Validators.compose([Validators.required])],
      'role': [null, Validators.compose([Validators.required])]
    });
     this.setDropdownValues();
  }

  setDropdownValues(){
    this.loginDialogForm.get('organization').setValue(this.data.organization[0].id);
    if(this.data.role[0]!=null ){
      this.loginDialogForm.get('role').setValue(this.data.role[0].id);
    }
    
  }

  public cancel() {
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  public confirm(formValue) {
    if (this.loginDialogForm.valid) {
      let selectedValues = formValue;
      localStorage.setItem('accountOrganizationId', this.loginDialogForm.controls.organization.value);
      localStorage.setItem('accountRoleId', this.loginDialogForm.controls.role.value);
      let orgName = this.data.organization.filter(item => item.id === this.loginDialogForm.controls.organization.value);
      localStorage.setItem("organizationName", orgName[0].name);
      let loginDetailsObj: any = {
        organization : this.data.organization,
        role : this.data.role,
        accountDetail : this.data.accountDetail
      }
      localStorage.setItem("accountInfo", JSON.stringify(loginDetailsObj));
      this.close(false);
      this.dataInterchangeService.getDataInterface(true);
      this.dataInterchangeService.getOrgRoleInterface(this.data);
      this.router.navigate(['/dashboard']);
    }
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }

}

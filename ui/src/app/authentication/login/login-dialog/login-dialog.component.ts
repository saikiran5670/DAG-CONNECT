import { Component, HostListener, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';

@Component({
  selector: 'app-login-dialog',
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.less']
})
export class LoginDialogComponent {
  public loginDialogForm: FormGroup;
  selectedRoles: any = [];
  constructor( private accountService: AccountService, 
    @Inject(MAT_DIALOG_DATA) public data: {
    title: string,
    cancelText: string,
    confirmText: string,
    organization: any,
    role: any,
    accountDetail: any,
    accountPreference: any
  }, private mdDialogRef: MatDialogRef<LoginDialogComponent>, public router: Router, public fb: FormBuilder, private dataInterchangeService: DataInterchangeService) {
    this.loginDialogForm = this.fb.group({
      'organization': [],
      'role': []
    });
     this.setDropdownValues();
  }

  setDropdownValues(){
    if(this.data.organization.length > 0){
      this.loginDialogForm.get('organization').setValue(this.data.organization[0].id);
      this.filterOrgRoles(this.data.organization[0].id); //-- filter roles based on org
    }
    // if(this.data.role.length > 0){
    //   this.loginDialogForm.get('role').setValue(this.data.role[0].id);
    // }
  }

  public cancel() {
    localStorage.clear(); // clear localstorage
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
      let orgName = this.data.organization.filter(item => parseInt(item.id) === parseInt(this.loginDialogForm.controls.organization.value));
      if(orgName.length > 0){
        localStorage.setItem("organizationName", orgName[0].name);
      }
      let loginDetailsObj: any = {
        organization: this.data.organization,
        role: this.data.role,
        accountDetail: this.data.accountDetail,
        accountPreference: this.data.accountPreference
      }
      let sessionObject: any = {
        accountId:  this.data.accountDetail.id,
        orgId: this.loginDialogForm.controls.organization.value,
        roleId: this.loginDialogForm.controls.role.value
      }
      this.accountService.setUserSelection(sessionObject).subscribe((data) =>{
      });
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

  orgBasedRoleSelection(event: any){
    this.filterOrgRoles(event.value); //-- pass orgId
  }

  filterOrgRoles(orgId: any){
    if(this.data.role.length > 0){ //-- (Roles > 0) 
      let filterRoles = this.data.role.filter(item => parseInt(item.organization_Id) === parseInt(orgId));
      if(filterRoles.length > 0){
        this.selectedRoles = filterRoles;
        this.loginDialogForm.get('role').setValue(this.selectedRoles[0].id);
      }
      else{
        this.selectedRoles = [];
      }
    }
  }

}

import { Component, HostListener, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
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
  selectedRoles: any = [];
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
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
  }

  public cancel() {
    localStorage.clear(); // clear localstorage
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  public confirm(formValue) {
    this.mdDialogRef.close(formValue);
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

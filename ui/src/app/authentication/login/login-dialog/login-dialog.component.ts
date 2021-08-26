import { Component, HostListener, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';
import { TranslationService } from 'src/app/services/translation.service';
import { TermsConditionsPopupComponent } from 'src/app/terms-conditions-content/terms-conditions-popup.component';

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
    accountPreference: any,
    organization_Id: any,
    account_Id: any,
    translationData: any,
    tacValue: any
  }, private mdDialogRef: MatDialogRef<LoginDialogComponent>, public router: Router, public fb: FormBuilder, private dataInterchangeService: DataInterchangeService, private translationService: TranslationService, private dialog: MatDialog) {
    this.loginDialogForm = this.fb.group({
      'organization': [],
      'role': []
    });
     this.setDropdownValues();
  }
  dialogRefTerms: MatDialogRef<TermsConditionsPopupComponent>;
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
    if(this.data.tacValue){
      this.confirmTodashboard(formValue);
    } else {
      this.openTermsConditionsPopup(formValue);
    }
    return;
  }
  
  confirmTodashboard(formValue){
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

  openTermsConditionsPopup(formValue){
    let objData= {
      AccountId: this.data.account_Id,
      OrganizationId: this.data.organization_Id
    }  
    this.translationService.getLatestTermsConditions(objData).subscribe((response)=>{

      let arrayBuffer= response[0].description;
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      let latestTCData= {
        id: 0,
        organization_Id: this.data.organization_Id,
        account_Id: this.data.account_Id,
        terms_And_Condition_Id: response[0].id,
        version_no: response[0].versionno
      }
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = true;
      dialogConfig.autoFocus = true;
      dialogConfig.data = {
        translationData: this.data.translationData,
        base64File: base64File,
        latestTCData: latestTCData
      }
      this.dialogRefTerms = this.dialog.open(TermsConditionsPopupComponent, dialogConfig);
      this.dialogRefTerms.afterClosed().subscribe(res => {
        if(res.termsConditionsAgreeFlag){
          this.confirmTodashboard(formValue);
        }
      });
     }, (error) => {
        this.confirmTodashboard(formValue);
     });     
  }
}

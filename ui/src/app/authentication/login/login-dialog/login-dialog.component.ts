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
    role: any
  }, private mdDialogRef: MatDialogRef<LoginDialogComponent>, public router: Router, public fb: FormBuilder, private dataInterchangeService: DataInterchangeService) {
    this.loginDialogForm = this.fb.group({
      'organization': [null, Validators.compose([Validators.required])],
      'role': [null, Validators.compose([Validators.required])]
    });
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
      this.close(false);
      this.dataInterchangeService.getDataInterface(true);
      this.router.navigate(['/dashboard']);
    }
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }

}

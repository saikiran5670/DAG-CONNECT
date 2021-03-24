import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-terms-conditions-popup',
  templateUrl: './terms-conditions-popup.component.html',
  styleUrls: ['./terms-conditions-popup.component.less']
})
export class TermsConditionsPopupComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
  }, private mdDialogRef: MatDialogRef<TermsConditionsPopupComponent>, public router: Router) {
   
  }

  ngOnInit(): void {
  }

  public cancel() {
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  onClickIAgree(){
    //Send terms and conditions accept flag to backend.
    //this.mdDialogRef.close({termsConditionsAgreeFlag : true}); 
    this.close(false);
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }

}

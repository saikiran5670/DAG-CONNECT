import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { TranslationService } from '../services/translation.service';

@Component({
  selector: 'app-terms-conditions-popup',
  templateUrl: './terms-conditions-popup.component.html',
  styleUrls: ['./terms-conditions-popup.component.less']
})
export class TermsConditionsPopupComponent implements OnInit {
  fileURL: any;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    base64File: any,
    latestTCData: any
  }, private mdDialogRef: MatDialogRef<TermsConditionsPopupComponent>, public router: Router, private domSanitizer: DomSanitizer, private translationService: TranslationService) {
   
  }

  ngOnInit(): void {
    this.fileURL = this.domSanitizer.bypassSecurityTrustResourceUrl("data:application/pdf;base64,"+this.data.base64File);
  }

  public cancel() {
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  onClickIAgree(){
    this.translationService.addUserAcceptedTermsConditions(this.data.latestTCData).subscribe(data => {

    }, (error) => {

    })
    this.mdDialogRef.close({termsConditionsAgreeFlag : true}); 
  }

  disagree(){
    this.mdDialogRef.close({termsConditionsAgreeFlag : false})
  }

}

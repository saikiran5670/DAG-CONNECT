import { Component, OnInit, HostListener, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-consent-single-opt',
  templateUrl: './consent-single-opt.component.html',
  styleUrls: ['./consent-single-opt.component.less']
})

export class ConsentSingleOptComponent implements OnInit {
  showOptOutMsg: boolean = false;
  consentMsg: any;
  consentMsgExtra: any;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    driverData: any
  }, private mdDialogRef: MatDialogRef<ConsentSingleOptComponent>) {
    this.showOptOutMsg = data.driverData.consentStatus === 'Opt-In' ? false : true;
    this.getConsentMsg(data.driverData.consentStatus); 
    this.getConsentExtraMsg(data.driverData.consentStatus);
  }

  ngOnInit() { }

  getConsentMsg(optValue: any){
    if(this.data.translationData.lblOptInOutChangeMessage)
      this.consentMsg = this.data.translationData.lblOptInOutChangeMessage.replace('$', optValue);
    else
      this.consentMsg = ("You are currently in '$' mode. This means no personal data from your driver(s) such as the driver ID are visible in the DAF CONNECT portal.").replace('$', optValue);
  }

  getConsentExtraMsg(optValue: any){
    if(this.data.translationData.lblConsentExtraMessage)  
      this.consentMsgExtra = this.data.translationData.lblConsentExtraMessage.replace('$', optValue);
    else
      this.consentMsgExtra = ("By selecting and confirming '$' mode (i.e. by checking the opt-in checkbox) personal data such as the driver ID from your driver(s) will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.").replace('$', optValue);;
  }

  public close(value: any) {
    this.mdDialogRef.close(value);
  }

  public onConfirm() {
    this.close(true);
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }

  onCancel(){
    this.close(false);
  }

  onChange(event: any){
    if(event.value === 'Opt-Out'){
      this.showOptOutMsg = true;
      this.getConsentMsg(event.value);
      this.getConsentExtraMsg(event.value);
    }
    else{
      this.showOptOutMsg = false;
      this.getConsentMsg(event.value);
      this.getConsentExtraMsg(event.value);
    }
  }

}
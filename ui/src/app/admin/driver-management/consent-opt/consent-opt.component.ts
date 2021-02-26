import { Component, OnInit, HostListener, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-consent-opt',
  templateUrl: './consent-opt.component.html',
  styleUrls: ['./consent-opt.component.less']
})
export class ConsentOptComponent implements OnInit {

  showMsgFlag: boolean = false;
  consentMsg: any;
  consentMsgExtra: any;
  checkedFlag: boolean = false;
  totalDrivers: number = 0;
  organizationName: any = '';
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    optValue: string,
    translationData: any,
    driverData: any
  }, private mdDialogRef: MatDialogRef<ConsentOptComponent>) {
    this.organizationName = localStorage.getItem('organizationName');
    this.showMsgFlag = data.optValue === 'Opt-In' ? false : true;
    this.checkedFlag = data.optValue === 'Opt-In' ? true : false;
    this.getConsentMsg(data.optValue); 
    this.getConsentExtraMsg(data.optValue);  
    this.getDriverCount();
  }

  getDriverCount(){
    if(this.checkedFlag){
      this.totalDrivers = this.data.driverData.filter((item: any) => item.consentStatus == 'Opt-In').length;
    }
    else{
      this.totalDrivers = this.data.driverData.filter((item: any) => item.consentStatus == 'Opt-Out').length;
    }
  }

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

  ngOnInit(): void {
  }

  public cancel() {
    this.close(false);
  }
  public close(value) {
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

  onChange(event)
  {
    if(event.value === 'Opt-Out'){
      this.showMsgFlag = true;
      this.getConsentMsg(event.value);
      this.getConsentExtraMsg(event.value);
    }
    else{
      this.showMsgFlag = false;
      this.getConsentMsg(event.value);
      this.getConsentExtraMsg(event.value);
    }
  }

  onCheckboxChange(event: any)
  {
    if(event.source.value === 'Opt-Out'){
      this.checkedFlag = !this.checkedFlag;
      this.showMsgFlag = true;
      this.getConsentMsg(event.source.value);
      this.getConsentExtraMsg(event.source.value);
    }
    else{
      this.showMsgFlag = false;
      this.checkedFlag = !this.checkedFlag;
      this.getConsentMsg(event.source.value);
      this.getConsentExtraMsg(event.source.value);
    }
    this.getDriverCount();
  }

}

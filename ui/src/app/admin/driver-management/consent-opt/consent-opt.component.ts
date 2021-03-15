import { Component, OnInit, HostListener, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-consent-opt',
  templateUrl: './consent-opt.component.html',
  styleUrls: ['./consent-opt.component.less']
})

export class ConsentOptComponent implements OnInit {
  showOptOutMsg: boolean = false;
  consentMsg: any;
  consentMsgExtra: any;
  organizationName: any;
  totalDrivers: number = 0;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    driverData: any,
    actionType: any,
    consentType: any
  }, private mdDialogRef: MatDialogRef<ConsentOptComponent>) {
    this.organizationName = localStorage.getItem('organizationName');
    this.totalDrivers = data.driverData.length;
    // if(data.actionType){ //---  True -> All & False -> single 
    //   //this.getDriverCount();
    // }else{
    //   //data.consentType = (data.driverData.status) ? 'Inherit' : data.consentType;
    // }
    this.showOptOutMsg = data.consentType === 'U' ? true : false;
    this.getConsentMsg(data.consentType); 
    //this.getConsentExtraMsg(data.consentType);
  }

  ngOnInit() { }

  getConsentMsg(optValue: any){
    let optVal: any = '';
    switch(optValue){
      case 'I' : {
        optVal = 'Opt-In'; 
        break;
      }
      case 'U' : {
        optVal = 'Opt-Out'; 
        break;
      }
      case 'H' : {
        optVal = 'Inherit'; 
        break;
      }
    }

    if(this.data.translationData.lblOptInOutChangeMessage){
      this.consentMsg = this.data.translationData.lblOptInOutChangeMessage.replace('$', optVal);
    }
    else{
      this.consentMsg = ("You are currently in '$' mode. This means no personal data from your driver(s) such as the driver ID are visible in the DAF CONNECT portal.").replace('$', optVal);
    }

    if(this.data.translationData.lblConsentExtraMessage){
      this.consentMsgExtra = this.data.translationData.lblConsentExtraMessage.replace('$', optVal);
    }
    else{
      this.consentMsgExtra = ("By selecting and confirming '$' mode (i.e. by checking the opt-in checkbox) personal data such as the driver ID from your driver(s) will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.").replace('$', optVal);
    }
  }

  // getDriverCount(){
  //   if(this.showOptOutMsg){ //-- opt-out mode
  //     this.totalDrivers = this.data.driverData.filter((item: any) => item.status == 'U').length;
  //   }
  //   else{ //-- opt-in mode
  //     this.totalDrivers = this.data.driverData.filter((item: any) => item.status == 'I').length;
  //   }
  // }

  // getConsentExtraMsg(optValue: any){
  //   if(this.data.translationData.lblConsentExtraMessage)  
  //     this.consentMsgExtra = this.data.translationData.lblConsentExtraMessage.replace('$', optValue);
  //   else
  //     this.consentMsgExtra = ("By selecting and confirming '$' mode (i.e. by checking the opt-in checkbox) personal data such as the driver ID from your driver(s) will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.").replace('$', optValue);
  // }

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
    this.data.consentType = event.value;
    if(event.value === 'U'){
      this.showOptOutMsg = true;
      this.getConsentMsg(event.value);
      //this.getConsentExtraMsg(event.value);
    }
    //else if(event.value === 'Opt-In'){
    else{
      this.showOptOutMsg = false;
      this.getConsentMsg(event.value);
      //this.getConsentExtraMsg(event.value);
    }
    // if(this.data.actionType){
    //   //this.getDriverCount();
    // }
  }

}
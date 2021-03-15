import { Component, OnInit, HostListener, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DriverService } from '../../../services/driver.service';

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
  closePopup: boolean = true;
  accountOrganizationId: any = 0;
  accountId: any = 0;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    driverData: any,
    actionType: any,
    consentType: any
  }, private mdDialogRef: MatDialogRef<ConsentOptComponent>, private driverService: DriverService) {
    this.organizationName = localStorage.getItem('organizationName');
    this.totalDrivers = data.driverData.length;
    this.showOptOutMsg = data.consentType === 'U' ? true : false;
    this.getConsentMsg(data.consentType); 
  }

  ngOnInit() { 
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
  }

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

  public onClose(value: any) {
    this.closePopup = false;
    this.mdDialogRef.close(value);
  }

  public onConfirm() {
    if(this.data.actionType){ //-- update All
      let objData: any = {
        orgID: this.accountOrganizationId,
        optoutoptinstatus: this.data.consentType
      };
      this.driverService.updateOptInOptOutDriver(objData).subscribe((drv: any) => {
        this.getDriverList();
      });
    }
    else{ //-- update single
      let objData: any = {
        id: this.data.driverData.id,
        organizationId: this.data.driverData.organizationId,
        driverIdExt: this.data.driverData.driverIdExt,
        email: this.data.driverData.email,
        firstName: this.data.driverData.firstName,
        lastName: this.data.driverData.lastName,
        //status: this.data.consentType,
        optIn: this.data.consentType,
        //isActive: true,
        modifiedBy: this.accountId //0
      }
      this.driverService.updateDriver(objData).subscribe((drv: any) => {
        this.getDriverList();
      });
    }
  }

  getDriverList(){
    let drvId: any = 0;
    this.driverService.getDrivers(this.accountOrganizationId, drvId).subscribe((driverList: any) => {
      this.onClose({ tableData: driverList });
    });
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.onClose(false);
  }

  onCancel(){
    this.onClose(false);
  }

  onChange(event: any){
    this.data.consentType = event.value;
    if(event.value === 'U'){
      this.showOptOutMsg = true;
      this.getConsentMsg(event.value);
    }
    else{
      this.showOptOutMsg = false;
      this.getConsentMsg(event.value);
    }
  }
  
}
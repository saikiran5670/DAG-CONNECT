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
  inheritMsgExtra: any;
  organizationName: any;
  totalDrivers: number = 0;
  closePopup: boolean = true;
  accountOrganizationId: any = 0;
  accountId: any = 0;
  orgInheritMode: any;
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    driverData: any,
    actionType: any,
    consentType: any,
    organizationData: any,
    radioSelected: any
  }, private mdDialogRef: MatDialogRef<ConsentOptComponent>, private driverService: DriverService) {
    this.organizationName = localStorage.getItem('organizationName');
    this.totalDrivers = data.driverData.length;
    this.showOptOutMsg = data.consentType === 'U' ? true : false;
    this.orgInheritMode = data.organizationData ? data.organizationData.driverOptIn : 'I'; //-- org driver dafult
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
        if(this.data.translationData.lblOptInOptOutAttemptingMsg){
          this.consentMsg = this.data.translationData.lblOptInOptOutAttemptingMsg.replace('$', optVal);
        }
        else{
          this.consentMsg = ("You are attempting to change consent to '$'").replace('$', optVal);
        }    
        if(this.data.translationData.lblOptInExtraMsg){
          this.consentMsgExtra = this.data.translationData.lblOptInExtraMsg;
        }
        else{
          this.consentMsgExtra = "By selecting and confirming this option you are confirming that the personal data of the selected driver(s), such as the driver ID, will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.";
        }
        break;
      }
      case 'U' : {
        optVal = 'Opt-Out'; 
        if(this.data.translationData.lblOptInOptOutAttemptingMsg){
          this.consentMsg = this.data.translationData.lblOptInOptOutAttemptingMsg.replace('$', optVal);
        }
        else{
          this.consentMsg = ("You are attempting to change consent to '$'").replace('$', optVal);
        } 
        if(this.data.translationData.lblOptOutExtraMsg){
          this.consentMsgExtra = this.data.translationData.lblOptOutExtraMsg;
        }
        else{
          this.consentMsgExtra = "By selecting and confirming this option, you are confirming that you understand that the personal data of the selected driver(s) such as the driver ID will no longer be visible in the DAF CONNECT portal. As a result of opting-out some services will no longer show the driver ID in the DAF CONNECT portal while some services may be terminated altogether. Termination (or partial or complete unavailability) of any services as a result of the opt-out request will by no means result in any restitution of fees or any other form of compensation from DAF Trucks NV.";
        }
        break;
      }
      case 'H' : {
        optVal = 'Inherit';
        if(this.data.translationData.lblInheritAttemptingMsg){
          this.consentMsg = this.data.translationData.lblInheritAttemptingMsg.replace('$', optVal);
        }
        else{
          this.consentMsg = ("You are attempting to '$' your organisation’s consent setting. This means that the consent of this driver will set to the consent of your organisation").replace('$', optVal);
        } 

        if(this.data.translationData.lblSinceyourorganisationconsentis){
          this.inheritMsgExtra = this.data.translationData.lblSinceyourorganisationconsentis.replace('$', (this.orgInheritMode == 'U') ? (this.data.translationData.lblOptOut || 'Opt-Out') : (this.data.translationData.lblOptIn || 'Opt-In'));
        }
        else{
          this.inheritMsgExtra = ("Since your organisation’s consent is '$'.").replace('$', (this.orgInheritMode == 'U') ? (this.data.translationData.lblOptOut || 'Opt-Out') : (this.data.translationData.lblOptIn || 'Opt-In'));
        }
        
        if(this.orgInheritMode == 'U'){ //-- opt-out mode
          if(this.data.translationData.lblOptOutExtraMsg){
            this.consentMsgExtra = this.data.translationData.lblOptOutExtraMsg;
          }
          else{
            this.consentMsgExtra = "By selecting and confirming this option, you are confirming that you understand that the personal data of the selected driver(s) such as the driver ID will no longer be visible in the DAF CONNECT portal. As a result of opting-out some services will no longer show the driver ID in the DAF CONNECT portal while some services may be terminated altogether. Termination (or partial or complete unavailability) of any services as a result of the opt-out request will by no means result in any restitution of fees or any other form of compensation from DAF Trucks NV.";
          }
        }else{ //-- opt-in mode
          if(this.data.translationData.lblOptInExtraMsg){
            this.consentMsgExtra = this.data.translationData.lblOptInExtraMsg;
          }
          else{
            this.consentMsgExtra = "By selecting and confirming this option you are confirming that the personal data of the selected driver(s), such as the driver ID, will be visible in the DAF CONNECT portal. You state that you are aware of your responsibility with regard to data privacy. At the same time, you state that you have consent from all your drivers to have their driver ID stored and shown in the DAF CONNECT portal and/or, if applicable, to share information with third parties. By submitting this request, you fully accept your legal responsibilities and thereby indemnify DAF Trucks NV from any privacy related responsibilities based on this decision.";
          }
        }
        break;
      }
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
        optIn: this.data.consentType,
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
      this.onClose({ tableData: driverList, consentMsg: this.getConsentUpdatedMsg() });
    });
  }

  getConsentUpdatedMsg(){
    let returnMsg: any = '';
      let driverName: any = `${this.data.driverData.firstName} ${this.data.driverData.lastName}`;
      switch(this.data.consentType){ 
        case 'I' : {
          if(this.data.actionType){ //-- All
            if(this.data.translationData.lblAlldriverswassuccessfully){
              returnMsg = this.data.translationData.lblAlldriverswassuccessfully.replace('$', (this.data.translationData.lblOptedin || 'Opted-in'));
            }
            else{
              returnMsg = ("All drivers was '$' successfully").replace('$', (this.data.translationData.lblOptedin || 'Opted-in'));
            }
          }else{ //-- single
            if(this.data.translationData.lblThedrivewasOptedinsuccessfully){
              returnMsg = this.data.translationData.lblThedrivewasOptedinsuccessfully.replace('$', driverName);
            }
            else{
              returnMsg = ("The driver '$' was Opted-in successfully").replace('$', driverName);
            }
          } 
          break;
        }
        case 'U' : {
          if(this.data.actionType){ //-- All
            if(this.data.translationData.lblAlldriverswassuccessfully){
              returnMsg = this.data.translationData.lblAlldriverswassuccessfully.replace('$', (this.data.translationData.lblOptedout || 'Opted-out'));
            }
            else{
              returnMsg = ("All drivers was '$' successfully").replace('$', (this.data.translationData.lblOptedout || 'Opted-out'));
            }
          }else{ //-- single
            if(this.data.translationData.lblThedrivewasOptedoutsuccessfully){
              returnMsg = this.data.translationData.lblThedrivewasOptedoutsuccessfully.replace('$', driverName);
            }
            else{
              returnMsg = ("The driver '$' was Opted-out successfully").replace('$', driverName);
            }
          }
          break;
        }
        case 'H' : {
          if(this.data.actionType){ //-- All
            if(this.data.translationData.lblAlldriverswassuccessfully){
              returnMsg = this.data.translationData.lblAlldriverswassuccessfully.replace('$', (this.orgInheritMode == 'U') ? (this.data.translationData.lblOptedout || 'Opted-out') : (this.data.translationData.lblOptedin || 'Opted-in'));
            }
            else{
              returnMsg = ("All drivers was '$' successfully").replace('$', (this.orgInheritMode == 'U') ? (this.data.translationData.lblOptedout || 'Opted-out') : (this.data.translationData.lblOptedin || 'Opted-in'));
            }
          }else{ //-- single
            if(this.data.translationData.lblThedrivewassuccessfully){
              returnMsg = this.data.translationData.lblThedrivewassuccessfully.replace('$', driverName);
            }
            else{
              returnMsg = ("The driver '$' was successfully '#'").replace('$', driverName);
            }
            if(this.orgInheritMode == 'I'){ //-- opt-in
              returnMsg = returnMsg.replace('#', (this.data.translationData.lblOptedin || 'Opted-in'));
            }else{ //-- opt-out
              returnMsg = returnMsg.replace('#', (this.data.translationData.lblOptedout || 'Opted-out'));
            }
          }
          break;
        }
      }
    return returnMsg;
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

  onUpdateStatusOk(event){
    let objData: any = {
      id: this.data.driverData.id,
      organizationId: this.data.driverData.organizationId,
      driverIdExt: this.data.driverData.driverIdExt,
      email: this.data.driverData.email,
      firstName: this.data.driverData.firstName,
      lastName: this.data.driverData.lastName,
      optIn: this.data.consentType,
      modifiedBy: this.accountId //0
    }
    console.log(objData);    
    this.closePopup = false;
    this.mdDialogRef.close(event);
  }  
}
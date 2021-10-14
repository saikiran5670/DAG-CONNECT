import { Component, OnInit, HostListener, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-schedule-confirm',
  templateUrl: './schedule-confirm.component.html',
  styleUrls: ['./schedule-confirm.component.less'],
  encapsulation: ViewEncapsulation.None
})

export class ScheduleConfirmComponent implements OnInit {
  closePopup: boolean = true;
  accountOrganizationId: any = 0;
  accountId: any = 0;
  organizationName: any;
  releaseNote: any;
  message: any;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    campaignName: any,
    vehicalName: any,
    baseLineId: any,
    scheduleDateTime: any,
  }, private mdDialogRef: MatDialogRef<ScheduleConfirmComponent>) {
    this.organizationName = localStorage.getItem('organizationName');
    this.getSchedulerConfirmationData(data);
  }

  ngOnInit(): void {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
  }

  getSchedulerConfirmationData(data) {
  }

  public onClose(value: any) {
    this.closePopup = false;
    this.mdDialogRef.close(value);
  }
 
  onCancel(){
    this.onClose(true);
  }

  public onSchedule() {
    this.onClose(true);
  }
  
}
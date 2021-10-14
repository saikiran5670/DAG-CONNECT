import { Component, OnInit, HostListener, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslationService } from 'src/app/services/translation.service';


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
  localStLanguage: any;
  translationData: any = {};

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    campaignName: any,
    vehicalName: any,
    baseLineId: any,
    scheduleDateTime: any,
  }, private mdDialogRef: MatDialogRef<ScheduleConfirmComponent>,private translationService: TranslationService) {
    this.organizationName = localStorage.getItem('organizationName');
    this.getSchedulerConfirmationData(data);
  }

  ngOnInit(): void {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 17 //-- for alerts
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    })
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
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
import { Component, OnInit, HostListener, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslationService } from 'src/app/services/translation.service';


@Component({
  selector: 'app-release-note',
  templateUrl: './release-note.component.html',
  styleUrls: ['./release-note.component.less'],
  encapsulation: ViewEncapsulation.None
})

export class ReleaseNoteComponent implements OnInit {
  closePopup: boolean = true;
  accountOrganizationId: any = 0;
  accountId: any = 0;
  organizationName: any;
  releaseNote: any;
  message: any;
  translationData: any = {};
  localStLanguage: any;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    releaseNoteData: any,
    vin: any,
    message: any,
    releaseNotes: any,
  }, private mdDialogRef: MatDialogRef<ReleaseNoteComponent>,private translationService: TranslationService) {
    this.organizationName = localStorage.getItem('organizationName');
    this.getReleaseNotes(data);
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

  getReleaseNotes(data) {
    if (data?.releaseNotes) {
      this.releaseNote = data.releaseNotes;
    }
    if (data?.message) {
      this.message = data.message;
    }
  }

  public onClose(value: any) {
    this.closePopup = false;
    this.mdDialogRef.close(value);
  }
 
  onCancel(){
    this.onClose(false);
  }
}
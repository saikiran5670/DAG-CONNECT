import { Component, OnInit, HostListener, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


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

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    translationData: any,
    releaseNoteData: any,
    vin: any,
    message: any,
    releaseNotes: any,
  }, private mdDialogRef: MatDialogRef<ReleaseNoteComponent>) {
    this.organizationName = localStorage.getItem('organizationName');
    this.getReleaseNotes(data);
  }

  ngOnInit(): void {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
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
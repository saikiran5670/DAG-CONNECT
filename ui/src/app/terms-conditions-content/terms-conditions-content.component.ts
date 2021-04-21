import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { TranslationService } from '../services/translation.service';

@Component({
  selector: 'app-terms-conditions-content',
  templateUrl: './terms-conditions-content.component.html',
  styleUrls: ['./terms-conditions-content.component.less']
})
export class TermsConditionsContentComponent implements OnInit {

  translationData: any= {};
  versions: any= [];
  fileURL: any;
  termsConditionsHistoryFormGroup: FormGroup;

  constructor(private _formBuilder: FormBuilder, private translationService: TranslationService,  private domSanitizer: DomSanitizer) { }

  ngOnInit(): void {
    this.termsConditionsHistoryFormGroup = this._formBuilder.group({
      tcVersions: ['']
    });
    let objData= {
      orgId :  localStorage.getItem('accountOrganizationId'),
      levelCode : 10,
      accountId : localStorage.getItem('accountId'),
    }
    this.translationService.getAllTCVersions(objData).subscribe(data => {
      this.versions= data;
      let objData1= {
        AccountId: localStorage.getItem('accountId'),
        OrganizationId: localStorage.getItem('accountOrganizationId'),
      }  
      this.translationService.getUserAcceptedTC(objData1).subscribe((response)=>{
        let activeTC = response.filter(resp => resp.state === "A")
        this.fileURL = this.getFileURL(activeTC[0].description);
        this.termsConditionsHistoryFormGroup.patchValue({
          tcVersions: activeTC[0].versionno
        });
      });
    }, (error) => {

    })
  }

  onTCVersionChange(value){
    let objData= {
      versionNo: value,
      languageCode: JSON.parse(localStorage.getItem("language")).code
    }
    this.translationService.getTCForVersionNo(objData).subscribe(response => {
      this.fileURL = this.getFileURL(response[0].description);
    });

  }

  getFileURL(byteData): any{
    let arrayBuffer= byteData;
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      return this.domSanitizer.bypassSecurityTrustResourceUrl("data:application/pdf;base64,"+base64File);
  }

}

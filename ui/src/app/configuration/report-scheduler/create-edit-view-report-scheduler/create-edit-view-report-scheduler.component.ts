import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportSchedulerService } from 'src/app/services/report.scheduler.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-edit-view-report-scheduler',
  templateUrl: './create-edit-view-report-scheduler.component.html',
  styleUrls: ['./create-edit-view-report-scheduler.component.less']
})
export class CreateEditViewReportSchedulerComponent implements OnInit {

  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Output() backToPage = new EventEmitter<any>();
  
  breadcumMsg: any = '';
  reportSchedulerForm: FormGroup;
  accountOrganizationId: any;
  accountId: any;
  userType: any= "";
  scheduleCreatedMsg: any= '';
  ReportTypeList: any= [];
  VehicleGroupList: any= [];
  LanguageCodeList: any= [];
  VehicleList: any= [];
  DriverList: any= [];
  selectedIndex: number = 0;
  tabVisibilityStatus: boolean = true;

  constructor(private _formBuilder: FormBuilder, private reportSchedulerService: ReportSchedulerService) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.reportSchedulerForm = this._formBuilder.group({
      reportType : ['', [Validators.required]],
      vehicleGroup : ['', [Validators.required]],
      language : ['', [Validators.required]],
      vehicle : ['', [Validators.required]],
      recipientEmail : ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      driver : ['', [Validators.required]],
      mailSubject : ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      mailDescription : ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
    });
    
    this.breadcumMsg = this.getBreadcum();
    if(this.actionType == 'edit' || this.actionType == 'view'){
      this.setDefaultValues();
    }
  }

  setDefaultValues(){
    // this.imageMaxMsg = false;
    // this.imageEmptyMsg = false;
    // this.categoryForm.get('categoryName').setValue(this.selectedRowData.subCategoryId == 0 ? this.selectedRowData.parentCategoryName : this.selectedRowData.subCategoryName);
    // this.categoryForm.get('type').setValue(this.selectedRowData.organizationId ? (this.selectedRowData.organizationId  > 0 ? 'Regular': 'Global' ) : 'Global');
    // this.categoryForm.get('categoryDescription').setValue(this.selectedRowData.description);
    // this.selectedCategoryType = this.selectedRowData.subCategoryId == 0 ? 'category' : 'subcategory';
    // this.categoryForm.get('parentCategory').setValue(this.selectedRowData.parentCategoryId);
    // //this.categoryForm.get('uploadFile').setValue(this.selectedRowData.icon);
  }
  
  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblReportScheduler : "ReportScheduler"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditScheduleDetails ? this.translationData.lblEditScheduleDetails : 'Edit Schedule Details') : (this.actionType == 'view') ? (this.translationData.lblViewScheduleDetails ? this.translationData.lblViewScheduleDetails : 'View Schedule Details') : (this.translationData.lblCreateScheduleDetails ? this.translationData.lblScheduleNewReport : 'Schedule New Report')}`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onReset(){
    this.setDefaultValues();
  }

  onCreateUpdate(){
    if(this.actionType == 'create'){ //-- create schedule
      let createdObj: any = {
      
      }     
      //this.reportSchedulerService.addLandmarkCategory(createdObj).subscribe((createdData: any) => {
        //if(createdData){
          this.scheduleCreatedMsg = this.getScheduleCreatedUpdatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.scheduleCreatedMsg };
          this.backToPage.emit(emitObj);
      //   }
      // }, (error) => {
        
      // });
    }else{ //-- update category
      let updatedObj: any = {
      }
      // this.reportSchedulerService.updateLandmarkCategory(updatedObj).subscribe((updatedData: any) => {
      //   if(updatedData){
          this.scheduleCreatedMsg = this.getScheduleCreatedUpdatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.scheduleCreatedMsg };
          this.backToPage.emit(emitObj);
      //   }
      // }, (error) => {
        
      // });
    }
  }

  getScheduleCreatedUpdatedMessage() {
    //let categoryName = `${this.categoryForm.controls.categoryName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblScheduleCreatedSuccessfully)
        return this.translationData.lblScheduleCreatedSuccessfully;
      else
        return ("New Report Schedule Created Successfully");
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblScheduleUpdatedSuccessfully)
        return this.translationData.lblScheduleUpdatedSuccessfully;
      else
        return ("Report Schedule Updated Successfully");
    }
    else{
      return '';
    }
  }

  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }
  
 
}

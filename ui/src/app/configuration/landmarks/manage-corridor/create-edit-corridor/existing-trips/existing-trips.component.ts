import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
// import { AccountService } from '../../../services/account.service';
import { CustomValidators } from '../../../../../shared/custom.validators';
import {NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule} from 'ngx-material-timepicker';

@Component({
  selector: 'app-existing-trips',
  templateUrl: './existing-trips.component.html',
  styleUrls: ['./existing-trips.component.less']
})
export class ExistingTripsComponent implements OnInit {
  startDate = new FormControl();
  endDate = new FormControl();
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @Input() disabled: boolean;
  @Input()	value: string = '11:00 PM';
  @Input()	format: number = 12;

  // range = new FormGroup({
  //   start: new FormControl(),
  //   end: new FormControl()
  // });
  translationData: any;
  OrgId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  // displayedColumns: string[] = ['select', 'firstName', 'emailId', 'roles', 'accountGroups'];
  selectedAccounts = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  // @Input() translationData: any;
  // @Input() selectedRowData: any;
  // @Input() actionType: any;
  // userCreatedMsg: any = '';
  // duplicateEmailMsg: boolean = false;
  // breadcumMsg: any = '';
  existingTripForm: FormGroup;
  // groupTypeList: any = [];
  // showUserList: boolean = true;

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.translationData = {
      lblVehicleGroup : '',
      lblToday : '',
      lblYesterday : '',
      lblLastWeek : '',
      lblLastMonth : '',
      lblLast3Months : '',
      lblGroupType: '',
      lblTimeRanges: '',
      lblPleasechoosevehicleGroup: ''

    }
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.existingTripForm = this._formBuilder.group({
      // userGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      // userGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
    {
      validator: [
        // CustomValidators.specialCharValidationForName('userGroupName'),
        // CustomValidators.specialCharValidationForNameWithoutRequired('userGroupDescription')
      ]
    });
  }

}

import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-edit-view-category',
  templateUrl: './create-edit-view-category.component.html',
  styleUrls: ['./create-edit-view-category.component.less']
})

export class CreateEditViewCategoryComponent implements OnInit {
  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  
  constructor() { }

  ngOnInit() { }

}

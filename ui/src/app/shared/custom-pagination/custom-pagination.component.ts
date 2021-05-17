import { Component, Input, OnInit } from '@angular/core';
import { Injectable } from '@angular/core';
// import { TranslateService } from "@ngx-translate/core"; 
import {MatPaginatorModule, MatPaginatorIntl } from '@angular/material/paginator';

@Component({
  selector: 'app-custom-pagination',
  templateUrl: './custom-pagination.component.html',
  styleUrls: ['./custom-pagination.component.less']
})
export class CustomPaginationComponent extends MatPaginatorIntl {
  @Input()
  showFirstLastButtons: boolean;
  constructor() { 
    super()
  // this.nextPageLabel = ' My new label for next page';
  // this.previousPageLabel = ' My new label for previous page';
  this.itemsPerPageLabel = 'View';
  this.getRangeLabel = (page: number, pageSize: number, length: number) => `Page ${(page + 1).toString()} of ${length.toString()} lists`;
  }
  

  ngOnInit(): void {
  }
 
}

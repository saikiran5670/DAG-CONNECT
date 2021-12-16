import { Component, Input, OnInit } from '@angular/core';
import { Injectable } from '@angular/core';
// import { TranslateService } from "@ngx-translate/core"; 
import {MatPaginatorModule, MatPaginatorIntl } from '@angular/material/paginator';
import { TranslationService } from 'src/app/services/translation.service';

@Component({
  selector: 'app-custom-pagination',
  templateUrl: './custom-pagination.component.html',
  styleUrls: ['./custom-pagination.component.less']
})
export class CustomPaginationComponent extends MatPaginatorIntl {
  @Input()
  showFirstLastButtons: boolean;
  constructor(private translationService: TranslationService) { 
    super()
  // this.nextPageLabel = ' My new label for next page';
  // this.previousPageLabel = ' My new label for previous page';
  this.itemsPerPageLabel = translationService.applicationTranslationData.lblView;
  this.getRangeLabel = (page: number, pageSize: number, length: number) => translationService.applicationTranslationData.lblPage +` ${(page + 1).toString()} `+ translationService.applicationTranslationData.lblOf +` ${length.toString()} `+translationService.applicationTranslationData.lblLists;
  }
  

  ngOnInit(): void {
  }
 
}

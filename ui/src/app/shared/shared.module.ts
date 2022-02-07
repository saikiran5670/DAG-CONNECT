import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorIntl, MatPaginatorModule,MatPaginator } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatTableExporterModule } from 'mat-table-exporter';

import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { LayoutModule } from '@angular/cdk/layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MaterialFileInputModule } from 'ngx-material-file-input';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { CommonFilterComponent } from './common-filter/common-filter.component';
import { RemoveDuplicatesPipe } from './pipes/remove-duplicates.pipe';
import { SafeStylePipe } from './pipes/safe-style.pipe';
import { CustomPaginationComponent } from './custom-pagination/custom-pagination.component';
import { SpinnerComponent } from './spinner/spinner.component';
import { ActiveInactiveDailogComponent } from './active-inactive-dailog/active-inactive-dailog.component';
import { CommonImportComponent } from './common-import/common-import.component';
import { CommonMapComponent } from './common-map/common-map.component';
import { DataTableComponent } from './data-table/data-table.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { CommonBreadcrumbsComponent } from './common-breadcrumbs/common-breadcrumbs.component';


@NgModule({
  declarations: [CommonFilterComponent, RemoveDuplicatesPipe, SafeStylePipe, CustomPaginationComponent, SpinnerComponent, ActiveInactiveDailogComponent,CommonImportComponent, CommonMapComponent, DataTableComponent, CommonBreadcrumbsComponent],
  imports: [
    CommonModule,
    FlexLayoutModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatStepperModule,
    LayoutModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    DragDropModule,
    MaterialFileInputModule,
    NgxDropzoneModule,
    MatTableExporterModule,
    NgxMatSelectSearchModule
  ],
  entryComponents:[],
  exports: [
    FlexLayoutModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatStepperModule,
    LayoutModule,
    MatFormFieldModule,
    DragDropModule,
    MaterialFileInputModule,
    CommonFilterComponent,
    RemoveDuplicatesPipe,
    SafeStylePipe,
    SpinnerComponent,
    CommonImportComponent,
    CommonMapComponent,
    DataTableComponent,
    CommonBreadcrumbsComponent
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: CustomPaginationComponent}],
})
export class SharedModule { }

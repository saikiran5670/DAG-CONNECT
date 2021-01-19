import { CommonModule } from '@angular/common';
import { NgModule,CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ConfirmDialogService } from './confirm-dialog.service';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { DeleteDialogComponent } from './delete-dialog.component';


@NgModule({
  imports: [
      CommonModule,
      MatDialogModule,
      MatButtonModule,MatFormFieldModule,
      MatInputModule,FormsModule,
  ],
  declarations: [
    DeleteDialogComponent
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
  ],
  exports: [DeleteDialogComponent],
  entryComponents: [DeleteDialogComponent],
  providers: [ConfirmDialogService]
})
export class DeleteDialogModule {
}

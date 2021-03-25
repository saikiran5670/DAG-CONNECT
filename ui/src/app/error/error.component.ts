import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit {


  constructor(public _route: Router, @Inject(MAT_DIALOG_DATA) public data: {
                  confirmText: string,
                  message: string,
                  title: string,

              }, private mdDialogRef: MatDialogRef<ErrorComponent>) {

  }

  public cancel() {
    this.close(false);
  }

  public close(value) {
    localStorage.clear();
    this._route.navigate(["/auth/login"]);
    this.mdDialogRef.close(value);
  }

  public confirm() {

    this.close(true);
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }

  ngOnInit(): void {
  }

}

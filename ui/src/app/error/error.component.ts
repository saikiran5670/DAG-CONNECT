import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.less']
})
export class ErrorComponent implements OnInit {
  showLoadingIndicator: any = false;

  constructor(public _route: Router, @Inject(MAT_DIALOG_DATA) public data: {
                  confirmText: string,
                  message: string,
                  title: string,

              }, private mdDialogRef: MatDialogRef<ErrorComponent>, private authService: AuthService) {

  }

  public cancel() {
    this.close(false);
  }

  public close(value: any) {
    this.showLoadingIndicator = true;
    if(localStorage.length !== 0) {
      localStorage.clear();
      this.authService.signOut().subscribe(()=>{
        //localStorage.clear(); // clear all localstorage
        this.hideloader();
        this.mdDialogRef.close(value);
        this._route.navigate(["/auth/login"]);
      }, (error) => {
        this.hideloader();
        this.mdDialogRef.close(value);
        this._route.navigate(["/auth/login"]);
      });
    } else {
      this.hideloader();
      this.mdDialogRef.close(value);
      this._route.navigate(["/auth/login"]);
    }
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

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

}

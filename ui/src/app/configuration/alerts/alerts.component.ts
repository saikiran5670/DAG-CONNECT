import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.less']
})
export class AlertsComponent implements OnInit {
  private subscription: Subscription;
  message: any;
  constructor(private alertService: AlertService) { }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  ngOnInit(): void {
    this.subscription = this.alertService.getAlert().subscribe((message) => {
      switch (message && message.type) {
        case "success":
          message.cssClass = "alert alert-success";
          break;
        case "error":
          message.cssClass = "alert alert-danger";
          break;
      }
      this.message = message;
    });
  }

}

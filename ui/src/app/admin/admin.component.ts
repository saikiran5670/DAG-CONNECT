import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, Event, NavigationStart, NavigationCancel, NavigationError } from '@angular/router';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.less']
})
export class AdminComponent implements OnInit {
  showLoadingIndicator=true;
  constructor(private router: Router) {
    // router.events.subscribe((routerEvent: Event) => {
    //   if (routerEvent instanceof NavigationStart) {
    //     this.showLoadingIndicator = true;
    //   }
    //   if (routerEvent instanceof NavigationEnd||routerEvent instanceof NavigationError ||
    //     routerEvent instanceof NavigationCancel) {
    //     this.showLoadingIndicator = false;
    //   }
    // });
   }

  ngOnInit(): void {
  }

}

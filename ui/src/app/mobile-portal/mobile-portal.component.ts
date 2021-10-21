import { AfterContentInit, Component, OnInit } from '@angular/core';
import { ExternalAuthService } from '../services/external-auth.service';

@Component({
  selector: 'app-mobile-portal',
  templateUrl: './mobile-portal.component.html',
  styleUrls: ['./mobile-portal.component.less']
})
export class MobilePortalComponent implements OnInit, AfterContentInit {

  constructor(private externalAuthService: ExternalAuthService) { }

  ngOnInit(): void {
  }

  ngAfterContentInit() {
    let accountFeatures = localStorage.getItem('accountFeatures') ? JSON.parse(localStorage.getItem('accountFeatures')) : {};
    let shopMenu = accountFeatures.menus?.find(el => el.key == 'lblMobilePortal');
    let shopFeature = accountFeatures.features?.find(el => el.featureId == shopMenu.featureId);
    this.externalAuthService.getSsoToken({ "featureName": shopFeature.name }).subscribe((data: any) => {
      if (data.status === 200) {
        window.open(data.body, '_blank');
      }
      else if (data.status === 401) {
        console.log("Error: Unauthorized");
      }
      else if (data.status == 302) {
        console.log("Error: Unauthorized");
      }
    },
      (error) => {
        if (error.status == 404 || error.status == 403) {
          console.log("Error: not found");
        }
        else if (error.status === 401) {
          console.log("Error: Unauthorized");
        }
        else if (error.status == 302) {
          console.log("Error: Unauthorized");
        }
        else if (error.status == 500) {
          console.log("Error: Internal server error");
        }
      })
  }

}

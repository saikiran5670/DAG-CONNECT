import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filters',
  templateUrl: './fleet-overview-filters.component.html',
  styleUrls: ['./fleet-overview-filters.component.less']
})
export class FleetOverviewFiltersComponent implements OnInit {
@Input() translationData: any;
tabVisibilityStatus: boolean = true;
selectedIndex: number = 0;

  constructor() { }

  ngOnInit(): void {
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }
  
  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }
  
}

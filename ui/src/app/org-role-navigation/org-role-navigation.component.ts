import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-org-role-navigation',
  templateUrl: './org-role-navigation.component.html',
  styleUrls: ['./org-role-navigation.component.less']
})
export class OrgRoleNavigationComponent implements OnInit {

  showLoadingIndicator:boolean = true;
  constructor() { }

  ngOnInit(): void {
  }

}

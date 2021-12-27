import { Component, OnInit,Input } from '@angular/core';

@Component({
  selector: 'app-common-breadcrumbs',
  templateUrl: './common-breadcrumbs.component.html',
  styleUrls: ['./common-breadcrumbs.component.css']
})
export class CommonBreadcrumbsComponent implements OnInit {
  @Input() breadcrumb;
  constructor() { }

  ngOnInit(): void {
  }

}

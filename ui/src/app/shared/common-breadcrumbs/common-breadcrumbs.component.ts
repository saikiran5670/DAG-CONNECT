import { Component, OnInit,Input } from '@angular/core';

@Component({
  selector: 'app-common-breadcrumbs',
  templateUrl: './common-breadcrumbs.component.html',
  styleUrls: ['./common-breadcrumbs.component.css']
})
export class CommonBreadcrumbsComponent implements OnInit {
  @Input() breadcrumb;
  @Input() topRightElements;
  constructor() { }

  ngOnInit(): void {
    console.log(this.breadcrumb, 'breadcrumb')
  }

}

import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleetkpi',
  templateUrl: './fleetkpi.component.html',
  styleUrls: ['./fleetkpi.component.less']
})
export class FleetkpiComponent implements OnInit {
  @Input() translationData : any;
  constructor() { }

  ngOnInit(): void {
  }

}

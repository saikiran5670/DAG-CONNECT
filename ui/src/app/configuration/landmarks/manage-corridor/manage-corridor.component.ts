import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-corridor',
  templateUrl: './manage-corridor.component.html',
  styleUrls: ['./manage-corridor.component.less']
})

export class ManageCorridorComponent implements OnInit {
  @Input() translationData: any;
  
  constructor() { }

  ngOnInit(){ }

}

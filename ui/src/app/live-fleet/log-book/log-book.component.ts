import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-log-book',
  templateUrl: './log-book.component.html',
  styleUrls: ['./log-book.component.less']
})

export class LogBookComponent implements OnInit {  
  isVehicleHealthOpen: boolean = false;
  constructor() {}

  ngOnInit(): void {
  }

  openVehicleHealth(){
    this.isVehicleHealthOpen = true;
  }

}
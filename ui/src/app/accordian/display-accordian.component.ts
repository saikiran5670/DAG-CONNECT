import { Component, Input, OnInit,Output,EventEmitter } from '@angular/core';


import { Employee } from '../models/users.model';

@Component({
  selector: 'app-display-accordian',
  templateUrl: './display-accordian.component.html',
  styleUrls: ['./display-accordian.component.css']
})
export class DisplayAccordianComponent implements OnInit {
  @Input() employee:Employee;
  @Input() searchTerm:string;
  @Output() notifyDelete:EventEmitter<number>=new EventEmitter<number>();
  confirmDelete:false;

  constructor() { }

  ngOnInit(): void {
  }

}

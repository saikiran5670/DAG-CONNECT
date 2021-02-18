import { Component, OnInit } from '@angular/core';
import { Employee } from '../models/users.model';
//import { EmployeeService } from '../services/employee.service';
export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}

const ELEMENT_DATA: PeriodicElement[] = [
  {position: 1, name: 'Hydrogen', weight: 1.0079, symbol: 'H'},
  {position: 2, name: 'Helium', weight: 4.0026, symbol: 'He'},
  {position: 3, name: 'Lithium', weight: 6.941, symbol: 'Li'},
  {position: 4, name: 'Beryllium', weight: 9.0122, symbol: 'Be'},
  {position: 5, name: 'Boron', weight: 10.811, symbol: 'B'},
  {position: 6, name: 'Carbon', weight: 12.0107, symbol: 'C'},
  {position: 7, name: 'Nitrogen', weight: 14.0067, symbol: 'N'},
  {position: 8, name: 'Oxygen', weight: 15.9994, symbol: 'O'},
  {position: 9, name: 'Fluorine', weight: 18.9984, symbol: 'F'},
  {position: 10, name: 'Neon', weight: 20.1797, symbol: 'Ne'},
];

@Component({
  selector: 'app-accordian',
  templateUrl: './list-accordian.component.html',
  styleUrls: ['./list-accordian.component.css']
})
export class ListAccordianComponent implements OnInit {
  displayedColumns: string[] = ['position', 'name', 'weight', 'symbol'];
  dset = ELEMENT_DATA;


  panelOpenState = false;
  employees: Employee[];
  filteredEmployees: Employee[];
  private _searchTerm: string;
  constructor(
    //private _empService:EmployeeService
  ) { }
  get searchTerm(): string {
    return this._searchTerm;
  }
  set searchTerm(value: string) {
    this._searchTerm = value;
    this.filteredEmployees = this.filterEmployees(value);
  }
  filterEmployees(searchString: string) {
    return this.employees.filter(
      (e) => e.name.toLowerCase().indexOf(searchString) !== -1
    );
  }
  ngOnInit(): void {
    // this._empService.getEmployees().subscribe((empList) => {
    //   this.employees = empList;
    //   this.filteredEmployees = this.employees;
    // });
    //console.log(this.filteredEmployees);
  }

}

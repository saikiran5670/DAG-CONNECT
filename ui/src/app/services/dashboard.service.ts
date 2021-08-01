import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor() { }

 
  calculatePercentage(_value,_thresholdValue){
    let _percent = (_value / _thresholdValue) * 100;
    return _percent;
  }

}

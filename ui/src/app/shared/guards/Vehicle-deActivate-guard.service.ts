import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
//import { CreateEmployeeComponent } from '../con/';
import { Injectable } from '@angular/core';
import { CreateEditVehicleDetailsComponent } from 'src/app/configuration/vehicle-management/create-edit-vehicle-details/create-edit-vehicle-details.component';

@Injectable()
export class vehicleCanDeActivateGuardService implements CanDeactivate<CreateEditVehicleDetailsComponent>{
  constructor(){}

  canDeactivate(component: CreateEditVehicleDetailsComponent): boolean {
   if(component.createVehicleForm.dirty){
    return confirm('Are you sure you want to discard your changes?');
   }
   return true;
  }

}
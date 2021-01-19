export class VehicleGroup {
  vehicleGroupID: number;
  organizationID: number;
  name: string;
  parentID: number;
  isActive: boolean;
  createdDate: Date;
  createdBy: number;
  updatedDate: Date;
  updatedBy: number;
  isDefaultGroup: boolean;
  isUserDefindGroup: boolean;
  vehicles: [
    {
      vehicleID: number;
      vin: string;
      registrationNo: string;
      chassisNo: string;
      terminationDate: Date;
      isActive: boolean;
      createdDate: Date;
      createdBy: number;
      updatedDate: Date;
      updatedBy: number;
    }
  ];
}

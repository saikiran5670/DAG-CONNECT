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
export class vehicleGrpGetRequest {
  id: number;
  organizationID: number;
  vehicles: boolean;
  vehiclesGroup: boolean;
  groupIds: [{}];
}
export class vehicleGetRequest {
  vehicleId: number;
  organizationID: number;
  vehicleIdList: string;
  vin: string;
  status: number;
}
export class vehicleUpdateRequest {
  id: number;
  name: string;
  description: string;
  organizationId: number;
  vehicles: [
    {
      vehicleGroupId: number;
      vehicleId: number;
    }
  ];
}

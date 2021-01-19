export class Employee {
  id: number;
  name: string;
  gender: string;
  email?: string;
  phoneNumber?: number;
  contactPreference: string;
  dateOfBirth: Date;
  department: string;
  isActive: boolean;
  photoPath?: string;
  password?: string;
  confirmPassword?: string;
  salary?: number;
  joiningDate?: Date;
  exitDate?: Date;
  address?: string;
}
export class Users {
  Salutation: string;
  firstName: string;
  lastName: string;
  emailid: string;
  userTypeid: number;
  createBy: number;
}
export class UserGroup {
  organizationId: number;
  name: string;
  isActive: boolean;
  usergroupId: number;
  vehicles: string;
  users: string;
  id: number;
  userGroupDescriptions: string;
}


export interface Product {
  id: number | null;
  productName: string;
  productCode: string;
  proddescription?: string;
  prodRating?: number;
}

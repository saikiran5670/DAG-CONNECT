using System;
using System.Collections.Generic;
using System.Net;
using static net.atos.daf.ct2.utilities.CommonEnums;

namespace net.atos.daf.ct2.driver.entity
{
    public class Driver
    {
        public int Id { get; set; }
        public int Organization_id { get; set; }
        public string Driver_id_ext { get; set; }
        //  public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //   public long DateOfBith { get; set; }
        public string Status { get; set; }
        public State State { get; set; }
        public string OptIn { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public long CreatedAt { get; set; }
        public string Email { get; set; }
    }
    public class DriverResponse
    {
        public int Id { get; set; }
        public int Organization_id { get; set; }
        public string Driver_id_ext { get; set; }
        //  public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //  public string DateOfBith { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public Boolean IsActive { get; set; }
        public string OptIn { get; set; }
        public string ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedAt { get; set; }
    }
    public class DriverImportResponse
    {
        public string DriverID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ReturnMessage { get; set; }
        public string Status { get; set; }
    }

    public class DriverDatamart
    {
        public string DriverID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OrganizationId { get; set; }
    }

    public class DriverLookupResponse
    {
        public IEnumerable<DriverLookup> DriverLookup { get; set; }
    }

    public class DriverLookup
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
    }

    public class RegisterDriverResponse
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class ValidateDriverResponse
    {
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string RoleID { get; set; } = "DRIVER";
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string UnitDisplay { get; set; }
        public string VehicleDisplay { get; set; }
        public List<ValidateDriverOrganisation> Organisations { get; set; }
    }

    public class ValidateDriverOrganisation
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

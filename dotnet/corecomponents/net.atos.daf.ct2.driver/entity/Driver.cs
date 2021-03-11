using System;

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
        public Boolean IsActive { get; set; }
        public string opt_in { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
        public long created_at { get; set; }
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
        public string opt_in { get; set; }
        public string modified_at { get; set; }
        public string modified_by { get; set; }
        public string created_at { get; set; }
    }
    public class DriverImportResponse
    {
        public string DriverID { get; set; }       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ReturnMessage { get; set; }
    }
}

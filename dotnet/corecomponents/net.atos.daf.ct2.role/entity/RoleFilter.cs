using System;

namespace net.atos.daf.ct2.role.entity
{
    public class RoleFilter
    {

    public int RoleId { get; set; }
    public int Organization_Id { get; set; }
    public int AccountId { get; set; }  
    public bool? Is_Active { get; set; }
    }
}

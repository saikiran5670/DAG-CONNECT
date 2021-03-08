using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class DropdownRequest
    {
        public List<string> Dropdownname { get; set; }
        public string Langaugecode { get; set; }
    }

    public class DropdownResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Filter { get; set; }
        public int MenuId { get; set; }

    }


}

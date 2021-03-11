using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.translationservice;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class Mapper
    {
        public dropdownarrayRequest MapDropdown(DropdownRequest request)
        {
            dropdownarrayRequest response = new dropdownarrayRequest();
            if (request == null) return response;
            if (request != null && request.Dropdownname != null && Convert.ToInt16(request.Dropdownname.Count) > 0)
            {
                foreach (var item in request.Dropdownname)
                {
                    response.Dropdownname.Add(new DropdownName() { Dropdownname = item });
                }
            }
            response.Languagecode = request.Langaugecode;
           
            return response;
        }

    }
}

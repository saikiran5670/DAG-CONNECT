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
            response.Languagecode = request.Languagecode;
           
            return response;
        }
        public TranslationUploadRequest MapFileDetailRequest(FileUploadRequest request)
        {
            TranslationUploadRequest response = new TranslationUploadRequest();
            if (request == null) return response;
            if (request != null && request.file_name != null && request.description != null && Convert.ToInt16(request.file_size) > 0 && Convert.ToInt16(request.failure_count) > 0 && request.file !=null)
            {
                foreach (var item in request.file)
                {
                    response.File.Add(new TranslationData() { Code = item.code, Type = item.type, Name = item.name, Value = item.value });
                }
            }
            response.FileName = request.file_name;
            response.Description = request.description;
            response.FileSize = request.file_size;
            response.FailureCount = request.failure_count;
            response.AddedCount = request.added_count;
            response.UpdatedCount = request.updated_count;

            return response;

        }

    }
}

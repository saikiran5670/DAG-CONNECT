using Google.Protobuf;
using net.atos.daf.ct2.poigeofenceservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class CategoryMapper
    {
        public CategoryAddRequest MapCategory(AddCategoryRequest request)
        {

            var Requests = new CategoryAddRequest();
            Requests.Id = request.Id;
            Requests.OrganizationId = request.Organization_Id;
            Requests.Name = request.Name;
            Requests.IconName = request.IconName;
            Requests.Type = request.Type;
            Requests.ParentId = request.Parent_Id;
            Requests.State = request.State;
            Requests.CreatedBy = request.Created_By;
            Requests.ModifiedBy = request.Modified_By;
            Requests.Icon = ByteString.CopyFrom(request.icon);

            return Requests;

        }


        public CategoryEditRequest MapCategoryforEdit(EditCategoryRequest request)
        {

            var Requests = new CategoryEditRequest();
            Requests.Id = request.Id;
            Requests.Name = request.Name;
            Requests.IconName = request.IconName;
            Requests.ModifiedBy = request.Modified_By;
            Requests.Icon = ByteString.CopyFrom(request.icon);

            return Requests;

        }

        public CategoryDeleteRequest MapCategoryforDelete(DeleteCategoryRequest request)
        {

            var Requests = new CategoryDeleteRequest();
            Requests.Id = request.Id;
            return Requests;
        }
    }
}

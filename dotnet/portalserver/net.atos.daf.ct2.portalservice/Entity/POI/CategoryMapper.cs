using Google.Protobuf;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.portalservice.Entity.Category;

namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class CategoryMapper
    {
        public CategoryAddRequest MapCategory(AddCategoryRequest request)
        {

            var Requests = new CategoryAddRequest();
            Requests.Id = request.Id;
            Requests.OrganizationId = request.Organization_Id;
            Requests.Name = !string.IsNullOrEmpty(request.Name) ? request.Name : string.Empty;
            Requests.IconName = request.IconName;
            Requests.Type = request.Type;
            Requests.ParentId = request.Parent_Id;
            Requests.State = request.State;
            Requests.Description = !string.IsNullOrEmpty(request.Description) ? request.Description : string.Empty;
            Requests.CreatedBy = request.Created_By;
            Requests.ModifiedBy = request.Modified_By;
            Requests.Icon = ByteString.CopyFrom(request.Icon);

            return Requests;

        }


        public CategoryEditRequest MapCategoryforEdit(EditCategoryRequest request)
        {

            var Requests = new CategoryEditRequest();
            Requests.Id = request.Id;
            Requests.Name = !string.IsNullOrEmpty(request.Name) ? request.Name : string.Empty;
            Requests.IconName = request.IconName;
            Requests.ModifiedBy = request.Modified_By;
            Requests.Icon = ByteString.CopyFrom(request.Icon);
            Requests.Description = !string.IsNullOrEmpty(request.Description) ? request.Description : string.Empty;
            Requests.OrganizationId = request.Organization_Id;

            return Requests;

        }

        public CategoryDeleteRequest MapCategoryforDelete(DeleteCategoryRequest request)
        {

            var Requests = new CategoryDeleteRequest();
            Requests.Id = request.Id;
            return Requests;
        }

        public CategoryGetRequest MapCategoryType(GetCategoryTypes request)
        {

            var Requests = new CategoryGetRequest();
            Requests.Type = request.Type;
            Requests.OrganizationId = request.Organization_Id;
            return Requests;
        }

        public DeleteRequest MapCategoryforBulkDelete(DeleteCategory request)
        {

            DeleteRequest response = new DeleteRequest();
            if (request == null) return response;

            if (request != null && request.Category_SubCategory != null)
            {
                foreach (var item in request.Category_SubCategory)
                {
                    response.CatSubCatIDList.Add(new CatSubCatID() { CategoryId = item.CategoryId, SubCategoryId = item.SubCategoryId });
                }
            }
            return response;
        }
    }
}

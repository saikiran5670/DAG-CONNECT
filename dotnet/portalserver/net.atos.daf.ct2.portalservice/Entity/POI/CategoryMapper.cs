﻿using Google.Protobuf;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.portalservice.Entity.Category;
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
            Requests.Name = !string.IsNullOrEmpty(request.Name) ? request.Name : string.Empty;
            Requests.IconName = request.IconName;
            Requests.Type = request.Type;
            Requests.ParentId = request.Parent_Id;
            Requests.State = request.State;
            Requests.Description = !string.IsNullOrEmpty(request.Description) ? request.Description : string.Empty;
            Requests.CreatedBy = request.Created_By;
            Requests.ModifiedBy = request.Modified_By;
            Requests.Icon = ByteString.CopyFrom(request.icon);

            return Requests;

        }


        public CategoryEditRequest MapCategoryforEdit(EditCategoryRequest request)
        {

            var Requests = new CategoryEditRequest();
            Requests.Id = request.Id;
            Requests.Name = !string.IsNullOrEmpty(request.Name) ? request.Name : string.Empty;
            Requests.IconName = request.IconName;
            Requests.ModifiedBy = request.Modified_By;
            Requests.Icon = ByteString.CopyFrom(request.icon);
            Requests.Description = !string.IsNullOrEmpty(request.Description) ? request.Description : string.Empty;

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

            if (request != null && request.category_SubCategory_s != null)
            {
                foreach (var item in request.category_SubCategory_s)
                {
                    response.CatSubCatIDList.Add(new CatSubCatID() { CategoryId = item.CategoryId, SubCategoryId = item.SubCategoryId });
                }
            }
            return response;
        }
    }
}

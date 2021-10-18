namespace net.atos.daf.ct2.httpclientservice.Entity.ota22
{
    public class Mapper
    {
        public VehiclesStatusOverviewResponse MapVehiclesStatusOverview(net.atos.daf.ct2.httpclientfactory.entity.ota22.VehiclesStatusOverviewResponse vehiclesStatusOverviewResponses)
        {
            var returnObj = new VehiclesStatusOverviewResponse();
            returnObj.HttpStatusCode = vehiclesStatusOverviewResponses.HttpStatusCode;
            returnObj.VehiclesStatusOverview = new VehiclesStatusOverview();
            if (vehiclesStatusOverviewResponses?.VehiclesStatusOverview?.Results != null)
            {
                foreach (var item in vehiclesStatusOverviewResponses?.VehiclesStatusOverview?.Results)
                {
                    returnObj.VehiclesStatusOverview
                        .VehiclesStatusOverviewResults
                        .Add(new VehiclesStatusOverviewResults
                        {
                            Vin = item.Vin ?? string.Empty,
                            Status = item.Status ?? string.Empty,
                            Description = item.Description ?? string.Empty
                        });
                }
            }
            return returnObj;
        }

        public net.atos.daf.ct2.httpclientfactory.entity.ota22.VehiclesStatusOverviewRequest MapVehiclesStatusOverviewRequest(VehiclesStatusOverviewRequest vehiclesStatusOverviewRequest)
        {
            var returnObj = new net.atos.daf.ct2.httpclientfactory.entity.ota22.VehiclesStatusOverviewRequest();
            returnObj.Language = vehiclesStatusOverviewRequest.Language;
            returnObj.Retention = vehiclesStatusOverviewRequest.Retention;
            returnObj.Vins = new System.Collections.Generic.List<string>();
            foreach (var item in vehiclesStatusOverviewRequest.Vins)
            {
                returnObj.Vins.Add(item);
            }
            return returnObj;
        }

        internal httpclientfactory.entity.ota22.VehicleUpdateDetailsRequest MapGetVehicleUpdateDetailsRequest(VehicleUpdateDetailsRequest request)
        {
            var returnObj = new httpclientfactory.entity.ota22.VehicleUpdateDetailsRequest();

            returnObj.Retention = request.Retention;
            returnObj.Vin = request.Vin;
            return returnObj;
        }

        internal VehicleUpdateDetailsResponse MapGetVehicleUpdateDetails(httpclientfactory.entity.ota22.VehicleUpdateDetailsResponse apiResponse)
        {
            var returnObj = new VehicleUpdateDetailsResponse();
            returnObj.HttpStatusCode = apiResponse.HttpStatusCode;
            returnObj.VehicleUpdateDetails = new VehicleUpdateDetails();
            returnObj.VehicleUpdateDetails.Vin = apiResponse.VehicleUpdateDetails?.Vin ?? string.Empty;
            returnObj.VehicleUpdateDetails.VehicleSoftwareStatus = apiResponse.VehicleUpdateDetails?.VehicleSoftwareStatus ?? string.Empty;

            if (apiResponse.VehicleUpdateDetails?.Campaigns != null)
                foreach (var item in apiResponse.VehicleUpdateDetails?.Campaigns)
                {
                    var campaign = new Campaign
                    {
                        BaselineAssignment = item.BaselineAssignment,
                        CampaignID = item.CampaignID,
                        CampaignSubject = item.CampaignSubject,
                        CampaignCategory = item.CampaignCategory,
                        CampaignType = item.CampaignType,
                        UpdateStatus = item.UpdateStatus
                    };
                    campaign.Systems.AddRange(item.Systems);
                    returnObj.VehicleUpdateDetails.Campaigns.Add(campaign);
                }
            return returnObj;
        }

        internal net.atos.daf.ct2.httpclientfactory.entity.ota22.CampiagnSoftwareReleaseNoteRequest MapCampiagnSoftwareReleaseNoteRequest(CampiagnSoftwareReleaseNoteRequest request)
        {
            var returnObj = new net.atos.daf.ct2.httpclientfactory.entity.ota22.CampiagnSoftwareReleaseNoteRequest();
            returnObj.CampaignId = request.CampaignId;
            returnObj.Language = request.Language;
            returnObj.Retention = request.Retention;
            returnObj.Vins = new System.Collections.Generic.List<string>();
            foreach (var item in request.Vins)
            {
                returnObj.Vins.Add(item);
            }
            return returnObj;
        }
        internal CampiagnSoftwareReleaseNoteResponse MapGetSoftwareReleaseNote(net.atos.daf.ct2.httpclientfactory.entity.ota22.CampiagnSoftwareReleaseNoteResponse apiResponse)
        {
            var returnObj = new CampiagnSoftwareReleaseNoteResponse();
            returnObj.ReleaseNote = apiResponse.CampiagnSoftwareReleaseNote?.ReleaseNotes ?? string.Empty;
            return returnObj;
        }
    }
}

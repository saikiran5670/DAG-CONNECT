using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.notificationdataservice.Entity
{
    public class CampaignEvent
    {
        [Required(ErrorMessage = "VIN should not be null or empty.")]
        public string VIN { get; set; }
        [Required(ErrorMessage = "Campaign should not be null or empty.")]
        public string Campaign { get; set; }
        [Required(ErrorMessage = "Baseline should not be null or empty.")]
        public string Baseline { get; set; }
        [Required(ErrorMessage = "StatusCode should not be null or empty.")]
        public Statuscode StatusCode { get; set; }
        [Required(ErrorMessage = "Status should not be null or empty.")]
        public string Status { get; set; }
        [Required(ErrorMessage = "CampaignID should not be null or empty.")]
        public string CampaignID { get; set; }
        [Required(ErrorMessage = "Subject should not be null or empty.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Timestamp should not be null or empty.")]
        public long Timestamp { get; set; }
    }

    public class Root
    {
        public CampaignEvent CampaignEvent { get; set; }
    }

    public enum Statuscode
    {
        UPDATE_FAILURE = 8,
        WAITING_FOR_MANAGER_APPROVAL = 10
    }
}

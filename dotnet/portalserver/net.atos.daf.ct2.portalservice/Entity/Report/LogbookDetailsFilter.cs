using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.CustomValidators.Report;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class LogbookDetailsFilter
    {
        [MinLength(1, ErrorMessage = "At least one group id is required.")]
        [GroupIdCheck(ErrorMessage = "Group id is invalid.")]
        public List<string> GroupId { get; set; }

        [MinLength(1, ErrorMessage = "At least one VIN id is required.")]
        public List<string> VIN { get; set; }

        [MinLength(1, ErrorMessage = "At least one alert level is required.")]
        [AlertLevelCheck(ErrorMessage = "Alert level is invalid.")]
        public List<string> AlertLevel { get; set; }

        [MinLength(1, ErrorMessage = "At least one alert category is required.")]
        [AlertCategoryCheck(ErrorMessage = "Alert category is invalid.")]
        public List<string> AlertCategory { get; set; }

        [MinLength(1, ErrorMessage = "At least one VIN id is required.")]
        public List<string> AlertType { get; set; }

        [Required]
        public long Start_Time { get; set; }
        [Required]
        public long End_time { get; set; }


    }
}


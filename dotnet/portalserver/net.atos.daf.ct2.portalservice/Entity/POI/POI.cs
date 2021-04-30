﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class POI
    {
        public int Id { get; set; }
        public byte[] Icon { get; set; }
        public int OrganizationId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        //public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public string State{ get; set; }
        //public double Distance { get; set; }
        //public int TripId { get; set; }
        public int CreatedBy { get; set; }
    }
}

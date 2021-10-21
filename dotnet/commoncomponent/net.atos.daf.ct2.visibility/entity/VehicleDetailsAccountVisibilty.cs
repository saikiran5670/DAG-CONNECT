namespace net.atos.daf.ct2.visibility.entity
{
    public class VehicleDetailsAccountVisibility
    {
        public int VehicleGroupId { get; set; }
        public int AccountId { get; set; } = default;
        public string ObjectType { get; set; } = string.Empty;
        public string GroupType { get; set; }
        public string FunctionEnum { get; set; } = string.Empty;
        public int OrganizationId { get; set; } = default;
        public string AccessType { get; set; } = string.Empty;
        public string VehicleGroupName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string Vin { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public string VehicleGroupDetails { get; set; } = string.Empty;
        public int[] VehicleGroupIds { get; set; } = new int[] { };
    }

    public class VehicleDetailsAccountVisibilityForOTA
    {
        //Need to remove thecomments once done end to end
        //public int VehicleGroupId { get; set; }
        //public int AccountId { get; set; } = default;
        //public string ObjectType { get; set; } = string.Empty;
        //public string GroupType { get; set; }
        ///public string FunctionEnum { get; set; } = string.Empty;
        //public int OrganizationId { get; set; } = default;
        //public string AccessType { get; set; } = string.Empty;
        public string VehicleGroupName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string Vin { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public string VehicleGroupNames { get; set; } = string.Empty;
        public string ModelYear { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string SoftwareStatus { get; set; } = string.Empty;

        public bool HasAdminRights { get; set; }

        //public int[] VehicleGroupIds { get; set; } = new int[] { };
    }

    public class VehicleDetailsVisibiltyAndFeature
    {
        public int VehicleGroupId { get; set; }

        //public string VehicleGroupName { get; set; }

        public int VehicleId { get; set; }

        //public string VehicleName { get; set; }

        public string Vin { get; set; }

        //public string RegistrationNo { get; set; }

        public string FeatureName { get; set; }

        public string FeatureKey { get; set; }

        public bool Subscribe { get; set; }
    }

    public class VehicleDetailsVisibiltyAndFeatureTemp
    {
        public int VehicleId { get; set; }
        public string FeatureKey { get; set; }
        public string SubscriptionType { get; set; }
    }
}

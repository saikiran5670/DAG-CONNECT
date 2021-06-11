using net.atos.daf.ct2.translation.Enum;

namespace net.atos.daf.ct2.translation.entity
{
    public class IconCoreMapper
    {



        public WarningType MapCharToDTCType(string type)
        {
            var statetype = WarningType.DTC;
            switch (type)
            {
                case "D":
                    statetype = WarningType.DTC;
                    break;
                case "M":
                    statetype = WarningType.DM;
                    break;

            }
            return statetype;

        }
        public ColorsName MapCharToColorName(string color)
        {
            var statetype = ColorsName.Green;
            switch (color)
            {
                case "R":
                    statetype = ColorsName.Red;
                    break;
                case "Y":
                    statetype = ColorsName.Yellow;
                    break;
                case "G":
                    statetype = ColorsName.Green;
                    break;

            }
            return statetype;

        }
        public StatusType MapCharToIconState(string status)
        {
            var statetype = StatusType.ACTIVE;
            switch (status)
            {
                case "A":
                    statetype = StatusType.ACTIVE;
                    break;
                case "I":
                    statetype = StatusType.INACTIVE;
                    break;
                case "D":
                    statetype = StatusType.DELETE;
                    break;
            }
            return statetype;

        }
    }
}

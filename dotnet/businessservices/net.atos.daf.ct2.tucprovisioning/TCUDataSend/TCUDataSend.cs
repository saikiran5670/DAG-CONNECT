using System;
using System.Collections.Generic;
using System.Text;

namespace TCUSend
{
    class TCUDataSend
    {

        private VehicleChangeEvent vehicleChangeEvent;

        public TCUDataSend(VehicleChangeEvent vehicleChangeEvent)
        {
            this.vehicleChangeEvent = vehicleChangeEvent;
        }

        public VehicleChangeEvent VehicleChangeEvent { get => vehicleChangeEvent; set => vehicleChangeEvent = value; }
    }
}

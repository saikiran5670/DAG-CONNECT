using System;
using System.Collections.Generic;
using System.Linq;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms
{
    public class RfmsVehicleStatusAccumulator
    {
        public List<AccelerationPedalPositionClass> AccumulateAccelerationPedalPositionClass(dynamic record)
        {

            var accumulatedClassRequest = new AccumulatedClassRequest();

            accumulatedClassRequest.ClassDistanceData = new List<int>() { 40, 10, 20, 2000, 30, 40, 10, 20, 2000, 30 };// record.accelerationpedalposclassdistr 
            accumulatedClassRequest.MaxRange = 100;// record.accelerationpedalposclassmaxrange 
            accumulatedClassRequest.MinRange = 0;//record.accelerationpedalposclassminrange 
            accumulatedClassRequest.NoOfStep = 10;// = record.accelerationpedalposclassdistrstep 



            var accClass = new List<AccelerationPedalPositionClass>();
            var intervals = GetPedalInterval(accumulatedClassRequest.MaxRange, accumulatedClassRequest.MinRange, accumulatedClassRequest.NoOfStep);
            foreach (var item in intervals.Select((value, i) => new { i, value }))
            {
                accClass.Add(new AccelerationPedalPositionClass()
                {
                    From = Convert.ToInt32(item.value.Split('-')[0]),
                    To = Convert.ToInt32(item.value.Split('-')[1]),
                    Seconds = accumulatedClassRequest.ClassDistanceData[item.i],
                    Meters = null,
                    MilliLitres = null
                });
            }
            return accClass;
        }
        //RetarderTorqueClass(
        public List<RetarderTorqueClass> AccumulateRetarderTorqueClass(dynamic record)
        {

            var accumulatedClassRequest = new AccumulatedClassRequest();

            accumulatedClassRequest.ClassDistanceData = new List<int>() { 40, 10, 20, 2000, 30, 40, 10, 20, 2000, 30 };// record.retardertorqueclassdistr 
            accumulatedClassRequest.MaxRange = 100;// record.retardertorqueclassmaxrange 
            accumulatedClassRequest.MinRange = 0;//record.retardertorqueclassminrange 
            accumulatedClassRequest.NoOfStep = 10;// = record.retardertorqueclassdistrstep 



            var accClass = new List<RetarderTorqueClass>();
            var intervals = GetPedalInterval(accumulatedClassRequest.MaxRange, accumulatedClassRequest.MinRange, accumulatedClassRequest.NoOfStep);
            foreach (var item in intervals.Select((value, i) => new { i, value }))
            {
                accClass.Add(new RetarderTorqueClass()
                {
                    From = Convert.ToInt32(item.value.Split('-')[0]),
                    To = Convert.ToInt32(item.value.Split('-')[1]),
                    Seconds = accumulatedClassRequest.ClassDistanceData[item.i],
                    Meters = null,
                    MilliLitres = null
                });
            }
            return accClass;
        }
        public List<EngineTorqueAtCurrentSpeedClass> AccumulateEngineTorqueAtCurrentSpeedClass(dynamic record)
        {

            var accumulatedClassRequest = new AccumulatedClassRequest();

            accumulatedClassRequest.ClassDistanceData = new List<int>() { 40, 10, 20, 2000, 30, 40, 10, 20, 2000, 30 };// record.enginetorqueengineloadclassdistr 
            accumulatedClassRequest.MaxRange = 100;// record.enginetorqueengineloadclassmaxrange 
            accumulatedClassRequest.MinRange = 0;//record.enginetorqueengineloadclassminrange 
            accumulatedClassRequest.NoOfStep = 10;// = record.enginetorqueengineloadclassdistrstep 



            var accClass = new List<EngineTorqueAtCurrentSpeedClass>();
            var intervals = GetPedalInterval(accumulatedClassRequest.MaxRange, accumulatedClassRequest.MinRange, accumulatedClassRequest.NoOfStep);
            foreach (var item in intervals.Select((value, i) => new { i, value }))
            {
                accClass.Add(new EngineTorqueAtCurrentSpeedClass()
                {
                    From = Convert.ToInt32(item.value.Split('-')[0]),
                    To = Convert.ToInt32(item.value.Split('-')[1]),
                    Seconds = accumulatedClassRequest.ClassDistanceData[item.i],
                    Meters = null,
                    MilliLitres = null
                });
            }
            return accClass;
        }
        private List<string> GetPedalInterval(int maxSize, int minSize, int step)
        {
            Dictionary<int, string> intervalRanges = new Dictionary<int, string>();
            List<string> intervals = new List<string>();
            int index = 0;
            int[] res = new int[10];
            for (int i = minSize; i <= maxSize;
            i += step)
            {
                index += 1;
                var a = i;//== 0 ? i : i += 1;
                var b = i + step > maxSize ? maxSize : i + step;
                if (a < maxSize)
                {
                    var range = a.ToString() + "-" + b.ToString();
                    intervalRanges.Add(index, range);
                    intervals.Add(range);
                };

            }
            return intervals;// intervalRanges;
        }
    }
}

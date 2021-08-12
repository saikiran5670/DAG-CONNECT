using System;
using System.Collections.Generic;
using System.Linq;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports
{

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }
    public class ChartMatrix
    {
        public int[] IA { get; set; }
        public int[] JA { get; set; }
        public int[] A { get; set; }
        public int ColumnsCount { get; set; }
        public int RowsCount { get; set; }
        public int[,] DMatrix { get; set; }
    }
    public class PerformanceChartMatrix
    {
        public List<KPIs> CalculateKPIData(List<IndexWiseChartData> vehicleChartDatas, double tripDuration, List<KpiDataRange> rangedata)
        {
            List<KPIs> lstKpis = new List<KPIs>();
            vehicleChartDatas = vehicleChartDatas.Where(s => s.Value != 0).ToList();

            //foreach (var item in vehicleChartDatas)
            //{
            //    int value = item.Value;
            //    foreach (var range in rangedata)
            //    {
            //        switch (range.Index)
            //        {
            //            case 0:// " ":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 1: // "O":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 2: // "A":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 3:// "P":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 4:// "E":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 5:// "N":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 6:// "I":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 7:// "D":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 8:// "U":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;
            //            case 9:// " ":
            //                if (value >= range.LowerVal && value <= range.UpperVal)
            //                    range.Value += value;
            //                break;

            //            default:
            //                break;
            //        }

            //    }


            //}
            var totalTripDurationinHr = (tripDuration / 3600);
            foreach (var kpiDict in rangedata)
            {
                KPIs kPIs = new KPIs();
                kPIs.Label = kpiDict.Kpi;
                kPIs.Index = kpiDict.Index;
                kPIs.Value = totalTripDurationinHr > 0 ? (kpiDict.Value / totalTripDurationinHr) : kPIs.Value;
                lstKpis.Add(kPIs);
            }
            return lstKpis;

        }

        public VehiclePerformanceData Getcombinedmatrix(List<VehPerformanceChartData> chartData, string performanceType, List<KpiDataRange> rangedata, double tripDuration)
        {
            //Console.WriteLine("Hello World!");
            VehiclePerformanceData result = new VehiclePerformanceData();
            List<ChartMatrix> matrix = new List<ChartMatrix>();
            foreach (var item in chartData)
            {
                ChartMatrix matdata = new ChartMatrix();
                matdata.A = item.MatrixValue.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.IA = item.CountPerIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.JA = item.ColumnIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.A = performanceType == "B" ? matdata.A.Select(x => -x).ToArray() : matdata.A;

                matdata.DMatrix = ParseSparseMatrix(matdata.A, matdata.IA);//, JA);
                matdata.ColumnsCount = matdata.DMatrix.GetUpperBound(1) - matdata.DMatrix.GetLowerBound(1) + 1;
                matdata.RowsCount = matdata.DMatrix.GetUpperBound(0) - matdata.DMatrix.GetLowerBound(0) + 1;
                matrix.Add(matdata);
            }
            int mAxColsCount = matrix.Max().ColumnsCount;
            int maxRowsCount = matrix.Max().RowsCount;
            result.ChartData = new List<IndexWiseChartData>();
            foreach (var item in matrix)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        IndexWiseChartData data = new IndexWiseChartData();
                        try
                        {
                            int value = item.DMatrix[i, j];
                            foreach (var range in rangedata)
                            {
                                switch (range.Index)
                                {
                                    case 0:// " ":
                                        if (value >= range.LowerVal && value <= range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 1: // "O":
                                        if (value > range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 2: // "A":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 3:// "P":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 4:// "E":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 5:// "N":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 6:// "I":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 7:// "D":
                                        if (value >= range.LowerVal && value <= range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 8:// "U":
                                        if (value > range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 9:// " ":
                                        if (value > range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;

                                    default:
                                        break;
                                }

                            }
                            if (result.ChartData.Any(k => k.Xindex == i && k.Yindex == j))
                            {
                                result.ChartData.First(k => k.Xindex == i && k.Yindex == j).Value += value;
                            }
                            else
                            {
                                data.Xindex = i;
                                data.Yindex = j;
                                data.Value += item.DMatrix[i, j];
                                result.ChartData.Add(data);
                            }

                        }
                        catch (Exception)
                        {
                            // If index is not present in matrix it will throw exception we will put 0 in that case.
                            if (result.ChartData.Any(k => k.Xindex == i && k.Yindex == j))
                            {
                                result.ChartData.First(k => k.Xindex == i && k.Yindex == j).Value += 0;
                            }
                            else
                            {
                                data.Xindex = i;
                                data.Yindex = j;
                                data.Value += 0;
                                result.ChartData.Add(data);
                            }
                        }

                    }
                }
            }
            result.PieChartData = CalculateKPIData(result.ChartData, tripDuration, rangedata);
            return result;
        }

        public VehiclePerformanceData GetcombinedmatrixBrake(List<VehPerformanceChartData> chartData, string performanceType, List<KpiDataRange> rangedata, double tripDuration)
        {
            //Console.WriteLine("Hello World!");
            VehiclePerformanceData result = new VehiclePerformanceData();
            List<ChartMatrix> matrix = new List<ChartMatrix>();
            foreach (var item in chartData)
            {
                ChartMatrix matdata = new ChartMatrix();
                matdata.A = item.MatrixValue.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.IA = item.CountPerIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.JA = item.ColumnIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.A = performanceType == "B" ? matdata.A.Select(x => -x).ToArray() : matdata.A;

                matdata.DMatrix = ParseSparseMatrix(matdata.A, matdata.IA);//, JA);
                matdata.ColumnsCount = matdata.DMatrix.GetUpperBound(1) - matdata.DMatrix.GetLowerBound(1) + 1;
                matdata.RowsCount = matdata.DMatrix.GetUpperBound(0) - matdata.DMatrix.GetLowerBound(0) + 1;
                matrix.Add(matdata);
            }
            //int mAxColsCount = matrix.Max().ColumnsCount;
            //int maxRowsCount = matrix.Max().RowsCount;
            result.ChartData = new List<IndexWiseChartData>();
            foreach (var item in matrix)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        IndexWiseChartData data = new IndexWiseChartData();
                        try
                        {
                            int value = item.DMatrix[i, j];
                            int y = 0;
                            foreach (var range in rangedata)
                            {
                                switch (range.Index)
                                {
                                    case 0:// " ":
                                        if (value >= range.LowerVal && value <= range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 1: // "O":
                                        if (value > range.LowerVal && value < range.UpperVal)
                                        {
                                            range.Value += value;
                                            if (value < -0.5)
                                            {
                                                y = 1;
                                            }
                                            else
                                            {
                                                y = 2;
                                            }
                                        }

                                        break;
                                    case 2: // "A":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                        {
                                            range.Value += value;
                                            if (value <= -0.75)
                                            {
                                                y = 3;
                                            }
                                            else
                                            {
                                                y = 4;
                                            }
                                        }
                                        break;
                                    case 3:// "P":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                        {
                                            range.Value += value;
                                            if (value <= -1.25)
                                            {
                                                y = 5;
                                            }
                                            else
                                            {
                                                y = 6;
                                            }
                                        }
                                        break;
                                    case 4:// "E":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                        {
                                            range.Value += value;
                                            if (value <= -2)
                                            {
                                                y = 7;
                                            }
                                            else
                                            {
                                                y = 8;
                                            }
                                        }
                                        break;
                                    case 5:// "N":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                        {
                                            range.Value += value;
                                            if (value <= -3)
                                            {
                                                y = 9;
                                            }
                                            else
                                            {
                                                y = 10;
                                            }
                                        }
                                        break;
                                    case 6:// "I":
                                        if (value >= range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 7:// "D":
                                        if (value >= range.LowerVal && value <= range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 8:// "U":
                                        if (value > range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;
                                    case 9:// " ":
                                        if (value > range.LowerVal && value < range.UpperVal)
                                            range.Value += value;
                                        break;

                                    default:
                                        break;
                                }

                            }
                            if (result.ChartData.Any(k => k.Xindex == i && k.Yindex == y))
                            {
                                result.ChartData.First(k => k.Xindex == i && k.Yindex == y).Value += value;
                            }
                            else
                            {
                                data.Xindex = i;
                                data.Yindex = y > 0 ? y : j;
                                data.Value += item.DMatrix[i, j];
                                result.ChartData.Add(data);
                            }

                        }
                        catch (Exception)
                        {
                            // If index is not present in matrix it will throw exception we will put 0 in that case.
                            if (result.ChartData.Any(k => k.Xindex == i && k.Yindex == j))
                            {
                                result.ChartData.First(k => k.Xindex == i && k.Yindex == j).Value += 0;
                            }
                            else
                            {
                                data.Xindex = i;
                                data.Yindex = j;
                                data.Value += 0;
                                result.ChartData.Add(data);
                            }
                        }

                    }
                }
            }
            result.PieChartData = CalculateKPIData(result.ChartData, tripDuration, rangedata);
            return result;
        }
        public static void DisplayMultiDimentionalMatrix(int[,] matrix, string Name)
        {
            //DISPLAY ARRAY TO VERIFY
            Console.WriteLine(Name);
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write("{0} ", matrix[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("---------------------------------------------------------------");
        }

        public static int[,] ParseSparseMatrix(int[] A, int[] IA)//, int[] JA)
        {
            IDictionary<int, int[]> rowColsDisct = new Dictionary<int, int[]>();
            int rowCnt = 0;
            foreach (int i in IA)
            {
                if (i > 0)
                {
                    int length = i - IA[rowCnt - 1];
                    int[] noOfElements = A.SubArray(IA[rowCnt - 1], length);
                    rowColsDisct.Add(rowCnt, noOfElements);
                }
                else
                {
                    int[] noOfElements = new int[0];
                    rowColsDisct.Add(rowCnt, noOfElements);
                }
                rowCnt++;
            }
            return CreateMultiDimentionalArray(rowColsDisct);
        }

        public static int[,] CreateMultiDimentionalArray(IDictionary<int, int[]> dict)
        {
            var keyOfMaxValue = dict.Aggregate((x, y) => x.Value.Length > y.Value.Length ? x : y).Key;
            int mdMaxRowCount = dict.Count;
            int mdMaxColCount = dict[keyOfMaxValue].Length;
            int[,] result = new int[mdMaxRowCount, mdMaxColCount];
            for (int rowCnt = 0; rowCnt < mdMaxRowCount; rowCnt++)
            {
                if (dict[rowCnt].Length > 0)
                {
                    int[] rowValues = dict[rowCnt];
                    for (int colCnt = 0; colCnt < mdMaxColCount; colCnt++)
                    {
                        if (rowValues.Length <= colCnt)
                        {
                            result[rowCnt, colCnt] = 0;
                        }
                        else
                        {
                            result[rowCnt, colCnt] = rowValues[colCnt];
                        }
                    }
                }
                else
                {
                    result[rowCnt, 0] = 0;
                }
            }
            return result;
        }
    }
}

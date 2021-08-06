using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
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

        public List<IndexWiseChartData> Getcombinedmatrix(List<VehPerformanceChartData> chartData)
        {
            //Console.WriteLine("Hello World!");
            List<ChartMatrix> matrix = new List<ChartMatrix>();
            foreach (var item in chartData)
            {
                ChartMatrix matdata = new ChartMatrix();
                matdata.A = item.MatrixValue.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.IA = item.CountPerIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.JA = item.ColumnIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.DMatrix = ParseSparseMatrix(matdata.A, matdata.IA);//, JA);
                matdata.ColumnsCount = matdata.DMatrix.GetUpperBound(1) - matdata.DMatrix.GetLowerBound(1) + 1;
                matdata.RowsCount = matdata.DMatrix.GetUpperBound(0) - matdata.DMatrix.GetLowerBound(0) + 1;
                matrix.Add(matdata);
            }
            //int engineLoadArrayColsCount = EngineLoadArray.GetUpperBound(1) - EngineLoadArray.GetLowerBound(1) + 1;
            //int engineLoadArrayRowsCount = EngineLoadArray.GetUpperBound(0) - EngneLoadArray.GetLowerBound(0) + 1;
            List<IndexWiseChartData> chartdata = new List<IndexWiseChartData>();
            foreach (var item in matrix)
            {

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        IndexWiseChartData data = new IndexWiseChartData();
                        try
                        {

                            if (chartdata.Any(k => k.Xindex == i && k.Yindex == j))
                            {
                                chartdata.First(k => k.Xindex == i && k.Yindex == j).Value += item.DMatrix[i, j];
                            }
                            else
                            {
                                data.Xindex = i;
                                data.Yindex = j;
                                data.Value += item.DMatrix[i, j];
                                chartdata.Add(data);
                            }

                        }
                        catch (Exception)
                        {
                            // If index is not present in matrix it will throw exception we will put 0 in that case.
                            if (chartdata.Any(k => k.Xindex == i && k.Yindex == j))
                            {
                                chartdata.First(k => k.Xindex == i && k.Yindex == j).Value += 0;
                            }
                            else
                            {
                                data.Xindex = i;
                                data.Yindex = j;
                                data.Value += 0;
                                chartdata.Add(data);
                            }
                        }

                    }
                }
            }
            return chartdata;
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

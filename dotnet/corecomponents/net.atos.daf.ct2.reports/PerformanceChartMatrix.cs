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
        public int Arra { get; set; }
    }
    public class Program
    {

        public void Getcombinedmatrix(List<VehPerformanceChartData> chartData)
        {
            //Console.WriteLine("Hello World!");
            List<ChartMatrix> matrix = new List<ChartMatrix>();
            foreach (var item in chartData)
            {
                ChartMatrix matdata = new ChartMatrix();
                matdata.A = item.MatrixValue.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.IA = item.CountPerIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matdata.JA = item.ColumnIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                matrix.Add(matdata);
            }
            //int engineLoadArrayColsCount = EngineLoadArray.GetUpperBound(1) - EngineLoadArray.GetLowerBound(1) + 1;
            //int engineLoadArrayRowsCount = EngineLoadArray.GetUpperBound(0) - EngineLoadArray.GetLowerBound(0) + 1;
            foreach (var item in chartData)
            {

                int[] A = item.MatrixValue.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                int[] IA = item.CountPerIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                int[] JA = item.ColumnIndex.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                //int[] A = new int[] { 5, 100, 112, 6, 2, 1, 1, 4, 2, 1, 1, 2, 1, 1, 13, 5, 3, 3, 2, 1, 1, 3, 2, 1, 18, 9, 9, 9, 5, 3, 2, 1, 1, 1, 2, 1, 9, 13, 22, 16, 4, 3, 4, 4, 1, 1, 2, 1, 2, 3, 1, 8, 2, 5, 4, 4, 3, 3, 1, 2, 1, 2, 2, 1, 2, 2, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1 };
                //int[] IA = new int[] { 0, 7, 14, 24, 36, 49, 59, 70, 77, 81, 83, 84, 84, 84, 84, 84, 84, 84, 84, 84, 84, 84, 84, 84, 84, 84 };
                //int[] JA = new int[] { 0, 1, 2, 3, 4, 5, 7, 0, 1, 2, 3, 5, 6, 7, 0, 1, 2, 3, 4, 5, 7, 8, 9, 11, 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 12, 13, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 0, 1, 2, 3, 4, 5, 6, 7, 8, 11, 0, 1, 2, 3, 5, 6, 7, 9, 13, 15, 17, 0, 2, 5, 7, 12, 13, 17, 0, 4, 7, 14, 2, 15, 5 };

                //int[] B = new int[] { 217, 7, 5, 7, 1, 2, 2, 10, 2, 17, 6, 11, 8, 3, 2, 1, 1, 3, 12, 48, 70, 24, 12, 6, 5, 1, 1 };
                //int[] IB = new int[] { 0, 7, 17, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27 };
                //int[] JB = new int[] { 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                //int[] C = new int[] { 0, 1, 1, 1, 1, 4, 1, 3, 2, 2, 2, 9, 1, 6, 0, 0, 0, 0, 0, 0 };
                //int[] IC = new int[] { 0, 1, 1, 1, 4, 5, 5, 8, 8, 11, 11, 11, 14, 14, 17, 17, 20, 20, 20, 20, 20, 20, 20, 20 };
                //int[] JC = new int[] { 0, 0, 1, 2, 1, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2 };

                var EngineLoadArray = ParseSparseMatrix(A, IA);//, JA);
                //var speedRPMArray = ParseSparseMatrix(B, IB, JB);
                //var BrakeArray = ParseSparseMatrix(C, IC, JC);
            }
            //DisplayMultiDimentionalMatrix(EngineLoadArray, "Engine Load Array");
            //DisplayMultiDimentionalMatrix(speedRPMArray, "Speed RPM Array");
            //DisplayMultiDimentionalMatrix(BrakeArray, "Brake Array");
            //int engineLoadArrayColsCount = EngineLoadArray.GetUpperBound(1) - EngineLoadArray.GetLowerBound(1) + 1;
            //int engineLoadArrayRowsCount = EngineLoadArray.GetUpperBound(0) - EngineLoadArray.GetLowerBound(0) + 1;
            int noOfTrips = 2;
            for (int tripCnt = 0; tripCnt < noOfTrips; tripCnt++)
            {
                //var EngineLoadArray = ParseSparseMatrix(A, IA, JA);
                //int value = 0;
                //for (int i = 0; i < engineLoadArrayRowsCount; i++)
                //{
                //    for (int j = 0; j < engineLoadArrayColsCount; j++)
                //    {
                //        value += EngineLoadArray[i, j];
                //    }
                //}
            }

            //int testValue = 0;
            //foreach (var tst in A)
            //{
            //    testValue += tst;
            //}
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

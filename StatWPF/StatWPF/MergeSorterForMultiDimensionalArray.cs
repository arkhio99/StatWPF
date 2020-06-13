using System;
using System.Collections.Generic;
using System.Text;

namespace StatWPF
{
    static class MergeSorterForMultiDimensionalArray
    {
        static private double[,] GetNewArrayWhereCountOfRowsIsPowerOf2(double[,] data, int indexOfImportantColumn)
        {
            int newRows = (int)Math.Pow(2,(Math.Ceiling(Math.Log2(data.GetLength(0)))));
            double[,] sortedData = new double[newRows, data.GetLength(1)];

            for (int i = 0; i < newRows; i++)
            {
                if (i < data.GetLength(0))
                {
                    sortedData[i, 0] = data[i, 0];
                    sortedData[i, 1] = data[i, 1];
                    sortedData[i, 2] = data[i, 2];
                }
                else
                {
                    sortedData[i, indexOfImportantColumn] = double.PositiveInfinity;
                }
            }
            return sortedData;
        }

        static private void Merge(int column, double[,] dataToMerge, int leftEdge1, int leftEdge2, int width)
        {
            int i1 = leftEdge1;
            int i2 = leftEdge2;
            int rightEdge1 = leftEdge1 + width;
            int rightEdge2 = leftEdge2 + width;
            double[,] mergedData = new double[width * 2, dataToMerge.GetLength(1)];
            int countOfElementsInMergedData = 0;

            while (i1 < rightEdge1 && i2 < rightEdge2)
            {
                if (dataToMerge[i1, column] < dataToMerge[i2, column])
                {
                    for (int i = 0; i < mergedData.GetLength(1); i++)
                        mergedData[countOfElementsInMergedData, i] = dataToMerge[i1, i];
                    countOfElementsInMergedData++;
                    i1++;
                }
                else
                {
                    for (int i = 0; i < mergedData.GetLength(1); i++)
                        mergedData[countOfElementsInMergedData, i] = dataToMerge[i2, i];
                    countOfElementsInMergedData++;
                    i2++;
                }
            }
            while (i1 < rightEdge1)
            {
                for (int i = 0; i < mergedData.GetLength(1); i++)
                    mergedData[countOfElementsInMergedData, i] = dataToMerge[i1, i];
                countOfElementsInMergedData++;
                i1++;
            }

            while (i2 < rightEdge2)
            {
                for (int i = 0; i < mergedData.GetLength(1); i++)
                    mergedData[countOfElementsInMergedData, i] = dataToMerge[i2, i];
                countOfElementsInMergedData++;
                i2++;
            }
            for (int i = 0; i < mergedData.GetLength(0); i++)
                for (int j = 0; j < mergedData.GetLength(1); j++)
                {
                    dataToMerge[leftEdge1 + i, j] = mergedData[i, j];
                }
        }
        static public void MergeSort(ref double[,] data, int indexOfImportantColumn)
        {
            double[,] sortedData = GetNewArrayWhereCountOfRowsIsPowerOf2(data, indexOfImportantColumn);
            for (int width = 1; width < sortedData.GetLength(0); width *= 2)
            {
                for (int i = 0; i < sortedData.GetLength(0); i += 2 * width)
                {
                    Merge(indexOfImportantColumn, sortedData, i, i + width, width);
                }
            }
            for(int i = 0; i < data.GetLength(0); i++)
            {
                for(int j = 0; j < data.GetLength(1); j++)
                {
                    data[i, j] = sortedData[i, j];
                }
            }
        }
    }
}

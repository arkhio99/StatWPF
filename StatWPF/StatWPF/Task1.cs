using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace StatWPF
{
    class Task1
    {
        private int countOfIntervals;
        public readonly double[,] data;
        private int indexOfImportantColumn;
        public readonly double width;
        public List<Interval> intervals;

        public Task1(int _countOfIntervals, double[,] _data, int _indexOfImportantColumn)
        {
            data = _data;
            countOfIntervals = _countOfIntervals;
            indexOfImportantColumn = _indexOfImportantColumn;
            MergeSorterForMultiDimensionalArray.MergeSort(ref data, indexOfImportantColumn);
            width = h();
            intervals = new List<Interval>(countOfIntervals);
            double start = data[0,indexOfImportantColumn];
            while (start < data[data.GetLength(0)-1, indexOfImportantColumn]-0.1)
            {
                intervals.Add(new Interval(start,start+width));
                start += width;
            }
            FillIntervals();
            FillAccumulatedFrequencies();
            FillMi();
        }        

        private void FillIntervals()
        {
            int indexOfInterval = 0;
            for(int i=0;i<data.GetLength(0);i++)
            {
                if (data[i, indexOfImportantColumn] > intervals[indexOfInterval].rightEdge)
                {
                    indexOfInterval++;
                }
                intervals[indexOfInterval].AddElement(data[i, indexOfImportantColumn]);
            }
        }

        private void FillAccumulatedFrequencies()
        {
            int accum = 0;
            for(int i=0;i<intervals.Count;i++)
            {
                Interval temp = intervals[i];
                accum += intervals[i].Frequency;
                temp.accumulatedFrequency = accum;
                intervals[i] = temp;
            }
        }

        private void FillMi()
        {
            for (int i = 0; i < intervals.Count; i++)
            {
                Interval temp = intervals[i];
                temp.mi = (double)temp.Frequency / data.GetLength(0);
                intervals[i] = temp;
            }
        }

        private double h(){
                return (data[data.GetLength(0) - 1, indexOfImportantColumn] - data[0, indexOfImportantColumn]) / countOfIntervals;
        }

        public class Interval
        {
            public readonly double leftEdge;
            public readonly double rightEdge;
            List<double> elements;
            public readonly double average;
            public int accumulatedFrequency;
            public double mi;
            public Interval(double l, double r)
            {
                leftEdge = l;
                rightEdge = r;
                average = l + (r - l) / 2;
                elements = new List<double>();
                accumulatedFrequency = 0;
                mi = 0;
            }
            public override string ToString()
            {
                return $"[{leftEdge:f2};{rightEdge:f2}]";
            }

            public void AddElement(double d)
            {
                elements.Add(d);
            }

            public int Frequency
            {
                get
                {
                    return elements.Count;
                }
            }     
        }
        public IEnumerable<string> GetIntervals()
        {
            return from interval in intervals select interval.ToString();
        }
    }
}

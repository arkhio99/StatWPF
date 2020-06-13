using System;
using System.Collections.Generic;
using System.Text;

namespace StatWPF
{
    public static class doubleArrayExtension
    {
        static public List<Tuple<double, double, double>> ToList(this double[,] d)
        {
            List<Tuple<double, double, double>> list = new List<Tuple<double, double, double>>(d.GetLength(0));
            for (int i = 0; i < d.GetLength(0); i++)
            {
                list.Add(new Tuple<double, double, double>(d[i, 0], d[i, 1], d[i, 2]));
            }
            return list;
        }
    }
}

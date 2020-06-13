using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.XPath;

namespace StatWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Task1 task1;
        public MainWindow()
        {
            InitializeComponent();
            label1.Text = "Для удобства отсортируем наши данные по нужному нам столбцу. Чтобы найти среднеарифметическое взвешенное, нам нужно высчитать частости, мода - частоты, медиана - накопленные частоты. ";
        }

        

        private void LoadData(DataGrid sourceDG, DataGrid sortedDG, string path)
        {
            using (StreamReader input = new StreamReader(path))
            {
                //первая строка - кол-во интервалов
                //вторая строка - индекс нужного столбца
                //третья строка - названия столбцов
                int howIntervals = int.Parse(input.ReadLine().Split(';')[0]);
                int indexOfImportantColumn = int.Parse(input.ReadLine().Split(';')[0]);
                string temp1 = input.ReadLine();
                Console.WriteLine(temp1);
                string[] headers = temp1.Split(';');
                sourceColumn1TB.Text = headers[0];
                sourceColumn2TB.Text = headers[1];
                sourceColumn3TB.Text = headers[2];
                sortedColumn1TB.Text = headers[0];
                sortedColumn2TB.Text = headers[1];
                sortedColumn3TB.Text = headers[2];
                string[] rows = input.ReadToEnd().Split('\n');
                double[,] data = new double[rows.Length, 3];
                for (int i = 0; i < rows.Length; i++)
                {
                    string[] temp = rows[i].Split(';');
                    for (int j = 0; j < 3; j++)
                    {
                        data[i, j] = double.Parse(temp[j]);
                    }
                }
                sourceDG.ItemsSource = data.ToList();
                task1 = new Task1(howIntervals, data, indexOfImportantColumn);
                sortedDG.ItemsSource = task1.data.ToList();
                sourceDG.Columns[0].Width = 35;
                sourceDG.Columns[1].Width = 75;
                sourceDG.Columns[2].Width = 80;
                sortedDG.Columns[0].Width = 35;
                sortedDG.Columns[1].Width = 75;
                sortedDG.Columns[2].Width = 80;
                hLbl.Content = "=" + $"{task1.width:f2}";
                intervalsTB.Text = "";
                frequenciesTB.Text = "";
                accumulatedFrequenciesTB.Text = "";
                miTB.Text = "";
                int count = 1;
                foreach(var interval in task1.intervals)
                {
                    intervalsTB.Text += (count)+". "+interval.ToString() + $" Середина: {interval.average:f2}"+"\n";
                    frequenciesTB.Text += (count) + ". " + $"{interval.Frequency:f2}" + '\n';
                    accumulatedFrequenciesTB.Text += (count) + ". " + interval.accumulatedFrequency.ToString() + '\n';
                    miTB.Text+= (count) + ". " + $"{interval.mi.ToString():f3}" + "\n";
                    count++;
                }
                double xAvg = GetXAvg(task1.intervals);
                double moda = GetModa(task1.intervals);
                double mediana = GetMedian(task1.intervals);
                double razmah = task1.data[task1.data.GetLength(0) - 1, indexOfImportantColumn] - task1.data[0, indexOfImportantColumn];
                RTB.Text = $"R={task1.data[task1.data.GetLength(0) - 1, indexOfImportantColumn]:f2}-{task1.data[0, indexOfImportantColumn]:f2}={razmah:f2}";
                double AvgLinOtkl = GetAvgLinOtkl(task1.intervals, xAvg);
                double vDisp = GetVibDisp(task1.intervals, xAvg);
                constructResultTable(task1.intervals);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            string path;
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                LoadData(sourceDG, sortedDG, path);
            }
            else
            {
                System.Windows.MessageBox.Show("Wrong path");
            }

        }

        private double GetXAvg(List<Task1.Interval> l)
        {
            xAvgTB.Text = "";
            double result = 0;
            bool isStart = true;
            foreach(var val in l)
            {
                result += val.average * val.mi;
                xAvgTB.Text += (isStart?"":"+")+$"{val.average:f2}*{val.mi:f2}";
                isStart = false;
            }
            xAvgTB.Text += $"={result:f2}";
            return result;
        }
        
        private double GetModa(List<Task1.Interval> l)
        {
            Task1.Interval temp=l[0];
            int index = 0;
            for(int i = 1; i < l.Count; i++)
            {
                if(temp.Frequency<l[i].Frequency)
                {
                    index = i;
                    temp = l[i];
                }
            }

            double result = 0;
            result = temp.leftEdge + (temp.rightEdge - temp.leftEdge) * (temp.Frequency - l[index - 1].Frequency) /
                                     ((temp.Frequency - l[index - 1].Frequency) + (temp.Frequency - l[index + 1].Frequency));
            string output = $"{temp.leftEdge:f2}+{(temp.rightEdge - temp.leftEdge):f2}*"+
                            $"({temp.Frequency}-{l[index - 1].Frequency})/(({temp.Frequency}-{l[index - 1].Frequency})+({temp.Frequency}-{l[index + 1].Frequency}))={result:f2}";
            modTB.Text = output;
            return result;
        }

        private double GetMedian(List<Task1.Interval> l)
        {
            double halfOfSumOfFrequencies = l[l.Count - 1].accumulatedFrequency / 2;
            Task1.Interval temp = l[0];
            int i = 1;
            while (temp.accumulatedFrequency < halfOfSumOfFrequencies)
            {
                temp = l[i];
                i++;
            }
            i--;
            double result = temp.leftEdge + (temp.rightEdge - temp.leftEdge) * (halfOfSumOfFrequencies - l[i - 1].accumulatedFrequency) / (temp.Frequency);
            string output = $"{ temp.leftEdge:f2}+{temp.rightEdge - temp.leftEdge:f2}*({halfOfSumOfFrequencies:f2}-{l[i - 1].accumulatedFrequency:f2})/{temp.Frequency:f2}={result:f2}";
            medianaTB.Text = output;
            return result;
        }

        private double GetAvgLinOtkl(List<Task1.Interval> l, double xAvg)
        {
            double result = 0;
            string output = "";
            bool isStart = true;
            for(int i = 0; i < l.Count; i++)
            {
                output += (isStart? "" : "+");
                output += $"|{l[i].average:f2}-{xAvg:f2}|*{l[i].mi:f2}";
                result += Math.Abs(l[i].average-xAvg) * l[i].mi;
                isStart = false;
            }
            output += $"={result:f2}";
            AvgLinOtklTB.Text = output;
            return result;
        }
        private double GetVibDisp(List<Task1.Interval> l, double xAvg)
        {
            double result = 0;
            string output = "";
            bool isStart = true;
            for (int i = 0; i < l.Count; i++)
            {
                output += (isStart ? "" : "+");
                output += $"({l[i].average:f2}-{xAvg:f2})^2 *{l[i].mi:f2}";
                result += (l[i].average - xAvg)* (l[i].average - xAvg) * l[i].mi;
                isStart = false;
            }
            output += $"={result:f2}";
            vDispTB.Text = output;
            return result;
        }

        private void constructResultTable(List<Task1.Interval> l)
        {
            resultDG.ItemsSource = from interval in l select new Tuple<string, string, int, int, string>(interval.ToString(), $"{interval.average:f2}", interval.Frequency, interval.accumulatedFrequency,$"{interval.mi:f2}") ;
        }
    }
}

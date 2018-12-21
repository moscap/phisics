using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using AForge.Math;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using OxyPlot;


namespace WindowsFormsApp1
{
    class Functions
    {
        public static double trans_func_gauss(double w, double sigma)
        {
            return Math.Exp(-w * w * sigma * sigma / 2.0) * Math.Sqrt(sigma) / Math.Sqrt(Math.PI * 2);
        }
        public static double func_gauss(double x, double sigma, double offset = 0)
        {
            double new_x = x - offset;
            return Math.Exp(-new_x * new_x / (sigma * sigma * 2.0)); // / (Math.Sqrt(2.0 * Math.PI) * sigma);
        }
        public static double func_rect(double x, double start, double end)
        {
            // Returns rect that is non-zero from -5 to 5.
            if (x < start || x > end)
            {
                return 0;
            }
            else
            {
                return 5;
            }
        }
        public static double trans_func_rect(double x, double start, double end)
        {
            return Math.Abs((end - start) * 5 *  Trig.Sinc(x * ((end - start) / 2.0) / Math.PI));
        }
        // приводим к нормальному виду
        public static void FlipFlop<T>(T[] f)
        {
            if (f.Length % 2 == 0)
            {
                for (int i = 0, j = f.Length / 2; j < f.Length; ++i, ++j)
                {
                    T buf = f[i];
                    f[i] = f[j];
                    f[j] = buf;
                }
            }
            else
            {
                T mem = f[f.Length / 2];
                for (int i = 0, j = f.Length / 2 + 1; j < f.Length; ++i, ++j)
                {
                    T buf = f[i];
                    f[i] = f[j];
                    f[j] = buf;
                }
                for (int i = f.Length / 2; i < f.Length - 1; ++i)
                {
                    f[i] = f[i + 1];
                }
                f[f.Length - 1] = mem;
            }
        }
        public static void FastDFT(Complex[] x, int mode = 1)
        {
            var dir = FourierTransform.Direction.Forward;
            if (mode == -1)
            {
                dir = FourierTransform.Direction.Backward;
            }
            FourierTransform.FFT(x, dir);
        }
        public static void DFT(Complex[] x, int mode = 1)
        {
            var dir = FourierTransform.Direction.Forward;
            if (mode == -1)
            {
                dir = FourierTransform.Direction.Backward;
            }
            FourierTransform.DFT(x, dir);
        }
        public static void complex_re_paint_min_max(object obj, double[] x, Complex[] y, double koef = 1.0, double sigma = 10, double offset = 0, string name = "New plot!")
        {
            var paint_obj = obj as System.Windows.Forms.DataVisualization.Charting.Chart;
            var ser = paint_obj.Series.Add(name);
            paint_obj.ChartAreas[0].AxisX.Minimum = x.Min();
            paint_obj.ChartAreas[0].AxisX.Maximum = x.Max();
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            ser.BorderWidth = 3;
            for (int i = 0; i < x.Length; i++)
            {
                ser.Points.AddXY(x[i], y[i].Re / koef);
            }
        }
        public static void complex_re_paint(object obj, double[] x, Complex[] y, double koef = 1.0, double sigma = 10,double offset = 0, string name = "New plot!")
        {
            var paint_obj = obj as System.Windows.Forms.DataVisualization.Charting.Chart;
            var ser = paint_obj.Series.Add(name);
            paint_obj.ChartAreas[0].AxisX.Maximum = 4000;
            paint_obj.ChartAreas[0].AxisX.Minimum = 400;
            paint_obj.ChartAreas[0].AxisX.IntervalOffset = 600;
            paint_obj.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            paint_obj.ChartAreas[0].AxisX.Interval = 1000;
            paint_obj.ChartAreas[0].AxisY.Maximum = 1.2;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            ser.BorderWidth = 3;
            for (int i = 0; i < x.Length; i++)
            {
                    ser.Points.AddXY(x[i], y[i].Re / koef);
            }
        }
        public static void complex_re_paint_2(object obj, double[] x, Complex[] y, double koef = 1.0, double sigma = 10, double offset = 0, string name = "New plot!")
        {
            var paint_obj = obj as System.Windows.Forms.DataVisualization.Charting.Chart;
            var ser = paint_obj.Series.Add(name);
            paint_obj.ChartAreas[0].AxisX.Maximum = 4000;
            paint_obj.ChartAreas[0].AxisX.Minimum = 400;
            paint_obj.ChartAreas[0].AxisX.IntervalOffset = 600;
            paint_obj.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            paint_obj.ChartAreas[0].AxisX.Interval = 1000;
            paint_obj.ChartAreas[0].AxisY.Maximum = 1.2;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            ser.MarkerSize = 10;
            ser.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            ser.BorderWidth = 2;
            for(int i = x.Length - 1; (i >= 0) ; --i)
            {
                ser.Points.AddXY(2 * x[0] - x[i], y[y.Length - i - 1].Re / koef);
            }
            for (int i = 0; i < x.Length; i++)
            {
                ser.Points.AddXY(x[i], y[i].Re / koef);
            }
            for (int i = 0; (i < x.Length) && ((x[x.Length - 1] + x[i] - x[0])) <= 5000; ++i)
            {
                ser.Points.AddXY(x[x.Length - 1] + x[i] - x[0], y[i].Re / koef);
            }
        }

        public static void complex_re_paint_2_n(object obj, double[] x, Complex[] y, double koef = 1.0, double sigma = 10, double offset = 0, string name = "New plot!")
        {
            var paint_obj = obj as System.Windows.Forms.DataVisualization.Charting.Chart;
            var ser = paint_obj.Series.Add(name);
            paint_obj.ChartAreas[0].AxisX.Maximum = 4000;
            paint_obj.ChartAreas[0].AxisX.Minimum = 400;
            paint_obj.ChartAreas[0].AxisX.IntervalOffset = 600;
            paint_obj.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            paint_obj.ChartAreas[0].AxisX.Interval = 1000;
            paint_obj.ChartAreas[0].AxisY.Maximum = 1.2;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            ser.MarkerSize = 10;
            ser.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            ser.MarkerColor = System.Drawing.Color.Brown;
            ser.BorderWidth = 2;
            for (int i = x.Length - 1; (i >= 0); --i)
            {
                ser.Points.AddXY(2 * x[0] - x[i], y[y.Length - i - 1].Re / koef);
            }
            for (int i = 0; i < x.Length; i++)
            {
                ser.Points.AddXY(x[i], y[i].Re / koef);
            }
            for (int i = 0; (i < x.Length) && ((x[x.Length - 1] + x[i] - x[0])) <= 5000; ++i)
            {
                ser.Points.AddXY(x[x.Length - 1] + x[i] - x[0], y[i].Re / koef);
            }
        }
        public static void complex_re_paint_for_ch2(object obj, double[] x, Complex[] y, double koef = 1.0, double sigma = 10, double offset = 0, string name = "New plot!")
        {
            var paint_obj = obj as System.Windows.Forms.DataVisualization.Charting.Chart;
            var ser = paint_obj.Series.Add(name);
            paint_obj.ChartAreas[0].AxisX.Maximum = 4000;
            paint_obj.ChartAreas[0].AxisX.Minimum = 400;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            for (int i = 0; i < x.Length; i++)
            {
                ser.Points.AddXY(x[i], y[i].Re / koef);
            }
        }
        
    }
}

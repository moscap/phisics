using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// using OxyPlot;
// using OxyPlot.Series;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        void PrintRes(complex[] y, int max, PaintEventArgs e)
        {
            //int centerX = this.ClientSize.Width / 2;
            //int centerY = this.ClientSize.Height / 2;
            //Pen p = new Pen(Color.Black, 2);
            //Pen p1 = new Pen(Color.Red, 2);
            //complex[] ans = SlowDFT(y);
            //for (int i = 0; i < ans.Length - 1; ++i)
            //{
            //    e.Graphics.DrawLine(p, (float)(centerX + centerX * (float)(i / (float)max)),
            //        (float)(centerY - centerY * (float)(ans[i].Magnitude / (float)max)),
            //        (float)(centerX + centerX * (float)((i + 1) / (float)max)),
            //        (float)(centerY - centerY * (float)(ans[i + 1].Magnitude / (float)max)));

            //}
            //for (int i = 0; i < y.Length - 1; ++i)
            //{
            //    e.Graphics.DrawLine(p1, (float)(centerX + centerX * (i / (float)max)),
            //        (float)(centerY - centerY * (Math.Exp(y[i].Magnitude) / (float)max)),
            //        (float)(centerX + centerX * ((i + 1) / (float)max)),
            //        (float)(centerY - centerY * (Math.Exp(y[i + 1].Magnitude) / (float)max)));

            //}
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            //int x = 0;
            //int y = 0;
            //Pen pen1 = new Pen(Color.Black, 2);
            //Bitmap point = new Bitmap(this.Width, this.Height);
            //Graphics g = Graphics.FromImage(point);
            ////Получение центра оси координат Х,Y
            ////Прорисовка оси
            //int centerX = this.ClientSize.Width / 2;
            //int centerY = this.ClientSize.Height / 2;
            //e.Graphics.Clear(Color.White);
            //e.Graphics.DrawLine(pen1, 0, centerY, centerX * 2, centerY);//Ось OX
            //e.Graphics.DrawLine(pen1, centerX, 0, centerX, centerY * 2);//Ось OY
            //complex[] f = new complex[100];
            ///*for(int i = 0; i < 50; ++i)
            //{
            //    f[i] = new complex(5);
            //}
            //for (int i = 50; i < 100; ++i)
            //{
            //    f[i] = new complex(0);
            //}*/

            //for(int i = 0; i < 100; ++i)
            //{
            //    f[i] = new complex(Math.Exp(-Math.Pow(i / 100.0, 2) / 2.0));
            //}

            //PrintRes(f, 100, e);
            /* for (int i = -8; i < 8; ++i)
             {
                 e.Graphics.DrawLine(pen1, centerX + centerX / 8 * i, centerY - centerY * (float)Math.Pow(1.0 / 8.0 * i, 2),
                     centerX + centerX / 8 * (i + 1), centerY - centerY * (float)Math.Pow(1.0 / 8.0 * (i + 1), 2));
             }*/
        }

        // Maybe it should have complex array
        static complex[] SlowDFT(double[] x)
        {
            int N = x.Length;
            complex[] X = new complex[N];

            for (int k = 0; k < N; k++)
            {
                X[k] = new complex(0);

                for (int n = 0; n < N; n++)
                {
                    complex temp = complex.from_polar(
                        1,
                        -2 * Math.PI * n * k / N
                    );
                    temp *= new complex(x[n]); // how to overload *= ??
                    X[k] += temp;
                }
            }

            return X;
        }

        static double func_rect(double x)
        {
            // Returns rect that is non-zero from -5 to 5.
            if (x < -5 || x > 5) {
                return 0;
            }
            else
            {
                return 5;
            }
        }

        static double func_gauss(double x, double sigma)
        {
            // Returns rect that is non-zero from -5 to 5.
            return Math.Exp(-x * x / (sigma * sigma * 2)) / (Math.Sqrt(2 * Math.PI) * sigma);
        }

        //private OxyPlot.WindowsForms.PlotView plot1;
        //private OxyPlot.WindowsForms.PlotView plot2;
        //this.plot1 = new OxyPlot.WindowsForms.PlotView();
        //this.plot2 = new OxyPlot.WindowsForms.PlotView();

        public Form1()
        {
            InitializeComponent();

            var FourierFunc = new OxyPlot.PlotModel { Title = "Fourie" };
            var SourceFunc = new OxyPlot.PlotModel { Title = "Source Func" };

            int NumOfPoints = 100;
            double XStart = -10.0;
            double XEnd = 10.0;
            double Interval = (XEnd - XStart) / NumOfPoints;
            double[] x = new double[NumOfPoints];

            // Fill x 
            for (int i = 0; i < NumOfPoints; i++) 
            {
                x[i] = XStart + Interval * i;
            }

            double[] f = new double[NumOfPoints];
            // Compute function values (rectangle)
            for (int i = 0; i < NumOfPoints; i++)
            {
                // f[i] = func_gauss(x[i], 1);
                f[i] = func_rect(x[i]);
            }

            // Don't know how to cast double array to complex 

            complex[] FourierF = SlowDFT(f);
            double[] RealFourierF = new double[FourierF.Length];
            for (int i = 0; i < FourierF.Length; i++)
            {
                RealFourierF[i] = FourierF[i].Magnitude;
            }

            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values =  new ChartValues<double> (f)
                }
            };

            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values =  new ChartValues<double> (RealFourierF)
                }
            };

            var lineSeriesF = new LineSeries();
            /*
            for (int i = 0; i < f.Length; i++)
            {
                lineSeriesF.Points.Add(new DataPoint(x[i], FourierF[i].Magnitude));
            }

            var lineSeriesS = new LineSeries();

            for (int i = 0; i < f.Length; i++)
            {
                lineSeriesS.Points.Add(new DataPoint(x[i], f[i]));
            }

            FourierFunc.Series.Add(lineSeriesF);
            FourierFunc.Series.Add(lineSeriesS);
            */
            // SourceFunc.Series.Add(lineSeriesS);

            this.plotView1.Model = FourierFunc;
            
            // this.plotView2.

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* var model = new PlotModel { Title = "ContourSeries" };
            double x0 = -3.1;
            double x1 = 3.1;
            double y0 = -3;
            double y1 = 3; 
            Func<double, double, double> peaks = (x, y) => 3 * (1 - x) * (1 - x) * 
            Math.Exp(-(x * x) -(y + 1) * (y + 1)) -10 * (x / 5 - x * x * x - y * y * y * y * y) * 
            Math.Exp(-x * x - y * y) - 1.0 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            var xx = ArrayBuilder.CreateVector(x0, x1, 1000);
            var yy = ArrayBuilder.CreateVector(y0, y1, 1000);
            var peaksData = ArrayBuilder.Evaluate(peaks, xx, yy);
            var cs = new ContourSeries {
                Color = OxyColors.Black,
                LabelBackground = OxyColors.White,
                ColumnCoordinates = yy,
                RowCoordinates = xx, Data = peaksData
            };
            model.Series.Add(cs);
            this.plotView1.Model = model;
            */
        }

    }
}

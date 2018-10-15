using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public int NumOfPoints { get; set; }
        double XStart { get; set; }
        double XEnd { get; set; }
        double[] x = null;
        double sigma;



        protected override void OnPaint(PaintEventArgs e)
        {            
        }
        static double trans_func_gauss(double w, double sigma)
        {
            return Math.Exp(-w * w * sigma * sigma / 2.0) * Math.Sqrt(sigma) / Math.Sqrt(Math.PI * 2);
        }
        static double func_gauss(double x, double sigma)
        {
            return Math.Exp(-x * x / (sigma * sigma * 2.0)) / (Math.Sqrt(2.0 * Math.PI) * sigma);
        }
        // приводим к нормальному виду
        void FlipFlop(double[] f)
        {
            if (f.Length % 2 == 0)
            {
                for (int i = 0, j = f.Length / 2; j < f.Length; ++i, ++j)
                {
                    double buf = f[i];
                    f[i] = f[j];
                    f[j] = buf;
                }
            }
            else
            {
                double mem = f[f.Length / 2];
                for (int i = 0, j = f.Length / 2 + 1; j < f.Length; ++i, ++j)
                {
                    double buf = f[i];
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
        // здесь выполняется все кроме пересоздания массива при
        // изменении числа точек и изменения параметров
        // измение массива х возлагатся на метод, в котором изменяеются его параметры
        void Repaint()
        {
            double[] f = new double[NumOfPoints];

            for (int i = 0; i < NumOfPoints; i++)
            {
                f[i] = func_gauss(x[i], sigma);
            }

            complex[] FourierF = complex.SlowDFT(f);

            double[] RealFourierF = new double[FourierF.Length];

            for (int i = 0; i < FourierF.Length; i++)
            {
                RealFourierF[i] = FourierF[i].Magnitude;
            }
            double koef = RealFourierF[0];
            FlipFlop(RealFourierF);

            var ListPoints = new ChartValues<LiveCharts.Defaults.ObservablePoint>();
            for (int i = 0; i < x.Length; i++)
            {
                ListPoints.Add(new LiveCharts.Defaults.ObservablePoint
                {
                    X = x[i],
                    Y = f[i]
                });
            }
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    // Values =  new ChartValues<double> (f)
                    Values =  ListPoints
                }
            };
            var yy = ArrayBuilder.CreateVector(-Math.PI / ((XEnd - XStart) / NumOfPoints),
                Math.PI / ((XEnd - XStart) / NumOfPoints), NumOfPoints);
            // TODO: change x axis for Fourier transformed function
            var ListPoints2 = new ChartValues<LiveCharts.Defaults.ObservablePoint>();
            for (int i = 0; i < yy.Length; i++)
            {
                ListPoints2.Add(new LiveCharts.Defaults.ObservablePoint
                {
                    X = yy[i],
                    Y = RealFourierF[i] / koef
                });
            }

            var ListPointsAnalyt = new ChartValues<LiveCharts.Defaults.ObservablePoint>();
            for (int i = 0; i < yy.Length; i++)
            {
                ListPointsAnalyt.Add(new LiveCharts.Defaults.ObservablePoint
                {
                    X = yy[i],
                    Y = trans_func_gauss(yy[i], sigma) / trans_func_gauss(0.0, sigma)
                });
            }

            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ListPoints2
                },
                new LineSeries
                {
                    Values = ListPointsAnalyt
                }
            };

        }
        // ставятся начальные значения
        public Form1()
        {
            InitializeComponent();
            NumOfPoints = trackBar1.Value;
            XStart = Convert.ToDouble(textBox2.Text);
            XEnd = Convert.ToDouble(textBox4.Text);
            sigma = Convert.ToDouble(textBox1.Text);
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 NewForm = new Form2();    
            NewForm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        // дальше проверка длинны это проверка того что ты не удалили все из текст бокса
        // а проверка приведения, это проверка того что в текст боксе число
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double buf;
            if (textBox1.Text.Length == 0) return;
            else if (!double.TryParse(textBox1.Text, out buf)) return;
            sigma = buf;
            Repaint();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            double buf;
            if (textBox2.Text.Length == 0) return;
            else if (!double.TryParse(textBox2.Text, out buf)) return;
            XStart = buf;
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            double buf;
            if (textBox4.Text.Length == 0) return;
            else if (!double.TryParse(textBox4.Text, out buf)) return;
            XEnd = buf;
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            NumOfPoints = trackBar1.Value;
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }
    }
}

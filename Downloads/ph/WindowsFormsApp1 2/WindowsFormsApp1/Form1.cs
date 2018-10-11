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
            return Math.Exp(-x * x / (sigma * sigma * 2)) / (Math.Sqrt(2 * Math.PI) * sigma);
        }

        public Form1()
        {
            InitializeComponent();
            NumOfPoints = trackBar1.Value;
            XStart = Convert.ToDouble(textBox2.Text);
            XEnd = Convert.ToDouble(textBox4.Text);
            sigma = Convert.ToDouble(textBox1.Text);

            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);

            double[] f = new double[NumOfPoints];
            // Compute function values (rectangle)
            for (int i = 0; i < NumOfPoints; i++)
            {
                f[i] = func_gauss(x[i], sigma);
                //f[i] = func_rect(x[i]);
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
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 NewForm = new Form2(this);    
            NewForm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            sigma = Convert.ToDouble(textBox1.Text);
            double[] f = new double[NumOfPoints];
            // Compute function values (rectangle)
            for (int i = 0; i < NumOfPoints; i++)
            {
                f[i] = func_gauss(x[i], sigma);
                //f[i] = func_rect(x[i]);
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
        }
    }
}

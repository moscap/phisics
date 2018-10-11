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
    public partial class Form2 : Form
    {
        public int NumOfPoints { get; set; }
        double XStart { get; set; }
        double XEnd { get; set; }
        double[] x = null;
        double Start { get; set; }
        double End { get; set; }

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

        static double func_rect(double x, double start, double end)
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
        void Repaint()
        {
            double[] f = new double[NumOfPoints];
            // Compute function values (rectangle)
            for (int i = 0; i < NumOfPoints; i++)
            {
                f[i] = func_rect(x[i], Start, End);
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

        public Form2()
        {
            InitializeComponent();
            NumOfPoints = trackBar1.Value;
            XStart = Convert.ToDouble(textBox3.Text);
            XEnd = Convert.ToDouble(textBox4.Text);
            Start = Convert.ToDouble(textBox1.Text);
            End = Convert.ToDouble(textBox2.Text);
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void elementHost2_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double buf;
            if (textBox1.Text.Length == 0) return;
            else if (!double.TryParse(textBox1.Text, out buf)) return;
            Start = buf;
            Repaint();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            double buf;
            if (textBox2.Text.Length == 0) return;
            else if (!double.TryParse(textBox2.Text, out buf)) return;
            End = buf;
            Repaint();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            double buf;
            if (textBox3.Text.Length == 0) return;
            else if (!double.TryParse(textBox3.Text, out buf)) return;
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

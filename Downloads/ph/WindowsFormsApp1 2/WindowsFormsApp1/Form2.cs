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
using MathNet.Numerics;
using AForge.Math;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public int NumOfPoints { get; set; }
        double XStart { get; set; }
        double XEnd { get; set; }
        double[] x = null;
        double sigma;
        double sigma1;
        Graphics graphics { get; set; }
        Rectangle moving_length { get; set; }

        // здесь выполняется все кроме пересоздания массива при
        // изменении числа точек и изменения параметров
        // измение массива х возлагатся на метод, в котором изменяеются его параметры
        void Repaint()
        {
            cartesianChart1.Series = new SeriesCollection();
            cartesianChart2.Series = new SeriesCollection();
            cartesianChart3.Series = new SeriesCollection();
            Complex[] f = new Complex[NumOfPoints];
            for (int i = 0; i < NumOfPoints; i++)
            {
                f[i] = new Complex(Functions.func_gauss(this.x[i], sigma, 1) *
                    Math.Sin(this.x[i] * Math.PI * 10), 0);
            }
            Functions.complex_re_paint(cartesianChart1, this.x, f);
            //Complex[] auto_f = new Complex[f.Length];
            //for (int i = 0; i < NumOfPoints; i++)
            //{
            //    auto_f[i] = new Complex(0, 0);
            //    for (int j = 0; j < NumOfPoints; j++)
            //    {
            //        auto_f[i] += f[(j + i) % NumOfPoints] * f[j];
            //    }
            //}
            //Complex[] ift_auto_f = new Complex[NumOfPoints];
            //auto_f.CopyTo(ift_auto_f, 0);
            Functions.FastDFT(f, -1);
            Functions.FlipFlop(f);
            var x = ArrayBuilder.CreateVector(-Math.PI / ((XEnd - XStart) / NumOfPoints),
                Math.PI / ((XEnd - XStart) / NumOfPoints), NumOfPoints);
            for (int i = 0; i < NumOfPoints; i++)
            {
                f[i] *= new Complex(1 - Functions.func_gauss(x[i], sigma1, 1.5), 0);
            }
            // Complex[] f_copy = new Complex[f.Length];
            // f.CopyTo(f_copy, 0);
            // Functions.FastDFT(f, -1);
            // Functions.FastDFT(f_copy, 1);
            //textBox3.Text = "ok" + NumOfPoints.ToString();
            double koef_r = f.Max(t => t.Re);
            //double koef_auto = auto_f.Max(t => t.Re);
            //double f_auto = ift_auto_f.Max(t => t.Magnitude);
            // double koef_s = f_copy.Max(t => t.Magnitude);
            // Functions.FlipFlop(f_copy);
            // Functions.FlipFlop(f);
            // Functions.FlipFlop(ift_auto_f);
   
            Functions.complex_re_paint(cartesianChart2, x, f, koef_r);
           // Functions.FastDFT
            //Functions.complex_re_paint(cartesianChart3, this.x, auto_f, koef_auto);
            //Functions.complex_magnitude_paint(cartesianChart2, x, ift_auto_f, f_auto);
            // Functions.complex_magnitude_paint(cartesianChart3, x, f_copy, koef_s);
            // Func<double, double> t_r_f = (prm) => Functions.trans_func_gauss(prm, sigma) / Functions.trans_func_gauss(0.0, sigma);
            // Functions.double_paint_with_func(cartesianChart2, x, x, t_r_f);
        }
        // ставятся начальные значения
        public Form2()
        {
            InitializeComponent();
            Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            tableLayoutPanel1.Width = (int)(resolution.Width * (15.0 / 16.0));
            tableLayoutPanel1.Height = (int)(resolution.Height * (10.0 / 11.0));
            cartesianChart1.Height = (int)(tableLayoutPanel1.Height * tableLayoutPanel1.RowStyles[0].Height / 100);
            cartesianChart1.Width = (int)(tableLayoutPanel1.Width * tableLayoutPanel1.ColumnStyles[0].Width / 100);
            cartesianChart2.Height = (int)(tableLayoutPanel1.Height * tableLayoutPanel1.RowStyles[0].Height / 100);
            cartesianChart2.Width = (int)(tableLayoutPanel1.Width * tableLayoutPanel1.ColumnStyles[1].Width / 100);
            NumOfPoints = (int)Math.Pow(2, trackBar1.Value);
            XStart = Convert.ToDouble(textBox3.Text);
            XEnd = Convert.ToDouble(textBox4.Text);
            sigma = Convert.ToDouble(textBox1.Text);
            sigma1 = Convert.ToDouble(textBox2.Text);
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void elementHost2_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
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
            sigma1 = buf;
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
            NumOfPoints = (int)Math.Pow(2, trackBar1.Value);
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }
    }
}

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
        double Start { get; set; }
        double End { get; set; }
       
        // здесь выполняется все кроме пересоздания массива при
        // изменении числа точек и изменения параметров
        // измение массива х возлагатся на метод, в котором изменяеются его параметры
        void Repaint()
        {
            cartesianChart1.Series = new SeriesCollection();
            cartesianChart2.Series = new SeriesCollection();
            Complex[] f = new Complex[NumOfPoints];
            for (int i = 0; i < NumOfPoints; i++)
            {
                f[i] = new Complex(Functions.func_rect(x[i], Start, End), 0);
            }
            Functions.complex_re_paint(cartesianChart1, x, f);
            Functions.FastDFT(f);
            double koef = f[0].Magnitude;
            Functions.FlipFlop(f);
            x = ArrayBuilder.CreateVector(-Math.PI / ((XEnd - XStart) / NumOfPoints),
                Math.PI / ((XEnd - XStart) / NumOfPoints), NumOfPoints);
            Functions.complex_magnitude_paint(cartesianChart2, x, f, koef);
            Func<double, double> t_r_f = (x) => Functions.trans_func_rect(x, Math.Max(Start, XStart), Math.Min(End, XEnd)) /
                Functions.trans_func_rect(0, Math.Max(Start, XStart), Math.Min(End, XEnd));
            Functions.double_paint_with_func(cartesianChart2, x, x, t_r_f);

        }
        // ставятся начальные значения
        public Form2()
        {
            InitializeComponent();
            Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            tableLayoutPanel1.Width = (int)(resolution.Width * (15.0 / 16.0));
            tableLayoutPanel1.Height = (int)(resolution.Height * (10.0 / 11.0));
            elementHost1.Height = (int)(tableLayoutPanel1.Height * tableLayoutPanel1.RowStyles[0].Height / 100);
            elementHost1.Width = (int)(tableLayoutPanel1.Width * tableLayoutPanel1.ColumnStyles[0].Width / 100);
            elementHost2.Height = (int)(tableLayoutPanel1.Height * tableLayoutPanel1.RowStyles[0].Height / 100);
            elementHost2.Width = (int)(tableLayoutPanel1.Width * tableLayoutPanel1.ColumnStyles[1].Width / 100);
            NumOfPoints = (int)Math.Pow(2, trackBar1.Value);
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
        // дальше проверка длинны это проверка того что ты не удалили все из текст бокса
        // а проверка приведения, это проверка того что в текст боксе число
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
            NumOfPoints = (int)Math.Pow(2, trackBar1.Value);
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }
    }
}

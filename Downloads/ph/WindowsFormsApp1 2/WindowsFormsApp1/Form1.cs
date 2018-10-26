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
        Graphics graphics { get; set; }
        Rectangle moving_length { get; set; }



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
            Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            tableLayoutPanel1.Width = (int)(resolution.Width * (15.0 / 16.0));
            tableLayoutPanel1.Height = (int)(resolution.Height * (10.0 / 11.0)) ;
            elementHost1.Height = (int)(tableLayoutPanel1.Height * tableLayoutPanel1.RowStyles[0].Height / 100);
            elementHost1.Width = (int)(tableLayoutPanel1.Width * tableLayoutPanel1.ColumnStyles[1].Width / 100);
            elementHost2.Height = (int)(tableLayoutPanel1.Height * tableLayoutPanel1.RowStyles[0].Height / 100);
            elementHost2.Width = (int)(tableLayoutPanel1.Width * tableLayoutPanel1.ColumnStyles[2].Width / 100);
            tableLayoutPanel3.Height = (int)(tableLayoutPanel1.Height * tableLayoutPanel1.RowStyles[1].Height);
            tableLayoutPanel3.Width = (int)(tableLayoutPanel1.Width * tableLayoutPanel1.ColumnStyles[2].Width);
            NumOfPoints = (int)Math.Pow(2, trackBar1.Value);
            XStart = Convert.ToDouble(textBox2.Text);
            XEnd = Convert.ToDouble(textBox4.Text);
            sigma = Convert.ToDouble(textBox1.Text);
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            graphics = tableLayoutPanel3.CreateGraphics(); 
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
            NumOfPoints = (int)Math.Pow(2, trackBar1.Value);
            x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            Repaint();
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            SolidBrush black_brush = new SolidBrush(Color.Black);
            SolidBrush blue_brush = new SolidBrush(Color.Blue);
            SolidBrush red_brush = new SolidBrush(Color.Red);
            int base_x = tableLayoutPanel3.Width / 9; // единицы измерения длинны
            int base_y = tableLayoutPanel3.Height / 9; // единицы измерения длинны
            graphics.FillRectangle(black_brush, new Rectangle(4 * base_x, base_y, base_x, base_y / 5));
            graphics.FillRectangle(blue_brush, new Rectangle(4 * base_x, 8 * base_y - base_y / 5, base_x, base_y / 5));
            graphics.FillRectangle(black_brush, new Rectangle(base_x, 4 * base_y, base_x, base_y));
            graphics.FillRectangle(red_brush, new Rectangle(2 * base_x, 4 * base_y + (int)(base_y * 0.4), base_x / 10, base_y / 5));
            graphics.RotateTransform((float)(Math.Atan((double)base_y / (double)base_x) * 180 / Math.PI));
            graphics.FillRectangle(black_brush, new Rectangle((int)Math.Sqrt(Math.Pow(4.5 * base_x, 2) + Math.Pow(4.5 * base_y, 2)) 
                - base_x / 2, 0, base_x, base_y / 5));


        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SolidBrush red_brush = new SolidBrush(Color.Red);
            int base_x = tableLayoutPanel3.Width / 9; // единицы измерения длинны
            int base_y = tableLayoutPanel3.Height / 9; // единицы измерения длинны
            moving_length = new Rectangle(8 * base_x, 4 * base_y, base_x / 5, base_y);
            graphics.FillRectangle(red_brush, moving_length);

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics graphics = tableLayoutPanel3.CreateGraphics();
            SolidBrush white_brush = new SolidBrush(Color.WhiteSmoke);
            SolidBrush red_brush = new SolidBrush(Color.Red);
            graphics.FillRectangle(white_brush, moving_length);
            moving_length = new Rectangle(moving_length.Left - 1, moving_length.Top,
                moving_length.Width, moving_length.Height);
            if (moving_length.Left <= tableLayoutPanel3.Width * 7.0 / 9)
                timer1.Enabled = false;
            graphics.FillRectangle(red_brush, moving_length);
        }
    }
}

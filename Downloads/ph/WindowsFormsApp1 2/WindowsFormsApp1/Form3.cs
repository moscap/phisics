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
using AForge.Math;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        public int NumOfPoints = 1868;
        double XStart { get; set; }
        double XEnd { get; set; }
        double[] x = null;
        double[] x_w = null;
        double sigma_G, omega_G;
        Graphics graphics { get; set; }
        Graphics mirror_graph { get; set; }
        Rectangle moving_length { get; set; }
        Rectangle sample { get; set; }
        Rectangle f_line { get; set; }
        Rectangle f_s_line { get; set; }
        Rectangle s_line { get; set; }
        System.Windows.Forms.DataVisualization.Charting.Series ser { get; set; }
        long tic { get; set; }
        Complex[] Y_c = null;
        double koef { get; set; }
        Complex[] G = null;
        Complex[] G_K = null;
        Complex[] K = null;

        // приводим к нормальному виду
        // здесь выполняется все кроме пересоздания массива при
        // изменении числа точек и изменения параметров
        // измение массива х возлагатся на метод, в котором изменяеются его параметры
        void Initialize_Empty()
        {
            //G = new Complex[NumOfPoints];
            Y_c = new Complex[NumOfPoints];
            x = ArrayBuilder.CreateVector(
                - Math.PI / ((x_w[1867] - x_w[0]) / NumOfPoints),
                Math.PI / ((x_w[1867] - x_w[0]) / NumOfPoints), 
                NumOfPoints);
            XEnd = x[1867];
            XStart = x[0];
            //for (int i = 0; i < NumOfPoints; i++)
            //{
            //    G[i] = new Complex(Functions.func_gauss(x_w[i], sigma_G, omega_G), 0);
            //}
            for (int i = 0; i < NumOfPoints; i++)
            {
                var mag = G[i].Magnitude;
                Y_c[i] = new Complex(mag * mag, 0);
            }
            Functions.DFT(Y_c, 1);
            Complex add_k = new Complex(Y_c[0].Re, 0);
            for (int i = 0; i < NumOfPoints; i++)
            {
                Y_c[i] += add_k;
            }
            Functions.FlipFlop(Y_c);
            koef = Y_c.Max(t => t.Re);
        }
        void Initialize_Filled()
        {

            G_K = new Complex[NumOfPoints];
            for (int i = 0; i < NumOfPoints; i++)
            {
                G_K[i] = Complex.Multiply(K[i], G[i]);
            }
            for (int i = 0; i < NumOfPoints; i++)
            {
                var mag = G_K[i].Magnitude;
                Y_c[i] = new Complex(mag * mag, 0);
            }
            Functions.DFT(Y_c, 1);
            Complex add_k = new Complex(Y_c[0].Re, 0);
            for (int i = 0; i < NumOfPoints; i++)
            {
                Y_c[i] += add_k;
            }
            Functions.FlipFlop(Y_c);
            koef = Y_c.Max(t => t.Re);
        }
        
        void Parse()
        {
            K = new Complex[1868];
            G = new Complex[1868];
            x_w = new double[1868];
            StreamReader file = new StreamReader("./sp1.txt");
            string buf;
            for (int i = 0; (buf = file.ReadLine()) != null; ++i)
            {
                string[] vals = buf.Split(',');
                x_w[i] = Convert.ToDouble(vals[0]);
                K[i] = new Complex(Convert.ToDouble(vals[2]), 0);
                G[i] = new Complex(Convert.ToDouble(vals[1]), 0);
            }
            Complex k_k = new Complex(K.Max(t => t.Re), 0);
            Complex k_g = new Complex(G.Max(t => t.Re), 0);
            for (int i = 0; i < NumOfPoints; ++i)
            {
                K[i] = Complex.Divide(K[i], k_k);
                G[i] = Complex.Divide(G[i], k_g);
            }
            //sigma_G = 350;
            //omega_G = 3200;
        }

        
        public Form3()
        {
            InitializeComponent();
            DoubleBuffered = true;

            chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{F0}";
            chart1.ChartAreas[0].AxisX.Title = "волновое число(см -1)";
            chart1.ChartAreas[0].AxisX.TitleFont = new Font(chart1.ChartAreas[0].AxisX.TitleFont.Name, 14, 
                chart1.ChartAreas[0].AxisX.TitleFont.Style, chart1.ChartAreas[0].AxisX.TitleFont.Unit);

            chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "{F0}";
            chart2.ChartAreas[0].AxisX.Title = "волновое число(см -1)";
            chart2.ChartAreas[0].AxisX.TitleFont = new Font(chart2.ChartAreas[0].AxisX.TitleFont.Name, 14,
                chart2.ChartAreas[0].AxisX.TitleFont.Style, chart2.ChartAreas[0].AxisX.TitleFont.Unit);
            chart2.ChartAreas[0].AxisY.TitleFont = new Font(chart2.ChartAreas[0].AxisY.TitleFont.Name, 14,
                chart2.ChartAreas[0].AxisY.TitleFont.Style, chart2.ChartAreas[0].AxisY.TitleFont.Unit);

            chart3.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            chart3.ChartAreas[0].AxisX.LabelStyle.Format = "{F2}";
            chart3.ChartAreas[0].AxisX.Title = "см";
            chart3.ChartAreas[0].AxisX.TitleFont = new Font(chart3.ChartAreas[0].AxisX.TitleFont.Name, 14,
                chart3.ChartAreas[0].AxisX.TitleFont.Style, chart3.ChartAreas[0].AxisX.TitleFont.Unit);

            graphics = tableLayoutPanel3.CreateGraphics();
            button1.Enabled = false;
            button4.Enabled = false;
            button3.Enabled = false;
            button6.Enabled = false;
            Parse();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            chart2.Series.Clear();
            Functions.complex_re_paint(chart2, x_w, G, 1, sigma_G, omega_G, "G");
            button1.Enabled = false;
            button6.Enabled = true;
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            chart2.ChartAreas[0].AxisY.Title = "";
            chart1.Series.Clear();
            chart2.Series.Clear();
            Initialize_Empty();
            button1.Enabled = false;
            SolidBrush smoke_brush = new SolidBrush(Color.WhiteSmoke);
            if (!moving_length.IsEmpty)
            {
                graphics.FillRectangle(smoke_brush, moving_length);
            }
            if (!sample.IsEmpty)
            {
                graphics.FillRectangle(smoke_brush, sample);
            }
            SolidBrush red_brush = new SolidBrush(Color.Red);
            int base_x = tableLayoutPanel3.Width / 9; // единицы измерения длинны
            int base_y = tableLayoutPanel3.Height / 9; // единицы измерения длинны
            moving_length = new Rectangle(8 * base_x, 4 * base_y, base_x / 5, base_y);
            s_line = new Rectangle((int)(base_x * 21 / 10.0), (int)(base_y * 4.5), moving_length.Left - (int)(base_x * 21 / 10.0), 1);
            graphics.FillRectangle(red_brush, moving_length);
            chart3.Series.Clear();
            ser = chart3.Series.Add("New plot");
            ser.BorderWidth = 2;
            chart3.ChartAreas[0].AxisX.Maximum = 0.1;
            chart3.ChartAreas[0].AxisX.Minimum = -0.1;
            chart3.ChartAreas[0].AxisY.Maximum = 1.2;
            chart3.ChartAreas[0].AxisY.Minimum = 0;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            tic = 0;
            timer1.Interval = Math.Max(1, (int)(1500.0 / NumOfPoints));
            timer1.Enabled = true;
        }

        private void tic_graph()
        {
            int base_x = tableLayoutPanel3.Width / 9; // единицы измерения длинны
            int base_y = tableLayoutPanel3.Height / 9; // единицы измерения длинны
            Graphics graphics = tableLayoutPanel3.CreateGraphics();
            mirror_graph = tableLayoutPanel3.CreateGraphics();
            mirror_graph.TranslateTransform((int)(base_x * 4.5), (int)(base_y * 4.5));
            mirror_graph.RotateTransform(45);
            DoubleBuffered = true;
            SolidBrush white_brush = new SolidBrush(Color.WhiteSmoke);
            SolidBrush red_brush = new SolidBrush(Color.Red);
            SolidBrush black_brush = new SolidBrush(Color.Black);
            SolidBrush green_brush = new SolidBrush(Color.LightGreen);
            SolidBrush yellow_brush = new SolidBrush(Color.Yellow);
            Pen green_pen = new Pen(Color.LightGreen, 3);

            graphics.FillRectangle(white_brush, moving_length);
            mirror_graph.FillRectangle(white_brush, new Rectangle(-base_x / 10, (int)(-base_y / 1.5), base_x / 5, (int)(base_y * 1.5)));
            graphics.FillRectangle(white_brush, f_line);
            graphics.FillRectangle(white_brush, s_line);
            s_line = new Rectangle((int)(base_x * 4.5), (int)(base_y * 4.5) - 3, moving_length.Left - (int)(base_x * 4.5) - 2, 7);
            moving_length = new Rectangle(tableLayoutPanel3.Width * 8 / 9 - (int)(tableLayoutPanel3.Width / 9 * tic / NumOfPoints)
                , moving_length.Top, moving_length.Width, moving_length.Height);

            ser.Points.AddXY(x[tic], Y_c[tic].Re / koef);

            graphics.FillRectangle(red_brush, moving_length);
            graphics.FillRectangle(green_brush, f_s_line);
            graphics.DrawRectangle(green_pen, f_line);
            graphics.DrawRectangle(green_pen, s_line);

            if (!sample.IsEmpty)
            {
                graphics.FillRectangle(yellow_brush, sample);
            }
            mirror_graph.FillRectangle(black_brush, new Rectangle(-base_x / 10, (int)(-base_y / 1.5), base_x / 5, (int)(base_y * 1.5)));

        }

        private void clear_rays()
        {
            int base_x = tableLayoutPanel3.Width / 9; // единицы измерения длинны
            int base_y = tableLayoutPanel3.Height / 9; // единицы измерения длинны
            Graphics graphics = tableLayoutPanel3.CreateGraphics();

            SolidBrush white_brush = new SolidBrush(Color.WhiteSmoke);
            SolidBrush black_brush = new SolidBrush(Color.Black);
            Pen white_pen = new Pen(Color.WhiteSmoke, 3);

            graphics.FillRectangle(white_brush, f_s_line);
            graphics.DrawRectangle(white_pen, f_line);
            graphics.DrawRectangle(white_pen, s_line);
            mirror_graph.FillRectangle(black_brush, new Rectangle(-base_x / 10, (int)(-base_y / 1.5), base_x / 5, (int)(base_y * 1.5)));
            if (!sample.IsEmpty)
            {
                SolidBrush yellow_brush = new SolidBrush(Color.Yellow);
                graphics.FillRectangle(yellow_brush, sample);
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            tic_graph();
            SolidBrush white_brush = new SolidBrush(Color.WhiteSmoke);

            tic += 2;
            if (tic >= NumOfPoints)
            {
                timer1.Enabled = false;
                if (button6.Enabled)
                {
                    button6.Enabled = false;
                    button4.Enabled = true;
                    clear_rays();
                }
                else
                {
                    button1.Enabled = true;
                    clear_rays();
                }
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {
            DoubleBuffered = true;
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
            f_line = new Rectangle((int)(base_x * 4.5) - 3, (int)(base_y * 6 / 5.0) + 1, 7, (int)(8 * base_y - base_y / 5) - (int)(base_y * 6 / 5.0) - 3);
            f_s_line = new Rectangle(base_x * 21 / 10, (int)(base_y * 4.5), (int)(base_x * 4.5) - base_x * 21 / 10, 3);
            graphics.TranslateTransform((int)(base_x * 4.5), (int)(base_y * 4.5));
            graphics.RotateTransform(45);
            mirror_graph = graphics;
            graphics.FillRectangle(black_brush, new Rectangle(-base_x / 20, (int)(-base_y / 1.5), base_x / 10, (int)(base_y * 1.5)));
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //button6.Enabled = false;
            Functions.complex_re_paint(chart1, x_w, G, 1, sigma_G, omega_G, "G");
            SolidBrush yellow_brush = new SolidBrush(Color.Yellow);
            int base_x = tableLayoutPanel3.Width / 9; // единицы измерения длинны
            int base_y = tableLayoutPanel3.Height / 9; // единицы измерения длинны
            sample = new Rectangle(4 * base_x, 6 * base_y, base_x, base_y);
            graphics.FillRectangle(yellow_brush, sample);
            chart2.Series.Clear();
            chart3.Series.Clear();
            ser = chart3.Series.Add("Acorr");
            ser.BorderWidth = 3;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            Initialize_Filled();
            tic = 0;
            timer1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Functions.complex_re_paint(chart2, x_w, G_K, 1, sigma_G, omega_G, "GK");
            button4.Enabled = false;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Functions.complex_re_paint(chart1, x_w, G_K, 1, sigma_G, omega_G, "GK");
            chart2.Series.Clear();
            Functions.complex_re_paint(chart2, x_w, K, 1, 1, 1, "K");
            button3.Enabled = false;
        }

        private void tableLayoutPanel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

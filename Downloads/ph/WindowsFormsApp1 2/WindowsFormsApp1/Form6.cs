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
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form6 : Form
    {
        public int NumOfPoints;
        double XStart;
        double XEnd;
        int step;
        double[] x = null;
        double[] x_w = null;
        double[] x_w_full = null;
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
        Complex[] Y_c_n = null;
        double koef { get; set; }
        Complex[] K = null;
        Complex[] K_full = null;
        bool FirstTime = true;
        // Created full (501) K, x_w_full
        void Whole_Parse()
        {
            NumOfPoints = 501; //  A constant: very bad!!
            K_full = new Complex[501];
            x_w_full = new double[501];
            StreamReader file = new StreamReader("./sun_spectrum.csv");
            string buf;
            double x_val = 0;
            int k = 0;
            while ((buf = file.ReadLine()) != null)
            {
                string[] vals = buf.Split(',');
                x_val = Convert.ToDouble(vals[0]);
                if (x_val >= 350 && x_val <= 800)
                {
                    x_w_full[k] = x_val;
                    K_full[k] = new Complex(Convert.ToDouble(vals[1]), 0);
                    k += 1;
                }
            }
            double[] new_x = new double[x_w_full.Length];
            for (int i = 0; i < K_full.Length; i++)
            {
                new_x[i] = Math.Pow(10, 7) / x_w_full[i];
            }
            x_w_full = new_x;
            for (int i = 0; i <= (x_w_full.Length / 2); i++)
            {
                double tmp = x_w_full[i];
                x_w_full[i] = x_w_full[x_w_full.Length - i - 1];
                x_w_full[x_w_full.Length - i - 1] = tmp;
                Complex ctmp = K_full[i];
                K_full[i] = K_full[x_w_full.Length - i - 1];
                K_full[x_w_full.Length - i - 1] = ctmp;
            }
            int length_x_w = x_w_full.Length;
            x = ArrayBuilder.CreateVector(
                -0.5 / ((x_w_full[length_x_w - 1] - x_w_full[0]) / length_x_w),
                0.5 / ((x_w_full[length_x_w - 1] -x_w_full[0]) / length_x_w),
                length_x_w);
            XEnd = x[length_x_w - 1];
            XStart = x[0];
            //x = ArrayBuilder.CreateVector(
            //    -0.5 / ((x_w_full[NumOfPoints - 1] - x_w_full[0]) / NumOfPoints),
            //    0.5 / ((x_w_full[NumOfPoints - 1] - x_w_full[0]) / NumOfPoints),
            //    NumOfPoints);
            //XStart = x[0];
            //XEnd = x[NumOfPoints - 1];
            // Console.WriteLine(k);
        }

        void Initialize_Filled()
        {
            Y_c = new Complex[K_full.Length];
            for (int i = 0; i < K_full.Length; i++)
            {
                var mag = K_full[i].Magnitude;
                Y_c[i] = new Complex(mag * mag, 0);
            }
            Functions.DFT(Y_c, 1);
            Complex add_k = new Complex(Y_c[0].Re, 0);
            for (int i = 0; i < Y_c.Length; i++)
            {
                Y_c[i] += add_k;
            }
            Functions.FlipFlop(Y_c);
            koef = Y_c.Max(x => x.Re);
        }

        void Initialize_New_Data()
        {
            NumOfPoints = 0;
            for (int i = 0; i < x_w_full.Length; i += step)
            {
                NumOfPoints += 1;
            }
            x_w = new double[NumOfPoints];
            K = new Complex[NumOfPoints];
            for (int i = 0, j = 0; i < x_w_full.Length; i += step, j += 1)
            {
                K[j] = K_full[i]; 
                x_w[j] = x_w_full[i];
            }
            int length_x_w = x_w.Length;
            Y_c = new Complex[length_x_w];
            x = ArrayBuilder.CreateVector(
                -0.5 / ((x_w[length_x_w - 1] - x_w[0]) / length_x_w),
                0.5 / ((x_w[length_x_w - 1] - x_w[0]) / length_x_w),
                length_x_w);
            XEnd = x[length_x_w - 1];
            XStart = x[0];
            for (int i = 0; i < K.Length; i++)
            {
                var mag = K[i].Magnitude;
                Y_c[i] = new Complex(mag * mag, 0);
            }
            Functions.DFT(Y_c, 1);
            Complex add_k = new Complex(Y_c[0].Re, 0);
            for (int i = 0; i < Y_c.Length; i++)
            {
                Y_c[i] += add_k;
            }
            Functions.FlipFlop(Y_c);
            koef = Y_c.Max(t => t.Re);
        }

        public Form6()
        {
            InitializeComponent();
            Timer timer1 = new Timer();
            this.ControlBox = false;
            this.WindowState = FormWindowState.Maximized;

            chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            chart1.ChartAreas[0].Name = "ChartArea0";
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{F0}";
            chart1.ChartAreas[0].AxisX.Title = "волновое число(см -1)";
            chart1.ChartAreas[0].AxisX.TitleFont = new Font(chart1.ChartAreas[0].AxisX.TitleFont.Name, 14,
                chart1.ChartAreas[0].AxisX.TitleFont.Style, chart1.ChartAreas[0].AxisX.TitleFont.Unit);

            chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            chart2.ChartAreas[0].Name = "ChartArea0";
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "{F0}";
            chart2.ChartAreas[0].AxisX.Title = "волновое число(см -1)";
            chart2.ChartAreas[0].AxisX.TitleFont = new Font(chart2.ChartAreas[0].AxisX.TitleFont.Name, 14,
                chart2.ChartAreas[0].AxisX.TitleFont.Style, chart2.ChartAreas[0].AxisX.TitleFont.Unit);

            chart3.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            chart3.ChartAreas[0].Name = "ChartArea0";
            chart3.ChartAreas[0].AxisX.LabelStyle.Format = "{F3}";
            chart3.ChartAreas[0].AxisX.Title = "см";
            chart3.ChartAreas[0].AxisX.TitleFont = new Font(chart3.ChartAreas[0].AxisX.TitleFont.Name, 14,
                chart3.ChartAreas[0].AxisX.TitleFont.Style, chart3.ChartAreas[0].AxisX.TitleFont.Unit);

            trackBar2_Scroll(this, new EventArgs());
            trackBar1_Scroll(this, new EventArgs());
            // x = ArrayBuilder.CreateVector(XStart, XEnd, NumOfPoints);
            graphics = tableLayoutPanel3.CreateGraphics();
            button1.Enabled = false;
            Whole_Parse();
            x_w = x_w_full;
            K = K_full;
            Initialize_Filled();
            string interv = String.Format("Интервал: {0:0.00000}", XEnd - XStart);
            label1.Text = interv;
            FirstTime = false;
            chart1.Titles.Clear();
            Title tit = chart1.Titles.Add("Спектр солнца");
            tit.Font = new System.Drawing.Font("Arial", 14);
            chart1.Titles[0].Visible = true;
            tit.DockedToChartArea = "ChartArea0";
            Functions.complex_re_paint_min_max(chart1, x_w, K, name: "Sun spectrum wave number");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
            moving_length = new Rectangle(tableLayoutPanel3.Width * 8 / 9 - (int)(tableLayoutPanel3.Width / 9 * tic / NumOfPoints)
                , moving_length.Top, moving_length.Width, moving_length.Height);
            s_line = new Rectangle((int)(base_x * 4.5), (int)(base_y * 4.5) - 3, moving_length.Left - (int)(base_x * 4.5) - 2, 7);

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

            if (NumOfPoints > 300)
                tic += 2;
            else
                tic++;

            if (tic >= NumOfPoints)
            {
                timer1.Enabled = false;
                button1.Enabled = true;
                clear_rays();
            }
        }


        private void Form6_Load(object sender, EventArgs e)
        {

        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            // TODO Scroll, deleted
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            step = (int)trackBar1.Value;
            if (FirstTime != true)
            {
                Initialize_New_Data();
                string interv = String.Format("Интервал: {0:0.00000}", XEnd - XStart);
                label1.Text = interv;
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Close();
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
            graphics.FillRectangle(red_brush, new Rectangle(8 * base_x, 4 * base_y, base_x / 5, base_y));
            f_line = new Rectangle((int)(base_x * 4.5) - 3, (int)(base_y * 6 / 5.0) + 1, 7, (int)(8 * base_y - base_y / 5) - (int)(base_y * 6 / 5.0) - 3);
            f_s_line = new Rectangle(base_x * 21 / 10, (int)(base_y * 4.5), (int)(base_x * 4.5) - base_x * 21 / 10, 3);
            graphics.TranslateTransform((int)(base_x * 4.5), (int)(base_y * 4.5));
            graphics.RotateTransform(45);
            graphics.FillRectangle(black_brush, new Rectangle(-base_x / 10, (int)(-base_y / 1.5), base_x / 5, (int)(base_y * 1.5)));
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics graphics = tableLayoutPanel3.CreateGraphics();
            DoubleBuffered = true;
            chart2.Titles[0].Visible = false;
            chart2.ChartAreas[0].AxisY.Title = "";
            chart2.Series.Clear();
            // Initialize_New_Data();
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
            chart3.ChartAreas[0].AxisX.Maximum = XEnd;
            chart3.ChartAreas[0].AxisX.Minimum = XStart;
            chart3.ChartAreas[0].AxisY.Maximum = 1.2;
            chart3.ChartAreas[0].AxisY.Minimum = 0;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            tic = 0;
            timer1.Interval = Math.Max(1, (int)(1500.0 / NumOfPoints));
            timer1.Enabled = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart2.Titles.Clear();
            Title tit = chart2.Titles.Add("Измеренный спектр солнца");
            tit.Font = new System.Drawing.Font("Arial", 14);
            chart2.Titles[0].Visible = true;
            tit.DockedToChartArea = "ChartArea0";
            chart2.Series.Clear();
            Functions.complex_re_paint_min_max(chart2, x_w, K, name: "K");
            button1.Enabled = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}

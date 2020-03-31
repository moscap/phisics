using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Math;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Additional_Sun : Form
    {
        Complex[] Y_c = null;
        double[] x_w = null;
        Complex[] K = null;
        bool first_time = true;

        void Parse()
        {
            K = new Complex[501];
            x_w = new double[501];
            StreamReader file = new StreamReader("./sun_spectrum.csv");
            string buf;
            double x = 0;
            int k = 0;
            while ((buf = file.ReadLine()) != null)
            {
                string[] vals = buf.Split(',');
                x = Convert.ToDouble(vals[0]);
                if (x >= 350 && x <= 800)
                {
                    x_w[k] = x;
                    K[k] = new Complex(Convert.ToDouble(vals[1]), 0);
                    k += 1;
                }               
            }
            // Console.WriteLine(k);
        }

        void Initialize_Filled()
        {
            Y_c = new Complex[K.Length];
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
        }

        public Additional_Sun()
        {
            InitializeComponent();

            Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            tableLayoutPanel1.Width = (int)(resolution.Width * (15.0 / 16.0));
            tableLayoutPanel1.Height = (int)(resolution.Height * (10.0 / 11.0));
            this.WindowState = FormWindowState.Maximized;

            chart1.Series.Clear();
            chart1.Size = new Size((int)(resolution.Width * (15.0 / 16.0)),
                                   (int)(resolution.Height * (10.0 / 11.0)));
            chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{F0}";
            chart1.ChartAreas[0].AxisX.Title = "длина волны(нм)";
            chart1.ChartAreas[0].AxisX.TitleFont = new Font(chart1.ChartAreas[0].AxisX.TitleFont.Name, 14,
                chart1.ChartAreas[0].AxisX.TitleFont.Style, chart1.ChartAreas[0].AxisX.TitleFont.Unit);
            
            Parse();
            Initialize_Filled();
            Functions.complex_re_paint_min_max(chart1, x_w, K, name: "Sun sprectrum nm");
            first_time = true;
        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chart1_Click_1(object sender, EventArgs e)
        {
            if (first_time)
            {
                first_time = false;
                double[] new_x = new double[501];
                for (int i = 0; i < K.Length; i++)
                {
                    new_x[i] = Math.Pow(10, 7) / x_w[i];
                }
                x_w = new_x;
                
                chart1.ChartAreas[0].AxisX.Title = "волновое число(см -1)";
                Functions.complex_re_paint_min_max(chart1, x_w, K, name: "Sun spectrum wave number");
            }
            else
            {
                Sun form6 = new Sun();
                form6.Show();
            }
        }
    }
}

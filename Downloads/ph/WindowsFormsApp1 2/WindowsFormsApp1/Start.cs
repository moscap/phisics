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
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            Bitmap bim = new Bitmap("./vmk.png");
            bim = new Bitmap(bim, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bim;

            bim = new Bitmap("./ff.jpeg");
            bim = new Bitmap(bim, pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Image = bim;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Principles form1 = new Principles();
            form1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Water form3 = new Water();
            form3.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Char_submenu form5 = new Char_submenu();
            form5.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Authors f = new Authors();
            f.Show();
        }

    }
}

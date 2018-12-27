using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            Bitmap bim = new Bitmap("./kos.jpg");
            bim = new Bitmap(bim, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bim;

            bim = new Bitmap("./kon.jpg");
            bim = new Bitmap(bim, pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Image = bim;

            bim = new Bitmap("./vmk.png");
            bim = new Bitmap(bim, pictureBox3.Width, pictureBox3.Height);
            pictureBox3.Image = bim;

            bim = new Bitmap("./ff.jpeg");
            bim = new Bitmap(bim, pictureBox4.Width, pictureBox4.Height);
            pictureBox4.Image = bim;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

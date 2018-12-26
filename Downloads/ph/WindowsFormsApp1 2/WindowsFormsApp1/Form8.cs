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

            Bitmap bim = new Bitmap("./kos.jpg");
            bim = new Bitmap(bim, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bim;

            bim = new Bitmap("./kon.jpg");
            bim = new Bitmap(bim, pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Image = bim;
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

        }
    }
}

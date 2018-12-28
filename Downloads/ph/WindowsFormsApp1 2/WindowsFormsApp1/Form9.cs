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
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            Bitmap bim = new Bitmap("./sp.jpg");
            bim = new Bitmap(bim, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bim;
        }
    }
}

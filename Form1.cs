using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicturesFitting
{
    public partial class Form1 : Form
    {
        Row row1 = new Row();
        public Form1()
        {
            InitializeComponent();
            row1.Add("1.jpg").Add("4.jpg").Add("2.jpg").Add("3.jpg");
            pictureBox1.Image = row1.GetTreeImages();
             

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                pictureBox1.Image.Save("output.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                MessageBox.Show("Сохранено в debug");
            }
            else
            {
                MessageBox.Show("Нет картинки");
            }
        }

        private void resizeButton_Click(object sender, EventArgs e)
        {
            int w = (int)numericUpDown1.Value;
            pictureBox1.Image =  row1.ResizeImages(w);
        }
    }
}

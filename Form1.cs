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
        Column column1 = new Column();
        Row row2 = new Row();
        public Form1()
        {
            InitializeComponent();
            //порядок не имеет значения
            row2.Add("4.jpg").Add("5.jpg").Add("3.jpg");
            column1.Add("2.jpg").Add(row2);
            row1.Add("1.jpg").Add(column1).Add("6.jpg");
            
            
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
            pictureBox1.Image =  row1.ResizeImages(4000);
        }
    }
}

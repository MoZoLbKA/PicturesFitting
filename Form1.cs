using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PicturesFitting
{
    public enum PaddingImages
    {
        Top,
        Left,
        Right,
        Bottom
    }
    public partial class Form1 : Form
    {
        private Row row1 = new Row();
        private Column column1 = new Column();
        private Row row2 = new Row();
        private Row row3 = new Row();
        private Column column2 = new Column();
        public Form1()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
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
            row2.Add("4.jpg").Add(column2);
            column1.Add(row2).Add("6.jpg").Add(row3);
            row1.Add("2.jpg").Add(column1).Add("3.jpg");
            column2.Add(row3).Add("2.jpg");
            row3.Add("5.jpg").Add("1.jpg");
            var paddings = new Dictionary<PaddingImages, int>() { { PaddingImages.Right, 20 },
                    { PaddingImages.Left, 20 },
                    { PaddingImages.Top, 20 },
                    { PaddingImages.Bottom, 20 }, };

            pictureBox1.Image = row1.DrawStoryBoard(4000,paddings);
        }
    }
}

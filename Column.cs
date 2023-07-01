using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicturesFitting
{
    internal class Column
    {
        List<Bitmap> data = new List<Bitmap>();
        Dictionary<Row,int> rows = new Dictionary<Row, int>();
        const int DEAFAULT_WIDTH = 1000;

        public Bitmap compiledColumn { get; private set; }
        private Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = File.Open(fileName, FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
            }
            return bitmap;
        }
        public Column Add(string frame)
        {
            data.Add(ConvertToBitmap(frame));
            return this;
        }
        public Column Add(Row frame)
        {
            rows.Add(frame,data.Count);
            return this;
        }
        public Bitmap GetTreeImages()
        {
            compiledColumn = MergeImages(data);
            return compiledColumn;
        }

        private Bitmap MergeImages(IEnumerable<Bitmap> images)
        {
            var enumerable = images as IList<Bitmap> ?? images.ToList();

            var width = 0;
            var height = 0;

            foreach (var image in enumerable)
            {
                height += image.Height;
                width = image.Width > width
                    ? image.Width
                    : width;
            }

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                var localHeight = 0;
                foreach (var image in enumerable)
                {
                    g.DrawImage(image, 0, localHeight);
                    localHeight += image.Height;
                }
            }
            compiledColumn = bitmap;
            return bitmap;
        }
        private Bitmap ResizeImage(Bitmap img, Size size)
        {
            Bitmap b = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, size.Width, size.Height);
            }
            return b;
        }
        internal Bitmap ResizeImages(int width)
        {
            foreach (var item in rows)
            {
                data.Insert(item.Value, item.Key.ResizeImages(width));
            }
            //for (int i = 0; i < rows.Count; i++)
            //{
            //    data.Add(rows[i].ResizeImages(width));
            //    rows.RemoveAt(i);
            //}
            if (data == null)
            {
                return null;
            }
            List<int> heights = new List<int>(data.Count);
            List<int> widths = new List<int>(data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                heights.Add(data[i].Height);
                widths.Add(data[i].Width);
            }
            for (int i = 0; i < data.Count; i++)
            {
                double ratio = (double)widths[i] / heights[i];
                int reduct = widths[i] - width;
                widths[i] -= reduct;
                heights[i] = (int)(widths[i] / ratio);
            }
            List<Bitmap> compression = new List<Bitmap>();
            for (int i = 0; i < data.Count; i++)
            {
                compression.Add(ResizeImage(data[i], new Size(widths[i], heights[i])));
            }
            Bitmap tmp = MergeImages(compression);
            double coef = width  / tmp.Width;
            compiledColumn = ResizeImage(tmp, new Size(width, (int)(tmp.Height * coef)));
            return compiledColumn;
        }
    }
}


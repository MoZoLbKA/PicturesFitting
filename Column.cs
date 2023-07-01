using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
namespace PicturesFitting
{
    internal class Column
    {
        List<Bitmap> data = new List<Bitmap>();
        Dictionary<Row,int> rows = new Dictionary<Row, int>();

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

        private Bitmap MergeImages(IEnumerable<Bitmap> images, int marginTop, int marginRight, int marginBot, int marginLeft)
        {
            var enumerable = images as IList<Bitmap> ?? images.ToList();

            var width = 0;
            var height = 0;

            foreach (var image in enumerable)
            {
                height += image.Height+marginTop+marginBot;
                width = image.Width > width
                    ? image.Width
                    : width;
            }
            width +=marginLeft + marginRight;
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                var localHeight = 0;
                foreach (var image in enumerable)
                {
                    g.DrawImage(image, marginLeft, localHeight);
                    localHeight += image.Height+marginTop+marginBot;
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
        internal Bitmap ResizeImages(int width, 
            int marginTop = 0,int 
            marginRight = 0,
            int marginBot = 0,
            int marginLeft = 0)
        {
            foreach (var item in rows)
            {
                data.Insert(item.Value, item.Key.ResizeImages(width));
            }
            
            if (data == null || data.Count == 0)
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

            Bitmap tmp = MergeImages(compression, marginTop, marginRight, marginBot, marginLeft);
            double coef = width  / tmp.Width;
            compiledColumn = ResizeImage(tmp, new Size(width, (int)(tmp.Height * coef)));
            return compiledColumn;
        }
    }
}


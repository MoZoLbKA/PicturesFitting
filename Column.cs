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

        public Bitmap compiledRow { get; private set; }
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
            return this;
        }
        public Bitmap GetTreeImages()
        {
            compiledRow = MergeImages(data);
            return compiledRow;
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
            compiledRow = bitmap;
            return bitmap;
        }
        private int Sum(List<int> array)
        {
            int sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum;
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
        internal Bitmap ResizeImages(int w)
        {
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
                double ratio = widths[i] / heights[i];
                int reduct = widths[i] - w;
                widths[i] -= reduct;
                heights[i] = (int)(widths[i] / ratio);
            }
            List<Bitmap> compression = new List<Bitmap>();
            for (int i = 0; i < data.Count; i++)
            {
                compression.Add(ResizeImage(data[i], new Size(widths[i], heights[i])));
            }
            Bitmap tmp = MergeImages(compression);
            double coef = w / tmp.Width;
            return ResizeImage(tmp, new Size(w, (int)(tmp.Height * coef)));
        }
    }
}


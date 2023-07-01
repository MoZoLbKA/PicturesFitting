﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace PicturesFitting
{
    internal class Row
    {
        private List<Bitmap> data = new List<Bitmap>();
        public Dictionary<Column,int> columns = new Dictionary<Column, int>();
        public Bitmap rawImages { get; private set; }
        public Bitmap compression { get; private set; }
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

        public Row Add(string frame)
        {
            data.Add(ConvertToBitmap(frame));
            return this;
        }

        public Row Add(Column frame)
        {
            columns.Add(frame,data.Count);
            return this;
        }

        private Bitmap MergeImages(IEnumerable<Bitmap> images)
        {
            var enumerable = images as IList<Bitmap> ?? images.ToList();

            var width = 0;
            var height = 0;

            foreach (var image in enumerable)
            {
                width += image.Width;
                height = image.Height > height
                    ? image.Height
                    : height;
            }

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                var localWidth = 0;
                foreach (var image in enumerable)
                {
                    g.DrawImage(image, localWidth, 0);
                    localWidth += image.Width;
                }
            }
            rawImages = bitmap;
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

        internal Bitmap ResizeImages(int width)
        {
            foreach (var item in columns)
            {
                data.Insert(item.Value,item.Key.ResizeImages(width));
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

            int sumOfWidths = Sum(widths);
            double coeff = (double)sumOfWidths / width;

            for (int i = 0; i < data.Count; i++)
            {
                double ratio = (double)widths[i] / heights[i];
                widths[i] = (int)(widths[i] / coeff);
                heights[i] = (int)(widths[i] / ratio);
            }

            int min = heights.Min();

            for (int i = 0; i < data.Count; i++)
            {
                double ratio = (double)widths[i] / heights[i];
                int reduct = heights[i] - min;
                heights[i] -= reduct;
                widths[i] = (int)(heights[i] * ratio);
            }
            List<Bitmap> compression = new List<Bitmap>();
            for (int i = 0; i < data.Count; i++)
            {
                compression.Add(ResizeImage(data[i], new Size(widths[i], heights[i])));
            }

            Bitmap tmp = MergeImages(compression);
            double coef = width / tmp.Width;
            data = new List<Bitmap>();
            this.compression = ResizeImage(tmp, new Size(width, (int)(tmp.Height * coef)));
            data.Add(this.compression);
            return this.compression;
        }
    }
}

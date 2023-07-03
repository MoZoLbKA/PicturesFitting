using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
namespace PicturesFitting
{
    internal class Column:DataCollection
    {
        Dictionary<Row,int> rows = new Dictionary<Row, int>();
        public Bitmap compiledColumn { get; private set; }

        public Column Add(Row frame)
        {
            rows.Add(frame, data.Count);
            return this;
        }
        public Column Add(string frame)
        {
            data.Add(ConvertToBitmap(frame));
            return this;
        }
        private  Bitmap  MergeImages(IEnumerable<Bitmap> images, Dictionary<PaddingImages,int> paddings,double coef)
        {
            var enumerable = images as IList<Bitmap> ?? images.ToList();
            
            var width = 0;
            var height = 0;
            var curPaddings = NormilizePaddings(paddings, coef);
            foreach (var image in enumerable)
            {
                height += image.Height+ curPaddings[PaddingImages.Top] + curPaddings[PaddingImages.Bottom];
                width = image.Width > width
                    ? image.Width
                    : width;
            }
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                var localHeight = curPaddings[PaddingImages.Top];
                foreach (var image in enumerable)
                {
                    g.DrawImage(image, curPaddings[PaddingImages.Left], localHeight);
                    localHeight += image.Height + curPaddings[PaddingImages.Top] + curPaddings[PaddingImages.Bottom];
                }
            }
            compiledColumn = bitmap;
            return bitmap;
        }
        private void GetSizesOfImage(List<int> heights, List<int> widths, int width)
        {
            for (int i = 0; i < data.Count; i++)
            {
                double ratio = (double)widths[i] / heights[i];
                int reduct = widths[i] - width;
                widths[i] -= reduct;
                heights[i] = (int)(widths[i] / ratio);
            }
        }
        internal Bitmap DrawStoryBoard(int width,
            Dictionary<PaddingImages, int> paddings)
        {
            CheckPaddings(paddings);
            foreach (var item in rows)
            {
                data.Insert(item.Value, item.Key.DrawStoryBoard(width,paddings));
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
            GetSizesOfImage(heights, widths, width);
            return GetCompressionOfData(widths, heights, width, paddings);
        }
        private Bitmap GetCompressionOfData(List<int> widths, List<int> heights, int width,
            Dictionary<PaddingImages, int> paddings)
        {
            List<Bitmap> compression = new List<Bitmap>();

            for (int i = 0; i < data.Count; i++)
            {
                compression.Add(ResizeImage(data[i], new Size(widths[i], heights[i])));
            }
            double coef = (double)width / compression.Max(x => x.Height);
            Bitmap tmp = MergeImages(compression, paddings, coef);
            coef = (double)width / tmp.Width;
            return ResizeImage(tmp, new Size(width, (int)(tmp.Height * coef)));
        }
    }
}


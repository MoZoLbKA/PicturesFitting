using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PicturesFitting
{
    internal class Row:DataCollection
    {
        private Dictionary<Column,int> columns = new Dictionary<Column, int>();
        
        public Row Add(Column frame)
        {
            columns.Add(frame, data.Count);
            return this;
        }

        public Row Add(string frame)
        {
            data.Add(ConvertToBitmap(frame));
            return this;
        }
        
        private Bitmap MergeImages(IEnumerable<Bitmap> images, Dictionary<PaddingImages, int> paddings, double coef)
        {
            var enumerable = images as IList<Bitmap> ?? images.ToList();

            var width = 0;
            var height = 0;
            var curPaddings = NormilizePaddings(paddings, coef);
            foreach (var image in enumerable)
            {
                width += (image.Width + curPaddings[PaddingImages.Right] + curPaddings[PaddingImages.Left]);
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
                    g.DrawImage(image, localWidth, curPaddings[PaddingImages.Top]);
                    localWidth += (image.Width + curPaddings[PaddingImages.Right]+ curPaddings[PaddingImages.Left]);
                }
            }
            return bitmap;
        }

        private void GetSizesOfImage(List<int> heights, List<int> widths,double coeff)
        {
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
        }

        internal Bitmap DrawStoryBoard(int width, Dictionary<PaddingImages, int> paddings = null)
        {
            CheckPaddings(paddings);
            foreach (var item in columns)
            {
                data.Insert(item.Value,item.Key.DrawStoryBoard(width,paddings));
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
            GetSizesOfImage(heights, widths, coeff);
            return GetCompressionOfData(widths,heights,width,paddings);
        }

        private Bitmap GetCompressionOfData(List<int> widths, List<int> heights,int width,
            Dictionary<PaddingImages,int> paddings)
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

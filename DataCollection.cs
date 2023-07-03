
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;


namespace PicturesFitting
{
    internal abstract class DataCollection
    {

        public virtual List<Bitmap> data { get; private set; } = new List<Bitmap>();
        public Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = File.Open(fileName, FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
            }
            return bitmap;
        }
        public Dictionary<PaddingImages, int> NormilizePaddings(Dictionary<PaddingImages, int> paddings, double coef)
        {
            return new Dictionary<PaddingImages, int>() {
                { PaddingImages.Top ,(int)(paddings[PaddingImages.Top] /coef )},
                { PaddingImages.Bottom ,(int)(paddings[PaddingImages.Bottom] / coef )},
                { PaddingImages.Left ,(int)(paddings[PaddingImages.Left] / coef ) },
                { PaddingImages.Right ,(int)(paddings[PaddingImages.Right] / coef )},
        };
        }
        public virtual Bitmap ResizeImage(Bitmap img, Size size)
        {
            Bitmap b = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, size.Width, size.Height);
            }
            return b;
        }
        
        public void CheckPaddings(Dictionary<PaddingImages, int> paddings)
        {
            if (paddings == null)
            {
                paddings = new Dictionary<PaddingImages, int>() { { PaddingImages.Right, 0 },
                    { PaddingImages.Left, 0 },
                    { PaddingImages.Top, 0 },
                    { PaddingImages.Bottom, 0 }, };
            }
        }
        public int Sum(List<int> array)
        {
            int sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum;
        }
    }
}

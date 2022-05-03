using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace JA_PROJ
{
    class BitmapByteConverter
    {
        public static byte[] BitmapToByte(Bitmap img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static byte[] BitmapExtractByte(Bitmap img)
        {
            byte[] ret = new byte[img.Width * img.Height * 3];

            int iterator = 0;

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);

                    ret[iterator] = pixel.R;
                    ret[iterator + 1] = pixel.G;
                    ret[iterator + 2] = pixel.B;

                    iterator = iterator + 3;
                }
            }

            return ret;
        }

        public static Bitmap BitmapFromPixels(byte[] img, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            int iterator = 0;

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {

                    Color tempPixel = System.Drawing.Color.FromArgb(img[iterator], img[iterator + 1], img[iterator + 2]);

                    bitmap.SetPixel(i, j, tempPixel);

                    iterator = iterator + 3;
                }
            }

            return bitmap;
        }

        public static Bitmap ByteToBitmap(byte[] source)
        {
            MemoryStream ms = new MemoryStream(source);
            Image ret = Image.FromStream(ms);

            return (Bitmap)ret;
        }
    }
}

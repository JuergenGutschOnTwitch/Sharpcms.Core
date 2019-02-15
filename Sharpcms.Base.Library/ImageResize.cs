// sharpcms is licensed under the open source license GPL - GNU General Public License.
//Credit: http://www.codeproject.com/csharp/imageresize.asp

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Sharpcms.Base.Library
{
    public static class ImageResize
    {
        #region AnchorPosition enum

        public enum AnchorPosition
        {
            Top,
            Center,
            Bottom,
            Left,
            Right
        }

        #endregion

        #region Dimensions enum

        public enum Dimensions
        {
            Width,
            Height
        }

        #endregion

        public static Image ScaleByPercent(Image imgPhoto, int percent)
        {
            double nPercent = ((double) percent/100);
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            const int sourceX = 0;
            const int sourceY = 0;
            const int destX = -1;
            const int destY = -1;
            int destWidth = (int) (sourceWidth*nPercent);
            int destHeight = (int) (sourceHeight*nPercent);

            Bitmap bitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(imgPhoto,
                              new Rectangle(destX, destY, destWidth + 2, destHeight + 2),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            graphics.Dispose();

            return bitmap;
        }

        public static Image ConstrainProportions(Image imgPhoto, int size, Dimensions dimension)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            const int sourceX = 0;
            const int sourceY = 0;
            const int destX = -1;
            const int destY = -1;
            double nPercent;

            switch (dimension)
            {
                case Dimensions.Width:
                    nPercent = (size/(double) sourceWidth);
                    break;
                default:
                    nPercent = (size/(double) sourceHeight);
                    break;
            }

            int destWidth = (int) (sourceWidth*nPercent);
            int destHeight = (int) (sourceHeight*nPercent);

            Bitmap bitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(imgPhoto,
                              new Rectangle(destX, destY, destWidth + 2, destHeight + 2),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            graphics.Dispose();

            return bitmap;
        }

        public static Image FixedSize(Image imgPhoto, int width, int height, Color color)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            const int sourceX = 0;
            const int sourceY = 0;
            int destX = 0;
            int destY = 0;
            decimal nPercent;
            decimal nPercentW = (width/(decimal) sourceWidth);
            decimal nPercentH = (height/(decimal) sourceHeight);

            //if we have to pad the height pad both the top and the bottom
            //with the difference between the scaled height and the desired height
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = (int) Math.Round((width - (sourceWidth*nPercent))/2, 0);
            }
            else
            {
                nPercent = nPercentW;
                destY = (int) Math.Round((height - (sourceHeight*nPercent))/2, 0);
            }

            int destWidth = (int) Math.Round(sourceWidth*nPercent);
            int destHeight = (int) Math.Round(sourceHeight*nPercent);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(color);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(imgPhoto,
                              new Rectangle(destX, destY, destWidth, destHeight),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            graphics.Dispose();

            return bitmap;
        }

        public static Image Crop(Image imgPhoto, int width, int height, AnchorPosition anchor)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            const int sourceX = 0;
            const int sourceY = 0;
            int destX = -1;
            int destY = -1;
            double nPercent;
            double nPercentW = (width/(double) sourceWidth);
            double nPercentH = (height/(double) sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (anchor)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = (int) (height - (sourceHeight*nPercent));
                        break;
                    default:
                        destY = (int) ((height - (sourceHeight*nPercent))/2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (anchor)
                {
                    case AnchorPosition.Left:
                        destX = 0;
                        break;
                    case AnchorPosition.Right:
                        destX = (int) (width - (sourceWidth*nPercent));
                        break;
                    default:
                        destX = (int) ((width - (sourceWidth*nPercent))/2);
                        break;
                }
            }

            int destWidth = (int) (sourceWidth*nPercent);
            int destHeight = (int) (sourceHeight*nPercent);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(imgPhoto,
                              new Rectangle(destX, destY, destWidth + 2, destHeight + 2),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            graphics.Dispose();

            return bitmap;
        }
    }
}
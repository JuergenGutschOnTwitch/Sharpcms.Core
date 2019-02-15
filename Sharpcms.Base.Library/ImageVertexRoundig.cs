// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Sharpcms.Base.Library
{
    public static class ImageVertexRounding
    {
        public static Image RoundedRectangle(Image imgPhoto, int radius)
        {
            using (Image image = imgPhoto)
            {
                Bitmap bitmap = new Bitmap(image.Width, image.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.Transparent);

                Brush brush = new TextureBrush(image);
                FillRoundedRectangle(graphics, new Rectangle(1, 1, image.Width - 3, image.Height - 3), (radius*2), brush);
                
                graphics.Dispose();

                return bitmap;
            }
        }

        public static Image RoundedRectangle(Image imgPhoto, int radius, int borderWidth, string borderColor)
        {
            using (Image image = imgPhoto)
            {
                Bitmap bitmap = new Bitmap(image.Width, image.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.Transparent);

                Brush brush = new TextureBrush(image);
                Pen pen = new Pen(GetColorFromArgb(borderColor), borderWidth);

                FillRoundedRectangle(graphics, new Rectangle(1, 1, image.Width - 3, image.Height - 3), (radius*2), brush);
                DrawRoundedRectangle(graphics, new Rectangle(1, 1, image.Width - 3, image.Height - 3), (radius*2), pen);

                graphics.Dispose();

                return bitmap;
            }
        }

        // http://forums.asp.net/p/942160/1128861.aspx
        private static void FillRoundedRectangle(Graphics grPhoto, Rectangle r, int d, Brush b)
        {
            Graphics graphics = grPhoto;
            int tempInt = d/2;

            // Create points that define polygon.
            Point[] points = {
                new Point(tempInt, 1),
                new Point(r.Width - tempInt, 1),
                new Point(r.Width, tempInt),
                new Point(r.Width, r.Height - tempInt),
                new Point(r.Width - tempInt, r.Height),
                new Point(tempInt, r.Height),
                new Point(1, r.Height - tempInt),
                new Point(1, tempInt)
            };

            // Define fill mode.
            const FillMode newFillMode = FillMode.Winding;

            graphics.SmoothingMode = SmoothingMode.Default;
            graphics.FillPolygon(b, points, newFillMode);

            // anti alias distorts fill so remove it.
            SmoothingMode mode = grPhoto.SmoothingMode;

            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.FillPie(b, r.X, r.Y, d, d, 180, 90);
            grPhoto.FillPie(b, r.X + r.Width - d, r.Y, d, d, 270, 90);
            grPhoto.FillPie(b, r.X, r.Y + r.Height - d, d, d, 90, 90);
            grPhoto.FillPie(b, r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            grPhoto.FillRectangle(b, r.X + d/2, r.Y, r.Width - d, d/2);
            grPhoto.FillRectangle(b, r.X, r.Y + d/2, r.Width, r.Height - d);
            grPhoto.FillRectangle(b, r.X + d/2, r.Y + r.Height - d/2, r.Width - d, d/2);
            grPhoto.SmoothingMode = mode;
        }

        // http://forums.asp.net/p/942160/1128861.aspx
        private static void DrawRoundedRectangle(Graphics grPhoto, Rectangle r, int d, Pen p)
        {
            // anti alias distorts fill so remove it.
            SmoothingMode mode = grPhoto.SmoothingMode;

            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.DrawArc(p, r.X, r.Y, d, d, 180, 90);
            grPhoto.DrawLine(p, r.X + d/2, r.Y, r.X + r.Width - d/2, r.Y);
            grPhoto.DrawArc(p, r.X + r.Width - d, r.Y, d, d, 270, 90);
            grPhoto.DrawLine(p, r.X, r.Y + d/2, r.X, r.Y + r.Height - d/2);
            grPhoto.DrawLine(p, r.X + r.Width, r.Y + d/2, r.X + r.Width, r.Y + r.Height - d/2);
            grPhoto.DrawLine(p, r.X + d/2, r.Y + r.Height, r.X + r.Width - d/2, r.Y + r.Height);
            grPhoto.DrawArc(p, r.X, r.Y + r.Height - d, d, d, 90, 90);
            grPhoto.DrawArc(p, r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            grPhoto.SmoothingMode = mode;
        }

        // http://dotnet-snippets.de/dns/c-hexstring-in-systemdrawingcolor-umwandeln-SID95.aspx
        private static Color GetColorFromArgb(string hexString)
        {
            int red = int.Parse(hexString.Substring(1, 2), NumberStyles.HexNumber);
            int green = int.Parse(hexString.Substring(3, 2), NumberStyles.HexNumber);
            int blue = int.Parse(hexString.Substring(5, 2), NumberStyles.HexNumber);

            return Color.FromArgb(red, green, blue);
        }
    }
}
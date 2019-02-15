// BitmapManipulator class, provides some useful static functions which
// operate on .NET <code>Bitmap</code> objects in useful ways.
// 
// Some of the useful features of this class incldue:
// <ul>
//   <li><code>GetBitmapFromUri</code> which downloads a bitmap from a URL, providing
//   some useful error message elaboration logic to present users with a more meaningful
//   error message in the event of a failure.</li>
// 
//   <li><code>ConvertBitmap</code> functions, which convert a bitmap from one format
//   to another, including optional quality and compression parameters for codecs like JPEG and
//   TIFF, respectively.</li>
// 
//   <li><code>ScaleBitmap</code> and <code>ResizeBitmap</code>, for modifying the dimensions
//   of a bitmap (these are standard issue and boring, but nonetheless useful)</li>
// 
//   <li><code>ThumbnailBitmap</code>, a very useful function that produces a thumbnail of an image
//   that fits within a given rectangle</li>
// 
//   <li><code>OverlayBitmap</code>, a useful function that overlays one bitmap atop another
//   with a caller-defined alpha parameter.  Great for putting watermarks or logos on pictures.</li>
// 
//   <li>A few other standard-issue image manipulation functions</li>
// </ul>
// 
// NOTE: This code includes support for GIF en/decoding, via the .NET Framework's
// System.Drawing classes.  However, in order to provide GIF functionality in your
// application, you must license the LZW encoding scheme used in GIF files from Unisys.
// As this is an opportunistic money-grab akin to SCO's, you are well advised to refuse
// to do this, and instead favor PNG whenever possible.
// 
// For more information, see http://www.microsoft.com/DEVONLY/Unisys.htm
// 
// Current Version: 1.0.0
// Revision History:
// 1.0.0 - ajn - 9/1/2003 - First release
// 
// Copyright(C) 2003 Adam J. Nelson.
// 
// This code is hereby released for unlimited non-commercial and commercial use
// 
// The author makes no guarantee regarding the fitness of this code, and hereby disclaims
// all liability for any damage incurred as a result of using this code.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace Sharpcms.Base.Library
{
    /// <summary>
    /// Utility class with static methods that do various useful things
    /// with bitmaps that require multiple GDI+ calls with .NET CLR
    /// </summary>
    public class BitmapManipulator
    {
        //MIME types for the various image formats

        #region ImageCornerEnum enum

        public enum ImageCornerEnum
        {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft,
            Center
        };

        #endregion

        #region TiffCompressionEnum enum

        public enum TiffCompressionEnum
        {
            CCITT3,
            CCITT4,
            LZW,
            RLE,
            None,
            Unspecified
        };

        #endregion

        private const String MIMEBmp = "image/bmp";
        private const String MIMEGif = "image/gif";
        private const String MIMEJpeg = "image/jpeg";
        private const String MIMEPjpeg = "image/pjpeg";
        private const String MIMEPng = "image/x-png";
        private const String MIMETiff = "image/tiff";

        private static readonly String[] SupportedMimeTypes = new[] {
            MIMEGif,
            MIMEJpeg,
            MIMEPjpeg,
            MIMETiff,
            MIMEPng,
            MIMEBmp
        };

        /// <summary>Attempts to download a bitmap from a given URI, then loads the bitmap into
        /// a <code>Bitmap</code> object and returns.
        /// 
        /// Obviously there are numerous failure cases for a function like this.  For ease 
        /// of use, all errors will be reported in a catch-all <code>BitmapManipException</code>,
        /// which provides a textual error message based on the exception that occurs.  As usual,
        /// the underlying exception is available in <code>InnerException</code> property.
        /// 
        /// Times out after 10 seconds waiting for a response from the server.</summary>
        /// 
        /// <param name="uri">String containing URI from which to retrieve image</param>
        /// 
        /// <returns>Bitmap object from URI.  Shouldn't ever be null, as any error will be reported
        ///     in an exception.</returns>
        public static Bitmap GetBitmapFromUri(String uri)
        {
            //Convert String to URI
            try
            {
                var uriObj = new Uri(uri);

                return GetBitmapFromUri(uriObj);
            }
            catch (ArgumentNullException exception)
            {
                throw new BitmapManipException("Parameter 'uri' is null", exception);
            }
            catch (UriFormatException exception)
            {
                throw new BitmapManipException(String.Format("Parameter 'uri' is malformed: {0}", exception.Message), exception);
            }
        }

        /// <summary>Attempts to download a bitmap from a given URI, then loads the bitmap into
        /// a <code>Bitmap</code> object and returns.
        /// 
        /// Obviously there are numerous failure cases for a function like this.  For ease
        /// of use, all errors will be reported in a catch-all <code>BitmapManipException</code>,
        /// which provides a textual error message based on the exception that occurs.  As usual,
        /// the underlying exception is available in <code>InnerException</code> property.
        /// 
        /// Times out after 10 seconds waiting for a response from the server.</summary>
        /// 
        /// <param name="uri"><code>Uri</code> object specifying the URI from which to retrieve image</param>
        /// 
        /// <returns>Bitmap object from URI.  Shouldn't ever be null, as any error will be reported
        ///     in an exception.</returns>
        private static Bitmap GetBitmapFromUri(Uri uri)
        {
            var bitmap = GetBitmapFromUri(uri, 10*1000);

            return bitmap;
        }

        /// <summary>Attempts to download a bitmap from a given URI, then loads the bitmap into
        /// a <code>Bitmap</code> object and returns.
        /// 
        /// Obviously there are numerous failure cases for a function like this.  For ease 
        /// of use, all errors will be reported in a catch-all <code>BitmapManipException</code>,
        /// which provides a textual error message based on the exception that occurs.  As usual,
        /// the underlying exception is available in <code>InnerException</code> property.
        /// </summary>
        /// 
        /// <param name="uri">String containing URI from which to retrieve image</param>
        /// <param name="timeoutMs">Timeout (in milliseconds) to wait for response</param>
        /// 
        /// <returns>Bitmap object from URI.  Shouldn't ever be null, as any error will be reported
        ///     in an exception.</returns>
        public static Bitmap GetBitmapFromUri(String uri, int timeoutMs)
        {
            //Convert String to URI
            try
            {
                var uriObj = new Uri(uri);

                return GetBitmapFromUri(uriObj, timeoutMs);
            }
            catch (ArgumentNullException ex)
            {
                throw new BitmapManipException("Parameter 'uri' is null", ex);
            }
            catch (UriFormatException ex)
            {
                throw new BitmapManipException(String.Format("Parameter 'uri' is malformed: {0}", ex.Message),
                                               ex);
            }
        }

        /// <summary>Attempts to download a bitmap from a given URI, then loads the bitmap into
        /// a <code>Bitmap</code> object and returns.
        /// 
        /// Obviously there are numerous failure cases for a function like this.  For ease
        /// of use, all errors will be reported in a catch-all <code>BitmapManipException</code>,
        /// which provides a textual error message based on the exception that occurs.  As usual,
        /// the underlying exception is available in <code>InnerException</code> property.
        /// </summary>
        /// 
        /// <param name="uri"><code>Uri</code> object specifying the URI from which to retrieve image</param>
        /// <param name="timeoutMs">Timeout (in milliseconds) to wait for response</param>
        /// 
        /// <returns>Bitmap object from URI.  Shouldn't ever be null, as any error will be reported
        ///     in an exception.</returns>
        private static Bitmap GetBitmapFromUri(Uri uri, int timeoutMs)
        {
            //Create a web request object for the URI, retrieve the contents,
            //then feed the results into a new Bitmap object.  Note that we 
            //are particularly sensitive to timeouts, since this all must happen
            //while the user waits
            try
            {
                var req = WebRequest.Create(uri);
                req.Timeout = timeoutMs;

                //The GetResponse call actually makes the request
                var resp = req.GetResponse();

                //Check the content type of the response to make sure it is
                //one of the formats we support
                if (Array.IndexOf(SupportedMimeTypes, resp.ContentType) == -1)
                {
                    String contentType = resp.ContentType;
                    resp.Close();
                    throw new BitmapManipException(
                        String.Format(
                            "The image at the URL you provided is in an unsupported format ({0}).  Uploaded images must be in either JPEG, GIF, BMP, TIFF, PNG, or WMF formats.",
                            contentType),
                        new NotSupportedException(String.Format("MIME type '{0}' is not a recognized image type", contentType)));
                }

                //Otherwise, looks fine
                Bitmap downloadedImage = new Bitmap(resp.GetResponseStream());

                resp.Close();

                return downloadedImage;
            }
            catch (UriFormatException exp)
            {
                throw new BitmapManipException("The URL you entered is not valid.  Please enter a valid URL, of the form http://servername.com/folder/image.gif", exp);
            }
            catch (WebException exp)
            {
                //Some sort of problem w/ the web request
                String errorDescription;

                switch (exp.Status)
                {
                    case WebExceptionStatus.ConnectFailure:
                        errorDescription = "Connect failure";
                        break;
                    case WebExceptionStatus.ConnectionClosed:
                        errorDescription = "Connection closed prematurely";
                        break;
                    case WebExceptionStatus.KeepAliveFailure:
                        errorDescription = "Connection closed in spite of keep-alives";
                        break;
                    case WebExceptionStatus.NameResolutionFailure:
                        errorDescription = "Unable to resolve server name.  Double-check the URL for errors";
                        break;
                    case WebExceptionStatus.ProtocolError:
                        errorDescription =
                            "Protocol-level error.  The server may have reported an error like 404 (file not found) or 403 (access denied), or some other similar error";
                        break;
                    case WebExceptionStatus.ReceiveFailure:
                        errorDescription = "The server did not send a complete response";
                        break;
                    case WebExceptionStatus.SendFailure:
                        errorDescription = "The complete request could not be sent to the server";
                        break;
                    case WebExceptionStatus.ServerProtocolViolation:
                        errorDescription = "The server response was not a valid HTTP response";
                        break;
                    case WebExceptionStatus.Timeout:
                        errorDescription =
                            "The server did not respond quickly enough.  The server may be down or overloaded.  Try again later";
                        break;
                    default:
                        errorDescription = exp.Status.ToString();
                        break;
                }

                throw new BitmapManipException(
                    String.Format(
                        "An error occurred while communicating with the server at the URL you provided.  {0}.",
                        errorDescription),
                    exp);
            }
            catch (BitmapManipException)
            {
                //Don't modify this one; pass it along
                throw;
            }
            catch (Exception exp)
            {
                throw new BitmapManipException(
                    String.Format("An error ocurred while retrieving the image from the URL you provided: {0}",
                                  exp.Message),
                    exp);
            }
        }

        /// <summary>Converts a bitmap to a JPEG with a specific quality level</summary>
        /// 
        /// <param name="inputBmp">Bitmap to convert</param>
        /// <param name="quality">Specifies a quality from 0 (lowest) to 100 (highest), or -1 to leave
        /// unspecified</param>
        /// 
        /// <returns>A new bitmap object containing the input bitmap converted.
        ///     If the destination format and the target format are the same, returns
        ///     a clone of the destination bitmap.</returns>
        public static Bitmap ConvertBitmapToJpeg(Bitmap inputBmp, int quality)
        {
            //If the dest format matches the source format and quality not changing, just clone
            if (inputBmp.RawFormat.Equals(ImageFormat.Jpeg) && quality == -1)
                return (Bitmap) inputBmp.Clone();

            //Create an in-memory stream which will be used to save
            //the converted image
            Stream imgStream = new MemoryStream();

            //Get the ImageCodecInfo for the desired target format
            ImageCodecInfo destCodec = FindCodecForType(MimeTypeFromImageFormat(ImageFormat.Jpeg));

            if (destCodec == null)
            {
                //No codec available for that format
                throw new ArgumentException(
                    "The requested format " + MimeTypeFromImageFormat(ImageFormat.Jpeg) +
                    " does not have an available codec installed", "destFormat");
            }

            //Create an EncoderParameters collection to contain the
            //parameters that control the dest format's encoder
            var destEncParams = new EncoderParameters(1);

            //Use quality parameter
            var qualityParam = new EncoderParameter(Encoder.Quality, quality);
            destEncParams.Param[0] = qualityParam;

            //Save w/ the selected codec and encoder parameters
            inputBmp.Save(imgStream, destCodec, destEncParams);

            //At this point, imgStream contains the binary form of the
            //bitmap in the target format.  All that remains is to load it
            //into a new bitmap object
            var destBitmap = new Bitmap(imgStream);

            //Free the stream
            //imgStream.Close();
            //For some reason, the above causes unhandled GDI+ exceptions
            //when destBitmap.Save is called.  Perhaps the bitmap object reads
            //from the stream asynchronously?

            return destBitmap;
        }

        /// <summary>Converts a bitmap to a Tiff with a specific compression</summary>
        /// 
        /// <param name="inputBmp">Bitmap to convert</param>
        /// <param name="compression">The compression to use on the TIFF file output.  Be warned that the CCITT3, CCITT4,
        ///     and RLE compression options are only applicable to TIFFs using a palette index color depth 
        ///     (that is, 1, 4, or 8 bpp).  Using any of these compression schemes with 24 or 32-bit 
        ///     TIFFs will throw an exception from the bowels of GDI+</param>
        /// 
        /// <returns>A new bitmap object containing the input bitmap converted.
        ///     If the destination format and the target format are the same, returns
        ///     a clone of the destination bitmap.</returns>
        public static Bitmap ConvertBitmapToTiff(Bitmap inputBmp, TiffCompressionEnum compression)
        {
            //If the dest format matches the source format and quality/bpp not changing, just clone
            if (inputBmp.RawFormat.Equals(ImageFormat.Tiff) && compression == TiffCompressionEnum.Unspecified)
                return (Bitmap) inputBmp.Clone();

            if (compression == TiffCompressionEnum.Unspecified)
                return ConvertBitmap(inputBmp, ImageFormat.Tiff);
            //None of the params are chaning; use the general purpose converter

            //Create an in-memory stream which will be used to save
            //the converted image
            Stream imgStream = new MemoryStream();

            //Get the ImageCodecInfo for the desired target format
            ImageCodecInfo destCodec = FindCodecForType(MimeTypeFromImageFormat(ImageFormat.Tiff));

            if (destCodec == null)
                //No codec available for that format
                throw new ArgumentException("The requested format " +
                                            MimeTypeFromImageFormat(ImageFormat.Tiff) +
                                            " does not have an available codec installed",
                                            "destFormat");

            //Create an EncoderParameters collection to contain the
            //parameters that control the dest format's encoder
            var destEncParams = new EncoderParameters(1);

            //set the compression parameter
            EncoderValue compressionValue;

            switch (compression)
            {
                case TiffCompressionEnum.CCITT3:
                    compressionValue = EncoderValue.CompressionCCITT3;
                    break;

                case TiffCompressionEnum.CCITT4:
                    compressionValue = EncoderValue.CompressionCCITT4;
                    break;

                case TiffCompressionEnum.LZW:
                    compressionValue = EncoderValue.CompressionLZW;
                    break;

                case TiffCompressionEnum.RLE:
                    compressionValue = EncoderValue.CompressionRle;
                    break;

                default:
                    compressionValue = EncoderValue.CompressionNone;
                    break;
            }
            var compressionParam = new EncoderParameter(Encoder.Compression, (long) compressionValue);

            destEncParams.Param[0] = compressionParam;

            //Save w/ the selected codec and encoder parameters
            inputBmp.Save(imgStream, destCodec, destEncParams);

            //At this point, imgStream contains the binary form of the
            //bitmap in the target format.  All that remains is to load it
            //into a new bitmap object
            var destBitmap = new Bitmap(imgStream);

            //Free the stream
            //imgStream.Close();
            //For some reason, the above causes unhandled GDI+ exceptions
            //when destBitmap.Save is called.  Perhaps the bitmap object reads
            //from the stream asynchronously?

            return destBitmap;
        }

        /// <summary>Converts a bitmap to another bitmap format, returning the new converted
        ///     bitmap
        /// </summary>
        /// 
        /// <param name="inputBmp">Bitmap to convert</param>
        /// <param name="destMimeType">MIME type of format to convert to</param>
        /// 
        /// <returns>A new bitmap object containing the input bitmap converted.
        ///     If the destination format and the target format are the same, returns
        ///     a clone of the destination bitmap.</returns>
        public static Bitmap ConvertBitmap(Bitmap inputBmp, String destMimeType)
        {
            return ConvertBitmap(inputBmp, ImageFormatFromMimeType(destMimeType));
        }

        /// <summary>Converts a bitmap to another bitmap format, returning the new converted
        ///     bitmap
        /// </summary>
        /// 
        /// <param name="inputBmp">Bitmap to convert</param>
        /// <param name="destFormat">Bitmap format to convert to</param>
        /// 
        /// <returns>A new bitmap object containing the input bitmap converted.
        ///     If the destination format and the target format are the same, returns
        ///     a clone of the destination bitmap.</returns>
        private static Bitmap ConvertBitmap(Image inputBmp, ImageFormat destFormat)
        {
            //If the dest format matches the source format and quality/bpp not changing, just clone
            if (inputBmp.RawFormat.Equals(destFormat))
                return (Bitmap) inputBmp.Clone();

            //Create an in-memory stream which will be used to save
            //the converted image
            Stream imgStream = new MemoryStream();

            //Save the bitmap out to the memory stream, using the format indicated by the caller
            inputBmp.Save(imgStream, destFormat);

            //At this point, imgStream contains the binary form of the
            //bitmap in the target format.  All that remains is to load it
            //into a new bitmap object
            var destBitmap = new Bitmap(imgStream);

            //Free the stream
            //imgStream.Close();
            //For some reason, the above causes unhandled GDI+ exceptions
            //when destBitmap.Save is called.  Perhaps the bitmap object reads
            //from the stream asynchronously?

            return destBitmap;
        }

        /// <summary>
        /// Scales a bitmap by a scale factor, growing or shrinking both axes while
        /// maintaining the original aspect ratio
        /// </summary>
        /// <param name="inputBmp">Bitmap to scale</param>
        /// <param name="scaleFactor">Factor by which to scale</param>
        /// <returns>New bitmap containing image from inputBmp, scaled by the scale factor</returns>
        private static Bitmap ScaleBitmap(Image inputBmp, double scaleFactor)
        {
            return ScaleBitmap(inputBmp, scaleFactor, scaleFactor);
        }

        /// <summary>
        /// Scales a bitmap by a scale factor, growing or shrinking both axes independently,
        /// possibly changing the aspect ration
        /// </summary>
        /// <param name="inputBmp">Bitmap to scale</param>
        /// <param name="xScaleFactor">The x scale factor.</param>
        /// <param name="yScaleFactor">The y scale factor.</param>
        /// <returns>
        /// New bitmap containing image from inputBmp, scaled by the scale factor
        /// </returns>
        private static Bitmap ScaleBitmap(Image inputBmp, double xScaleFactor, double yScaleFactor)
        {
            //Create a new bitmap object based on the input
            var newBmp = new Bitmap(
                (int) (inputBmp.Size.Width*xScaleFactor),
                (int) (inputBmp.Size.Height*yScaleFactor),
                PixelFormat.Format24bppRgb); //Graphics.FromImage doesn't like Indexed pixel format

            //Create a graphics object attached to the new bitmap
            Graphics newBmpGraphics = Graphics.FromImage(newBmp);

            //Set the interpolation mode to high quality bicubic 
            //interpolation, to maximize the quality of the scaled image
            newBmpGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            newBmpGraphics.ScaleTransform((float) xScaleFactor, (float) yScaleFactor);

            //Draw the bitmap in the graphics object, which will apply
            //the scale transform
            //Note that pixel units must be specified to ensure the framework doesn't attempt
            //to compensate for varying horizontal resolutions in images by resizing; in this case,
            //that's the opposite of what we want.
            var drawRect = new Rectangle(0, 0, inputBmp.Size.Width, inputBmp.Size.Height);
            newBmpGraphics.DrawImage(inputBmp, drawRect, drawRect, GraphicsUnit.Pixel);

            //Return the bitmap, as the operations on the graphics object
            //are applied to the bitmap
            newBmpGraphics.Dispose();

            //newBmp will have a RawFormat of MemoryBmp because it was created
            //from scratch instead of being based on inputBmp.  Since it it inconvenient
            //for the returned version of a bitmap to be of a different format, now convert
            //the scaled bitmap to the format of the source bitmap
            return ConvertBitmap(newBmp, inputBmp.RawFormat);
        }

        /// <summary>
        /// Resizes a bitmap's width and height independently
        /// </summary>
        /// <param name="inputBmp">Bitmap to resize</param>
        /// <param name="imgWidth">New width</param>
        /// <param name="imgHeight">New height</param>
        /// <returns>Resized bitmap</returns>
        public static Bitmap ResizeBitmap(Bitmap inputBmp, int imgWidth, int imgHeight)
        {
            //Simply compute scale factors that result in the desired size, then call ScaleBitmap
            return ScaleBitmap(inputBmp,
                               imgWidth/(float) inputBmp.Size.Width,
                               imgHeight/(float) inputBmp.Size.Height);
        }

        /// <summary>
        /// Generates a thumbnail of the bitmap.  This is effectively a specialized
        /// resize function, which maintains the aspect ratio of the image while
        /// resizing it to ensure that both its width and height are within
        /// caller-specified maximums
        /// </summary>
        /// <param name="inputBmp">Bitmap for which to generate thumbnail</param>
        /// <param name="maxWidth">Maximum width of thumbnail</param>
        /// <param name="maxHeight">Maximum height of thumbnail</param>
        /// <returns>Thumbnail of inputBmp w/ the same aspect ratio, but
        /// width and height both less than or equal to the maximum limits</returns>
        public static Bitmap ThumbnailBitmap(Bitmap inputBmp, int maxWidth, int maxHeight)
        {
            //Compute the scaling factor that will scale the bitmap witdh
            //to the max width, and the other scaling factor that will scale
            //the bitmap height to the max height.
            //Apply the lower of the two, then if the other dimension is still
            //outside the caller-defined limits, compute the scaling factor
            //which will bring that dimension within the limits.
            double widthScaleFactor = maxWidth/(double) inputBmp.Size.Width;
            double heightScaleFactor = maxHeight/(double) inputBmp.Size.Height;
            double finalScaleFactor;

            //Now pick the smaller scale factor
            if (widthScaleFactor < heightScaleFactor)
            {
                //If this scale factor doesn't bring the height
                //within the required maximum, combine this width
                //scale factor with an additional scaling factor
                //to take the height the rest of the way down
                if (inputBmp.Size.Height*widthScaleFactor > maxHeight)
                {
                    //Need to scale height further
                    heightScaleFactor = (maxHeight*widthScaleFactor)/inputBmp.Size.Height;

                    finalScaleFactor = widthScaleFactor*heightScaleFactor;
                }
                else
                {
                    //Width scale factor brings both dimensions inline sufficiently
                    finalScaleFactor = widthScaleFactor;
                }
            }
            else
            {
                //Else, height scale factor is smaller than width.
                //Apply the same logic as above, but with the roles of the width
                //and height scale factors reversed
                if (inputBmp.Size.Width*heightScaleFactor > maxWidth)
                {
                    //Need to scale height further
                    widthScaleFactor = (maxWidth*heightScaleFactor)/inputBmp.Size.Width;

                    finalScaleFactor = widthScaleFactor*heightScaleFactor;
                }
                else
                {
                    //Height scale factor brings both dimensions inline sufficiently
                    finalScaleFactor = heightScaleFactor;
                }
            }

            return ScaleBitmap(inputBmp, finalScaleFactor);
        }

        public static Bitmap RotateBitmapRight90(Bitmap inputBmp)
        {
            //Copy bitmap
            var newBmp = (Bitmap) inputBmp.Clone();

            newBmp.RotateFlip(RotateFlipType.Rotate90FlipNone);

            //The RotateFlip transformation converts bitmaps to memoryBmp,
            //which is uncool.  Convert back now
            return ConvertBitmap(newBmp, inputBmp.RawFormat);
        }

        public static Bitmap RotateBitmapRight180(Bitmap inputBmp)
        {
            //Copy bitmap
            var newBmp = (Bitmap) inputBmp.Clone();

            newBmp.RotateFlip(RotateFlipType.Rotate180FlipNone);

            //The RotateFlip transformation converts bitmaps to memoryBmp,
            //which is uncool.  Convert back now
            return ConvertBitmap(newBmp, inputBmp.RawFormat);
        }

        public static Bitmap RotateBitmapRight270(Bitmap inputBmp)
        {
            //Copy bitmap
            var newBmp = (Bitmap) inputBmp.Clone();

            newBmp.RotateFlip(RotateFlipType.Rotate270FlipNone);

            //The RotateFlip transformation converts bitmaps to memoryBmp,
            //which is uncool.  Convert back now
            return ConvertBitmap(newBmp, inputBmp.RawFormat);
        }

        /// <summary>
        /// Reverses a bitmap, effectively rotating it 180 degrees in 3D space about
        /// the Y axis.  Results in a "mirror image" of the bitmap, reversed much
        /// as it would be in a mirror
        /// </summary>
        /// <param name="inputBmp"></param>
        /// <returns></returns>
        public static Bitmap ReverseBitmap(Bitmap inputBmp)
        {
            //Copy the bitmap to a new bitmap object
            var newBmp = (Bitmap) inputBmp.Clone();

            //Flip the bitmap
            newBmp.RotateFlip(RotateFlipType.RotateNoneFlipX);

            //The RotateFlip transformation converts bitmaps to memoryBmp,
            //which is uncool.  Convert back now
            return ConvertBitmap(newBmp, inputBmp.RawFormat);
        }

        /// <summary>
        /// Reverses a bitmap, effectively rotating it 180 degrees in 3D space about
        /// the X axis.  Results in an upside-down view of the image
        /// </summary>
        /// <param name="inputBmp"></param>
        /// <returns></returns>
        public static Bitmap FlipBitmap(Bitmap inputBmp)
        {
            //Copy the bitmap to a new bitmap object
            var newBmp = (Bitmap) inputBmp.Clone();

            //Flip the bitmap
            newBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //The RotateFlip transformation converts bitmaps to memoryBmp,
            //which is uncool.  Convert back now
            return ConvertBitmap(newBmp, inputBmp.RawFormat);
        }

        /// <summary>
        /// Renders a bitmap over another bitmap, with a specific alpha value.
        /// This can be used to overlay a logo or watermark over a bitmap
        /// </summary>
        /// <param name="destBmp">Bitmap over which image is to be overlaid</param>
        /// <param name="bmpToOverlay">Bitmap to overlay</param>
        /// <param name="overlayAlpha">Alpha value fo overlay bitmap.  0 = fully transparent, 100 = fully opaque</param>
        /// <param name="overlayPoint">Location in destination bitmap where overlay image will be placed</param>
        /// <returns></returns>
        private static Bitmap OverlayBitmap(Image destBmp, Image bmpToOverlay, int overlayAlpha, Point overlayPoint)
        {
            //Convert alpha to a 0..1 scale
            float overlayAlphaFloat = overlayAlpha/100.0f;

            //Copy the destination bitmap
            //NOTE: Can't clone here, because if destBmp is indexed instead of just RGB, 
            //Graphics.FromImage will fail
            var newBmp = new Bitmap(destBmp.Size.Width,
                                    destBmp.Size.Height);

            //Create a graphics object attached to the bitmap
            Graphics newBmpGraphics = Graphics.FromImage(newBmp);

            //Draw the input bitmap into this new graphics object
            newBmpGraphics.DrawImage(destBmp,
                                     new Rectangle(0, 0,
                                                   destBmp.Size.Width,
                                                   destBmp.Size.Height),
                                     0, 0, destBmp.Size.Width, destBmp.Size.Height,
                                     GraphicsUnit.Pixel);

            //Create a new bitmap object the same size as the overlay bitmap
            var overlayBmp = new Bitmap(bmpToOverlay.Size.Width, bmpToOverlay.Size.Height);

            //Make overlayBmp transparent
            overlayBmp.MakeTransparent(overlayBmp.GetPixel(0, 0));

            //Create a graphics object attached to the bitmap
            Graphics overlayBmpGraphics = Graphics.FromImage(overlayBmp);

            //Create a color matrix which will be applied to the overlay bitmap
            //to modify the alpha of the entire image
            float[][] colorMatrixItems = {
                                             new float[] {1, 0, 0, 0, 0},
                                             new float[] {0, 1, 0, 0, 0},
                                             new float[] {0, 0, 1, 0, 0},
                                             new[] {0, 0, 0, overlayAlphaFloat, 0},
                                             new float[] {0, 0, 0, 0, 1}
                                         };

            var colorMatrix = new ColorMatrix(colorMatrixItems);

            //Create an ImageAttributes class to contain a color matrix attribute
            var imageAttrs = new ImageAttributes();
            imageAttrs.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            //Draw the overlay bitmap into the graphics object, applying the image attributes
            //which includes the reduced alpha
            var drawRect = new Rectangle(0, 0, bmpToOverlay.Size.Width, bmpToOverlay.Size.Height);
            overlayBmpGraphics.DrawImage(bmpToOverlay,
                                         drawRect,
                                         0, 0, bmpToOverlay.Size.Width, bmpToOverlay.Size.Height,
                                         GraphicsUnit.Pixel,
                                         imageAttrs);
            overlayBmpGraphics.Dispose();

            //overlayBmp now contains bmpToOverlay w/ the alpha applied.
            //Draw it onto the target graphics object
            //Note that pixel units must be specified to ensure the framework doesn't attempt
            //to compensate for varying horizontal resolutions in images by resizing; in this case,
            //that's the opposite of what we want.
            newBmpGraphics.DrawImage(overlayBmp,
                                     new Rectangle(overlayPoint.X, overlayPoint.Y, bmpToOverlay.Width,
                                                   bmpToOverlay.Height),
                                     drawRect,
                                     GraphicsUnit.Pixel);

            newBmpGraphics.Dispose();

            //Recall that newBmp was created as a memory bitmap; convert it to the format
            //of the input bitmap
            return ConvertBitmap(newBmp, destBmp.RawFormat);
        }

        /// <summary>
        /// Renders a bitmap over another bitmap, with a specific alpha value.
        /// This can be used to overlay a logo or watermark over a bitmap
        /// </summary>
        /// <param name="destBmp">Bitmap over which image is to be overlaid</param>
        /// <param name="bmpToOverlay">Bitmap to overlay</param>
        /// <param name="overlayAlpha">Alpha value fo overlay bitmap.  0 = fully transparent, 100 = fully opaque</param>
        /// <param name="corner">Corner of destination bitmap to place overlay bitmap</param>
        /// <returns></returns>
        private static Bitmap OverlayBitmap(Image destBmp, Image bmpToOverlay, int overlayAlpha, ImageCornerEnum corner)
        {
            //Translate corner to rectangle and pass through to other impl
            Point overlayPoint;

            if (corner.Equals(ImageCornerEnum.TopLeft))
                overlayPoint = new Point(0, 0);
            else if (corner.Equals(ImageCornerEnum.TopRight))
                overlayPoint = new Point(destBmp.Size.Width - bmpToOverlay.Size.Width, 0);
            else if (corner.Equals(ImageCornerEnum.BottomRight))
                overlayPoint = new Point(destBmp.Size.Width - bmpToOverlay.Size.Width,
                                         destBmp.Size.Height - bmpToOverlay.Size.Height);
            else if (corner.Equals(ImageCornerEnum.Center))
                overlayPoint = new Point(destBmp.Size.Width/2 - bmpToOverlay.Size.Width/2,
                                         destBmp.Size.Height/2 - bmpToOverlay.Size.Height/2);
            else
                overlayPoint = new Point(0, destBmp.Size.Height - bmpToOverlay.Size.Height);

            return OverlayBitmap(destBmp, bmpToOverlay, overlayAlpha, overlayPoint);
        }

        public static Bitmap OverlayBitmap(Bitmap destBmp, Bitmap bmpToOverlay, Point overlayPoint)
        {
            return OverlayBitmap(destBmp, bmpToOverlay, 0, overlayPoint);
        }

        public static Bitmap OverlayBitmap(Bitmap destBmp, Bitmap bmpToOverlay, ImageCornerEnum corner)
        {
            return OverlayBitmap(destBmp, bmpToOverlay, 0, corner);
        }

        /// <summary>
        /// Crops an image, resulting in a new image consisting of the portion of the
        /// original image contained in a provided bounding rectangle
        /// </summary>
        /// <param name="inputBmp">Bitmap to crop</param>
        /// <param name="cropRectangle">Rectangle specifying the range of pixels
        /// within the image which is to be retained</param>
        /// <returns>New bitmap consisting of the contents of the crop rectangle</returns>
        public static Bitmap CropBitmap(Bitmap inputBmp, Rectangle cropRectangle)
        {
            //Create a new bitmap object based on the input
            var newBmp = new Bitmap(cropRectangle.Width,
                                    cropRectangle.Height,
                                    PixelFormat.Format24bppRgb); //Graphics.FromImage doesn't like Indexed pixel format

            //Create a graphics object and attach it to the bitmap
            Graphics newBmpGraphics = Graphics.FromImage(newBmp);

            //Draw the portion of the input image in the crop rectangle
            //in the graphics object
            newBmpGraphics.DrawImage(inputBmp,
                                     new Rectangle(0, 0, cropRectangle.Width, cropRectangle.Height),
                                     cropRectangle,
                                     GraphicsUnit.Pixel);

            //Return the bitmap
            newBmpGraphics.Dispose();

            //newBmp will have a RawFormat of MemoryBmp because it was created
            //from scratch instead of being based on inputBmp.  Since it it inconvenient
            //for the returned version of a bitmap to be of a different format, now convert
            //the scaled bitmap to the format of the source bitmap
            return ConvertBitmap(newBmp, inputBmp.RawFormat);
        }

        private static String MimeTypeFromImageFormat(ImageFormat format)
        {
            if (format.Equals(ImageFormat.Jpeg))
                return MIMEJpeg;

            if (format.Equals(ImageFormat.Gif))
                return MIMEGif;

            if (format.Equals(ImageFormat.Bmp))
                return MIMEBmp;

            if (format.Equals(ImageFormat.Tiff))
                return MIMETiff;

            if (format.Equals(ImageFormat.Png))
                return MIMEPng;

            throw new ArgumentException("Unsupported  image format '" + format + "'", "format");
        }

        private static ImageFormat ImageFormatFromMimeType(String mimeType)
        {
            switch (mimeType)
            {
                case MIMEJpeg:
                case MIMEPjpeg:
                    return ImageFormat.Jpeg;
                case MIMEGif:
                    return ImageFormat.Gif;
                case MIMEBmp:
                    return ImageFormat.Bmp;
                case MIMETiff:
                    return ImageFormat.Tiff;
                case MIMEPng:
                    return ImageFormat.Png;
                default:
                    throw new ArgumentException("Unsupported  MIME type '" + mimeType + "'", "mimeType");
            }
        }

        private static ImageCodecInfo FindCodecForType(String mimeType)
        {
            ImageCodecInfo[] imgEncoders = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < imgEncoders.GetLength(0); i++)
                if (imgEncoders[i].MimeType == mimeType)
                    return imgEncoders[i]; //Found it

            //No encoders match
            return null;
        }

        #region Nested type: BitmapManipException

        private class BitmapManipException : Exception
        {
            public BitmapManipException(String msg, Exception innerException)
                : base(msg, innerException)
            {
            }
        }

        #endregion
    }
}
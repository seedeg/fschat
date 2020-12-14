using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace FsChat.Utilities.Common
{
    public static class ImageManipulation
    {
        public static Image ConvertBase64StringToImage(string imageContent)
        {
            var bytes = Convert.FromBase64String(imageContent);
            using (var ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return Image.FromStream(ms);
            }
        }

        public static byte[] ConvertBase64ImageToBytes(string imageContent)
        {
            using (var image = ConvertBase64StringToImage(imageContent))
            {
                return image.ToBytes(ImageFormat.Png);
            }
        }

        public static byte[] ToBytes(this Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                ms.Position = 0;
                return ms.ToArray();
            }
                
        }

        public static byte[] ResizeImageFromStream(Stream inputStream, int width, int height)
        {
            inputStream.Seek(0, SeekOrigin.Begin);
            using (var img = Image.FromStream(inputStream))
            {
                if (width == 0 && height == 0)
                {
                    throw new InvalidOperationException("A width or a height must be provided");
                }

                var mustCrop = true;

                if (width == 0)
                {
                    width = (img.Size.Width * height / img.Size.Height);
                    mustCrop = false;
                }
                if (height == 0)
                {
                    height = (img.Size.Height * width / img.Size.Width);
                    mustCrop = false;
                }

                var sourceBitmap = Image.FromStream(inputStream);

                var targetImageHorizontalDiff = 0;
                var targetImageVerticalDiff = 0;
                if (sourceBitmap.Width > sourceBitmap.Height)
                {
                    targetImageHorizontalDiff = sourceBitmap.Width - sourceBitmap.Height;
                }
                if (sourceBitmap.Height > sourceBitmap.Width)
                {
                    targetImageVerticalDiff = sourceBitmap.Height - sourceBitmap.Width;
                }

                using (var targetImg = mustCrop
                    ? CropImage(sourceBitmap, new Rectangle(targetImageHorizontalDiff, targetImageVerticalDiff, Math.Min(sourceBitmap.Width, sourceBitmap.Height), Math.Min(sourceBitmap.Width, sourceBitmap.Height)), new Rectangle(0, 0, width, height))
                    : CropImage(sourceBitmap, new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), new Rectangle(0, 0, width, height)))
                {
                    using (var ms = new MemoryStream())
                    {
                        targetImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        return ms.ToArray();
                    }
                }
            }
        }

        public static Image ResizeImage(this Image image, Size newSize)
        {
            var newImage = new Bitmap(newSize.Width, newSize.Height);

            using (var gfx = Graphics.FromImage(newImage))
            {
                gfx.DrawImage(image, new Rectangle(Point.Empty, newSize));
            }

            return newImage;
        }

        public static Bitmap CropImage(Image originalImage, Rectangle sourceRectangle, Rectangle? destinationRectangle = null)
        {
            if (destinationRectangle == null)
            {
                destinationRectangle = new Rectangle(Point.Empty, sourceRectangle.Size);
            }

            var croppedImage = new Bitmap(destinationRectangle.Value.Width, destinationRectangle.Value.Height);
            using (var graphics = Graphics.FromImage(croppedImage))
            {
                graphics.DrawImage(originalImage, destinationRectangle.Value, sourceRectangle, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }

        public static byte[] ConvertImageUrlToByteArray(string url)
        {
            using (var webClient = new WebClient())
            {
                return webClient.DownloadData(url);
            }
        }
    }
}

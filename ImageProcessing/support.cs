using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenCvSharp;
using System.Drawing;
using OpenCvSharp.CPlusPlus;
using System.Drawing.Imaging;
using System.IO;

namespace FaceRecognitionSystem.ImageProcessing
{
    //Additional tools for image processing
    public static class support
    {
        public enum Algorithm { EigenFaces, FisherFaces };

        public static Mat norm_0_255(CvMat src)
        {
            // Create and return normalized image:
            CvMat dst = new CvMat(src.Rows, src.Cols, MatrixType.U8C1);
            switch (src.ElemChannels)
            {
                case 1:
                    dst = new CvMat(src.Rows, src.Cols, MatrixType.U8C1);
                    Cv.Normalize(src, dst, 0d, 255d, NormType.MinMax);
                    break;
                case 3:
                    dst = new CvMat(src.Rows, src.Cols, MatrixType.U8C3);
                    Cv.Normalize(src, dst, 0d, 255d, NormType.MinMax);
                    break;
                default:
                    src.Copy(dst);
                    break;
            }
            return dst;
        }

        //         static byte[] getRGB(Bitmap bmp)
        //         {
        //             var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
        //                 System.Drawing.Imaging.ImageLockMode.ReadOnly,
        //                 bmp.PixelFormat);
        //             try
        //             {
        //                 var ptr = (IntPtr)((long)data.Scan0);
        //                 var ret = new byte[bmp.Width*bmp.Height];
        //                 System.Runtime.InteropServices.Marshal.Copy(ptr, ret, 0, ret.Length);
        //                 return ret;
        //             }
        //             finally
        //             {
        //                 bmp.UnlockBits(data);
        //             }
        //         }

        public static byte[] ReadStreamToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static Stream VaryQualityLevel(Bitmap bmp)
        {
            // Get a bitmap.
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            MemoryStream ms = new MemoryStream();
            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            new Bitmap(bmp).Save(ms, jgpEncoder, myEncoderParameters);
            return ms;
        }


        public static IplImage ByteArrayToIplImage(byte[] imageBuffer, LoadMode loadMode)
        {
            //  OpenCvSharp.CPlusPlus.Mat m = Cv.EncodeImage(".jpg", img);
            IplImage res = IplImage.FromImageData((byte[])imageBuffer, loadMode);
            if (res == null)
            {
                Bitmap img = null;
                using (var ms = new System.IO.MemoryStream(imageBuffer))
                {
                    img = Image.FromStream(ms) as Bitmap;
                }
                Stream str = VaryQualityLevel(img);
                if (str != null)
                {
                    imageBuffer = ReadStreamToEnd(str);
                    str.Dispose();
                    str = null;
                }
                res = IplImage.FromImageData((byte[])imageBuffer, loadMode);
            }
            return res;
        }

        public static byte[] IplImageToByteArray(IplImage image)
        {
            byte[] managedArray = new byte[image.ImageSize];
            System.Runtime.InteropServices.Marshal.Copy(image.ImageData, managedArray, 0, image.ImageSize);
            Cv.ReleaseImage(image);
            return managedArray;
        }

        public static IplImage CropIplImage(IplImage img, CvRect? rect)
        {
            if (rect == null) return null;
            Cv.SetImageROI(img, (CvRect)rect);
            IplImage tmp = Cv.CreateImage(Cv.GetSize(img),
                                           img.Depth,
                                           img.NChannels);
            Cv.Copy(img, tmp, null);
            Cv.ResetImageROI(img);
            Cv.ReleaseImage(img);
            return tmp;
        }

        public static IplImage CropIplImage(IplImage img, int x, int y, int width, int height)
        {
            return CropIplImage(img, new CvRect(x, y, width, height));
        }

        public static Mat ResizeImage(Mat src, CvSize size)
        {
            Mat t = new Mat(size, src.Type);
            Cv.Resize(src.ToCvMat(), t.ToCvMat());
            return t;
        }

    }


}
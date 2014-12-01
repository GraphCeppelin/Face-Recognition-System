using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenCvSharp;

namespace FaceRecognitionSystem.ImageProcessing
{
    public static class HaarCascade
    {
        static CvHaarClassifierCascade faceCascade = LoadHaarCascade(HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath) + "haarcascade_frontalface_alt_tree.xml");

        private static CvHaarClassifierCascade LoadHaarCascade(string path)
        {
            string tstr = "";
            return Cv.Load<CvHaarClassifierCascade>(path, null, null, out tstr);
        }

        /// <summary>
        /// Find faces on the photo
        /// </summary>
        /// <param name="frame">photo</param>
        /// <returns>recognized faces</returns>
        public static IplImage[] GetFaces(IplImage frame)
        {
            // memory-access interface
            IplImage[] imgs = null;
            //HaarDetectObjects use a given storage area for it results and working storage
            using (var pStorageface = Cv.CreateMemStorage(0))
            // detect faces in image
            {
                var pFaceRectSeq = Cv.HaarDetectObjects
                   (frame as CvArr, faceCascade, pStorageface,
                   1.1d,                       // increase search scale by 10% each pass
                   3,                         // merge groups of three detections
                   HaarDetectionType.DoCannyPruning,  // skip regions unlikely to contain a face
                   Cv.Size(40, 40));            // smallest size face to detect = 40x40

                imgs = new IplImage[(pFaceRectSeq != null ? pFaceRectSeq.Total : 0)];
                // draw a rectangular outline around each detection
                int k = 0;
                for (int i = 0; i < imgs.Length; i++)
                {
                    try
                    {
                        //Crop
                        imgs[k] = support.CropIplImage(frame, Cv.GetSeqElem<CvRect>(pFaceRectSeq, i));
                        //Resize
                        imgs[k] = support.ResizeImage(imgs[k], new CvSize(100, 100)).ToIplImage();
                        //Encode to jpeg.
                        imgs[k] = ((OpenCvSharp.CPlusPlus.Mat)Cv.EncodeImage(".jpg", imgs[k])).ToIplImage();
                    }
                    catch
                    {
                        k--;
                    }
                    finally
                    {
                        k++;
                    }
                }
            }
            return imgs;
        }
    }
}
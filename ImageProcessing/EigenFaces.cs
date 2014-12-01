using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using System.IO;

namespace FaceRecognitionSystem.ImageProcessing
{
    class EigenFaces
    {

        /// <summary>
        /// Try to recognize face on the sample image by eigenfaces algorithm
        /// </summary>
        /// <param name="err">Eror happened due the execution of the subroutine</param>
        /// <param name="trainData">Array of images train a system</param>
        /// <param name="sample">Face to recognize</param>
        /// <param name="threshold">Threshold of the algorithm. Defines maximum length of the eigenvector length for recognition</param>
        /// <param name="cropImage">Defines whether it is need to crop sample image (to find a face)</param>
        /// <returns></returns>
        public static bool ProcessImageSample(out string err, out int predictedLabel, Dictionary<int, List<byte[]>> trainData, byte[] sample, int threshold = 3500, bool cropImage = false)
        {
            IplImage sampleImg = null; //image to recognize
            Mat eigenvalues = null; //eigenvalues
            Mat W = null; //eigenvectors
            Mat mean = null; //mean images
            CvSize actualImagesSize; //size of train data and sample images
            List<Mat> trainImages = new List<Mat>();
            err = "";
            predictedLabel = -1;
            List<int> labels = new List<int>();
            try
            {
                if (sample == null)
                    throw new ArgumentException("sample == null");
                sampleImg = support.ByteArrayToIplImage(sample, LoadMode.GrayScale);
                if (sampleImg == null)
                    throw new Exception("Could not convert sample byte array to IplImage");

            foreach (KeyValuePair<int, List<byte[]>> ms in trainData)
                foreach (byte[] m in ms.Value)
                {
                    trainImages.Add(support.ByteArrayToIplImage(m,LoadMode.GrayScale));
                    labels.Add(ms.Key);
                }
                
            // Quit if there are not enough images for this demo.
            if (trainImages.Count <= 1)
            {
                throw new Exception("Needs at least 2 images to work. Please add more images to your data set!");
            }
            else
            {
                actualImagesSize.Height = trainImages[0].Rows;
                actualImagesSize.Width = trainImages[0].Cols;
            }
            // Get the height from the first image. We'll need this
            // later in code to reshape the images to their original
            // size:
            int height = trainImages[0].Rows;
            // The following lines simply get the last images from
            // your dataset and remove it from the vector. This is
            // done, so that the training data (which we learn the
            // cv::FaceRecognizer on) and the test data we test
            // the model with, do not overlap.

            if (cropImage)
            {
                IplImage[] cropImgs = HaarCascade.GetFaces(sampleImg);
                if (cropImgs.Length > 0)
                {
                    sampleImg = support.ResizeImage(cropImgs[0], actualImagesSize).ToIplImage();
                }
                foreach (IplImage im in cropImgs)
                    Cv.ReleaseImage(im);
            }
            if (sampleImg.ElemChannels > 1)
            {
                throw new Exception("sampleImg should be grayscale!");
            }

            // The following line create an Eigenfaces model for
            // face recognition and train it with the images and
            // This here is a full PCA, if you just want to keep
            // 10 principal components (read Eigenfaces), then call
            // the factory method like this:
            //
            //      cv::createEigenFaceRecognizer(10);
            //
            // If you want to create a FaceRecognizer with a
            // confidence threshold (e.g. 123.0), call it with:
            //
            //      cv::createEigenFaceRecognizer(10, 123.0);
            //
            // If you want to use _all_ Eigenfaces and have a threshold,
            // then call the method like this:
            //
            //      cv::createEigenFaceRecognizer(0, 123.0);
            //


            predictedLabel = CvCpp.ProcessEigenFaceRecognizer(trainImages, labels, sampleImg, out eigenvalues, out W, out mean, 0, threshold);

            if (predictedLabel < 0)
                return false;
            Cv.ReleaseImage(sampleImg);
            foreach (IplImage im in trainImages)
                    Cv.ReleaseImage(im);
            return true;
            }
            catch (System.Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }

    }
}

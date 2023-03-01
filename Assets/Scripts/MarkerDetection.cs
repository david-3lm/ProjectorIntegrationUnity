using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using OpenCvSharp.Demo;
using OpenCvSharp.Aruco;
using UnityEngine.Windows;
using Unity;
using System.Security.Cryptography;

public class MarkerDetection : WebCamera
{

    private Mat img;

    


    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        img = OpenCvSharp.Unity.TextureToMat(input);
        DetectMark(img, out output);

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void DetectMark(Mat image, out Texture2D output)
    {
        // Create default parameres for detection
        DetectorParameters detectorParameters = DetectorParameters.Create();

        // Dictionary holds set of all available markers
        Dictionary dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);

        // Variables to hold results
        Point2f[][] corners;
        int[] ids;
        Point2f[][] rejectedImgPoints;

        // Create Opencv image from unity texture
        Mat mat = image;

        // Convert image to grasyscale
        Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

        // Detect and draw markers
        CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
        CvAruco.DrawDetectedMarkers(mat, corners, ids);

        Debug.Log(corners.Length);

        // Create Unity output texture with detected markers
        Texture2D outputTexture = OpenCvSharp.Unity.MatToTexture(mat);

        // Set texture to see the result
        //RawImage rawImage = gameObject.GetComponent<RawImage>();
        //rawImage.texture = outputTexture;

        output = outputTexture;
    }


}

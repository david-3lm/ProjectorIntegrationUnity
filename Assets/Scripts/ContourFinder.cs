using OpenCvSharp;
using OpenCvSharp.Demo;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContourFinder : WebCamera
{
    [SerializeField] private float threshold = 90f;
    [SerializeField] private bool ShowProcessedImg = true;

    private Mat img;
    private Mat processedImg = new Mat();

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        img = OpenCvSharp.Unity.TextureToMat(input);

        Cv2.CvtColor(img, processedImg, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(processedImg, processedImg, threshold,255,ThresholdTypes.BinaryInv);

        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(ShowProcessedImg ? processedImg : img);
        else
            OpenCvSharp.Unity.MatToTexture(ShowProcessedImg ? processedImg : img, output);
        return true;
    }
}

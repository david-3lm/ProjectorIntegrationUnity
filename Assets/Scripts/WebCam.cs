using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;

public class WebCam : WebCamera
{
    public Mat imgWebCam = null;
    public bool imgHilo = false;
    public Mat imgProcessed = null;
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        imgWebCam = OpenCvSharp.Unity.TextureToMat(input);
        if (imgHilo)
        {
            output = OpenCvSharp.Unity.MatToTexture(imgProcessed, output);
            imgHilo = false;
        }
        else
        {
            if (output == null)
                output = OpenCvSharp.Unity.MatToTexture(imgWebCam);
            else
                OpenCvSharp.Unity.MatToTexture(imgWebCam, output);
        }
        return true;
    }
    public void SetImgProcessed(Mat img)
    {
        imgProcessed = img;
    }

}

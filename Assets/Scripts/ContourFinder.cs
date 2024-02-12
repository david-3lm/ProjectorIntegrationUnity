using OpenCvSharp;
using UnityEngine;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ContourFinder : WebCam
{
    [Header("Contour Attributes")]
    [SerializeField] private float threshold = 90f;
    [SerializeField] private bool ShowProcessedImg = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float minArea = 5000f;

    private Mat img;
    private Mat processedImg = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;

    //[Header("Camera")]
    //public WebCam webCam;

    Thread t;
    bool threadStarted=false;
    Camera cam;

    //Limit Check
    int minX,maxX,minY,maxY;

    [SerializeField] ChangeScene changerScene;

    private void Awake()
    {
        if (!Limits.Instance)
        {
            changerScene.Changer();
            return; 
        }
        base.Awake();
        cam = Camera.main;

        //Adjust the limits
        minX =Limits.Instance.valuesX.Min();
        minY =Limits.Instance.valuesY.Min();
        maxX =Limits.Instance.valuesX.Max();
        maxY =Limits.Instance.valuesY.Max();

        t = new Thread(ThreadMethod);
        //ProcessTexture();
    }
    private void Update()
    {
        base.Update();
        ProcessTexture();
    }
    private void OnApplicationQuit()
    {
        t.Interrupt();
    }
    /// <summary>
    /// Method executed by a thread that calls ValidateCenters
    /// </summary>
    private void ThreadMethod()
    {
        while (t.IsAlive)
            ValidateThread();
    }
    /// <summary>
    /// Gets the a list of detected centers and validates them inside the limits
    /// </summary>
    public void ValidateThread()
    {
        if (contours == null || imgHilo) return;

        imgHilo = true;
    }

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

    private void ClampPoint(Point p, int minX, int maxX, int minY, int maxY)
    {
        p.X=Mathf.Clamp(p.X, minX, maxX);
        p.Y=Mathf.Clamp(p.Y, minY, maxY);
    }
    /// <summary>
    /// Gets the image from the camera and calculates the centers (inside the game) of the interactors in the real world.
    /// </summary>
    protected void ProcessTexture()
    {
        img = imgWebCam;
        if (!threadStarted)
        {
            t.Start();
            threadStarted = true;
        }
        if (img != null)
        {


            Cv2.CvtColor(img, processedImg, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(processedImg, processedImg, new Size(5,5), 0);
            Cv2.Threshold(processedImg, processedImg, threshold, 255, ThresholdTypes.BinaryInv);
            Cv2.FindContours(processedImg, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);


            SetImgProcessed(processedImg);

        }
    }
    /// <summary>
    /// Gets the list of contours returned by OpenCV and returns a list of centers.
    /// </summary>
    /// <param name="processedImg"></param>
    /// <param name="contours">Contours of the objects detected by OpenCV</param>
    /// <param name="centersList">The list of centers</param>

    private Vector2[] PointsToVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
            vectorList[i]=cam.ScreenToWorldPoint(vectorList[i]);
        }
        return vectorList;
    }

    /// <summary>
    /// Function that gets the color at a certain coord in the camera. If black there is an object.
    /// </summary>
    /// <param name="X">Coord X in the camera</param>
    /// <param name="Y">Coord Y in the camera</param>
    /// <returns></returns>
    public Vector3Int GetColorAt(float X, float Y)
    {
        int iX, iY;

        //Lerp to change screen coords to projection coords using Limits.
        float lerpX = X / Screen.width;
        float lerpY = Y / Screen.height;
        iX = (int) Mathf.Lerp(minX, maxX, lerpX);
        iY = (int) Mathf.Lerp(minY, maxY, lerpY);
        Vec3b color = processedImg.At<Vec3b>(iY, iX);

        return new Vector3Int(color[0], color[1]);
    }

}

using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;
using UnityEngine.Windows;
using System.Threading;
using System.Diagnostics;
using static ArucoUnity.Plugin.Std;
using static UnityEngine.UIElements.VisualElement;
using System.Collections.Generic;
using System.Linq;

public class ColorMask : WebCamera
{
    private Mat img;
    private Mat processedImg = new Mat();
    [SerializeField] private bool ShowProcessedImg = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float minArea = 5000f;
    [SerializeField] private PolygonCollider2D collider2D;


    static double [,] d= { { 50, 100, 100} };
    static double [,] u= { { 100, 255, 255 } };
    Mat lower = new Mat(3,1,MatType.CV_64F,d);
    Mat upper = new Mat(3,1,MatType.CV_64F,u);

    //CONTOURS
    double area;
    Point[] points;
    private Point[][] contours;
    Thread t;
    bool newIMGReady;
    bool threadStarted = false;
    private Vector2[] vectorList;
    private HierarchyIndex[] hierarchy;

    //LIMITS
    public int minX, minY, maxX, maxY;

    //webcam


    void Awake()
    {
        base.Awake();
        t = new Thread(new ThreadStart(ThreadMethod));
        ProcessTexture(webCamTexture, ref renderedTexture);


    }
    private void Update()
    {
        base.Update();
        //ThreadMethod();
    }
    private void ThreadMethod()
    {
        //while (t.IsAlive)
        //{
            ProcessTexture(webCamTexture,ref renderedTexture);
            print("update");
            ProcessContour();
            newIMGReady = true;

       // }
    }
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {


        //img = OpenCvSharp.Unity.TextureToMat(input);
        img = imgWebCam;

        if (!threadStarted)
        {
            t.Start();
            threadStarted = true;
        }

        Cv2.CvtColor(img, processedImg, ColorConversionCodes.BGR2HSV);
        InputArray lowerBound = new InputArray(lower);
        InputArray upperBound = new InputArray(upper);
        Cv2.InRange(processedImg, lowerBound, upperBound, processedImg);
        Cv2.FindContours(processedImg, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);
        ProcessContour();

        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(ShowProcessedImg? processedImg :img);
        else
            OpenCvSharp.Unity.MatToTexture(ShowProcessedImg? processedImg:img, output);
        return true;
    }

    // Start is called before the first frame update
    private Vector2[] PointsToVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
        }
        return vectorList;
    }

    public void ProcessContour()
    {
        foreach (Point[] contour in contours)
        {
            points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            area = Cv2.ContourArea(contour);


            if (area > minArea)
            {
                DrawContour(processedImg, new Scalar(127, 127, 127), 20, points);

            }
        }
    }
    private void DrawContour(Mat image, Scalar color, int thickness, Point[] points)
    {
        for (int i = 1; i < points.Length; i++)
        {
            Cv2.Line(image, points[i - 1], points[i], color, thickness);
        }
        Cv2.Line(image, points[points.Length - 1], points[0], color, thickness);
    }

    public void GetLimits()
    {
        List<int> valuesX =  new List<int>();
        List<int> valuesY =  new List<int>();
        foreach (Point[] contour in contours)
        {
            points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            area = Cv2.ContourArea(contour);


            if (area > minArea)
            {
                foreach (var p in points)
                {
                    valuesX.Add(p.X);
                    valuesY.Add(p.Y);
                }

            }
        }

        minX = valuesX.Min();
        minY = valuesY.Min();
        maxX = valuesX.Max();
        maxY = valuesY.Max();

    }
}

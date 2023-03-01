using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;
using UnityEngine.Windows;
using System.Threading;

public class ContourFinder : WebCamera
{
    [SerializeField] private float threshold = 90f;
    [SerializeField] private bool ShowProcessedImg = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float minArea = 5000f;
    [SerializeField] private PolygonCollider2D collider2D;

    private Mat img;
    private Mat processedImg = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;

    double area;
    Point[] points;
    bool newIMGReady;
    Thread t;
    bool threadStarted=false;

    [SerializeField]ColorMask mask;

    private void Awake()
    {
        base.Awake();
        t = new Thread(ThreadMethod);
    }
    private void ThreadMethod()
    {
        while (t.IsAlive)
        {
            ProcessContour();
            newIMGReady = true;
            //Thread.Sleep(1000);
        }
    }

    public void ProcessContour()
    {
        foreach (Point[] contour in contours)
        {
            points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            area = Cv2.ContourArea(contour);


            if (area > minArea)
            {
                bool validated = true;
                foreach(Point p in points)
                {
                    if (p.X <= mask.minX || p.X >= mask.maxX || p.Y <= mask.minY || p.Y >= mask.maxY) validated= false;

                }
                
                if(validated) drawContour(processedImg, new Scalar(127, 127, 127), 20, points);

            }
        }
    }
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        img = OpenCvSharp.Unity.TextureToMat(input);
        Cv2.CvtColor(img, processedImg, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(processedImg, processedImg, threshold, 255, ThresholdTypes.BinaryInv);
        Cv2.FindContours(processedImg, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

        if (!threadStarted)
        {
            t.Start();
            threadStarted = true;
        }

        //ProcessContour();
        if (newIMGReady)
        {
            
            collider2D.pathCount =0;
            
            collider2D.pathCount++;
            collider2D.SetPath(collider2D.pathCount - 1, PointsToVector2(points));
            newIMGReady= false;
            Debug.Log(collider2D.pathCount);
        }
        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(ShowProcessedImg ? processedImg : img);
        else
            OpenCvSharp.Unity.MatToTexture(ShowProcessedImg ? processedImg : img, output);
        return true;
    }
    private Vector2[] PointsToVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
        }
        return vectorList;
    }
    private void drawContour(Mat image, Scalar color, int thickness, Point[] points)
    {
        for (int i = 1; i < points.Length; i++)
        {
            Cv2.Line(image, points[i - 1], points[i], color, thickness);
        }
        Cv2.Line(image, points[points.Length - 1], points[0], color, thickness);
    }
}

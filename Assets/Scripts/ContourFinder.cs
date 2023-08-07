using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;
using UnityEngine.Windows;
using System.Threading;
using UnityEditor.EditorTools;
using UnityEditor;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ContourFinder : MonoBehaviour
{
    [Header("Contour Attributes")]
    [SerializeField] private float threshold = 90f;
    [SerializeField] private bool ShowProcessedImg = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float minArea = 5000f;
    [SerializeField] private PolygonCollider2D collider2D;
    [SerializeField] private PolygonCollider2D aux = new PolygonCollider2D();
    [Header("Color Mask")]
    [SerializeField] private Color color = Color.green;

    [SerializeField] private bool maskByColor = false;

    private Mat img;
    private Mat processedImg = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;

    [Header("Camera")]
    public WebCam webCam;

    double area;
    Point[] points;

    bool newIMGReady =false;
    Thread t;
    bool threadStarted=false;
    Camera cam;

    //Limit Check
    List<Point> validatedPoints;
    int minX,maxX,minY,maxY;


    // [SerializeField]ColorMask mask;
    Mat lower, upper;



    private void Awake()
    {
        double[,] d = { { 50, 100, 100 } };
        double[,] u = { { 100, 255, 255 } };
        lower = new Mat(3, 1, MatType.CV_64F, d);
        upper = new Mat(3, 1, MatType.CV_64F, u);
        cam = Camera.main;


        //Adjust the limits
        validatedPoints = new List<Point>();

        minX =Limits.Instance.valuesX.Min();
        minY =Limits.Instance.valuesY.Min();
        maxX =Limits.Instance.valuesX.Max();
        maxY =Limits.Instance.valuesY.Max();

        //float valueX = Mathf.InverseLerp(0,1920,minX);
        //float valueY = Mathf.InverseLerp(0, 1080, minY);


        //Debug.Log(minX);
        //Debug.Log(valueX);

        //if(collider2D.TryGetComponent<RectTransform>(out var rectTransform))
        //{
        //    rectTransform.pivot = new Vector2(valueX,valueY);
        //    Debug.Log("ey");
        //}


        t = new Thread(ThreadMethod);
        if (maskByColor)
        {
            ProcessTextureByColor();
        }
        else
        {
            ProcessTexture();
        }
    }
    private void Update()
    {
        if (maskByColor)
        {
            ProcessTextureByColor();
        }
        else
        {
            ProcessTexture();
        }
        
    }
    private void OnApplicationQuit()
    {
        t.Interrupt();
    }
    private void ThreadMethod()
    {

        while (t.IsAlive)
        {
            //webCam.CallProcessTexture();


            ProcessContour();
            //Thread.Sleep(1000);
            newIMGReady = true;


        }
    }

    public void ProcessContour()
    {
        if (contours == null || webCam.imgHilo) return;
        Mat mat = processedImg;
        foreach (Point[] contour in contours)
        {
            points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            area = Cv2.ContourArea(contour);

            if (area > minArea)
            {
                bool validated = true;
                foreach(Point p in points)
                {
                    //Check if the point is inside the limits
                    if (p.X <= minX || p.X >= maxX || p.Y <= minY || p.Y >= maxY)
                        validated = false;
                    else
                        validatedPoints.Add(p);

                }

                if(validated) 
                    drawContour(mat, new Scalar(127, 127, 127), 20, validatedPoints.ToArray());
                


            }
        }
        webCam.imgHilo = true;


    }
    protected void ProcessTexture()
    {
        img = webCam.imgWebCam;
        if (!threadStarted)
        {
            t.Start();
            threadStarted = true;
        }
        if (img != null)
        {


            Cv2.CvtColor(img, processedImg, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(processedImg, processedImg, threshold, 255, ThresholdTypes.BinaryInv);
            Cv2.FindContours(processedImg, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);



            //ProcessContour();
            if (newIMGReady)
            {

                //collider2D.pathCount = 0;

                //collider2D.pathCount++;
                //collider2D.SetPath(collider2D.pathCount - 1, PointsToVector2(points));
                //newIMGReady = false;
                //Debug.Log(collider2D.pathCount);
                if (validatedPoints != null)
                {
                    drawCollider(collider2D, validatedPoints.ToArray());
                    validatedPoints.Clear();
                }
            }
            webCam.setImgProcessed(processedImg);
        }
    }

    protected void ProcessTextureByColor()
    {
        img = webCam.imgWebCam;
        if (!threadStarted)
        {
            t.Start();
            threadStarted = true;
        }
        if (img != null)
        {
            //float h, s, v;
            //Color.RGBToHSV(color, out h, out s, out v);


            Cv2.CvtColor(img, processedImg, ColorConversionCodes.BGR2HSV);
            InputArray lowerBound = new InputArray(lower);
            InputArray upperBound = new InputArray(upper);
            Cv2.InRange(processedImg, lowerBound, upperBound, processedImg);
            
            Cv2.FindContours(processedImg, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);



            //ProcessContour();
            if (newIMGReady)
            {

                //collider2D.pathCount = 0;

                //collider2D.pathCount++;
                //collider2D.SetPath(collider2D.pathCount - 1, PointsToVector2(points));
                //newIMGReady = false;
                //Debug.Log(collider2D.pathCount);
                if (points != null)
                {
                    drawCollider(collider2D, points);
                }
            }
            webCam.setImgProcessed(processedImg);
        }
    }
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
    private void drawContour(Mat image, Scalar color, int thickness, Point[] points)
    {
        for (int i = 1; i < points.Length; i++)
        {
            Cv2.Line(image, points[i - 1], points[i], color, thickness);
            
        }
        Cv2.Line(image, points[points.Length - 1], points[0], color, thickness);
    }

    private void drawCollider(PolygonCollider2D collider2D, Point[] points)
    {

        aux.pathCount++;
        aux.SetPath(aux.pathCount - 1, PointsToVector2(points));
        if (aux.GetPath(aux.pathCount - 1).Length >= 50)
        {
            collider2D.pathCount = 0;
            collider2D.pathCount++;
            
            collider2D.SetPath(collider2D.pathCount - 1, PointsToVector2(points));
            aux.pathCount = 0;
        }
        newIMGReady = false;
    }
}

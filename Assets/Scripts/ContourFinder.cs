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
using System;

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
    private List<Point> centers;
    private List<Point> centersAux;
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

    //Interactor 
    [SerializeField] GameObject interactor;
    private List<GameObject> interactorPool = new List<GameObject>();


    private void Awake()
    {
        double[,] d = { { 50, 100, 100 } };
        double[,] u = { { 100, 255, 255 } };
        lower = new Mat(3, 1, MatType.CV_64F, d);
        upper = new Mat(3, 1, MatType.CV_64F, u);
        cam = Camera.main;

        centers = new List<Point>();
        centersAux = new List<Point>();


        //Adjust the limits
        validatedPoints = new List<Point>();

        minX =Limits.Instance.valuesX.Min();
        minY =Limits.Instance.valuesY.Min();
        maxX =Limits.Instance.valuesX.Max();
        maxY =Limits.Instance.valuesY.Max();
        
        for (int i = 0;i <50; i++)
        {
            interactorPool.Add(Instantiate(interactor,new Vector3(1000,1000,0),Quaternion.identity));
        }

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
        /*
        foreach (Point[] contour in contours)
        {
            points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            area = Cv2.ContourArea(contour);

            if (area > minArea)
            {
                bool validated = true;
         

                foreach(Point p in points)
                {
                    ////Check if the point is inside the limits
                    //if (p.X <= minX || p.X >= maxX || p.Y <= minY || p.Y >= maxY)
                    //{
                    //    validated = false;
                    //    ClampPoint(p, minX, maxX, minY, maxY);
                    //}



                    //validatedPoints.Add(p);

                }

                //Validate the centers
                //foreach(Point p in centers)
                //{

                //    if (p.X <= minX || p.X >= maxX || p.Y <= minY || p.Y >= maxY)
                //    {
                //        validated = false;

                //    }
                //}
                //if (validated)
                //{

                //    //drawContour(mat, new Scalar(127, 127, 127), 20, validatedPoints.ToArray());
                //    //drawContour(mat, new Scalar(127, 127, 127), 20,points);
                //    drawCenters(mat, new Scalar(127, 127, 127), 20,centers);
                    
                //} 
                

        
            }

        }
        */

        if (centers.Count < 1) return;
        //Validate the centers
        List<Point> centerAux = new List<Point>();
        centerAux.AddRange(centers);
        List<Point> validatedCenters = new List<Point>();
        foreach (Point p in centerAux)
        {
            bool isInside = true;
            if (p.X <= minX || p.X >= maxX || p.Y <= minY || p.Y >= maxY)
            {
                isInside = false;
            }
            if(isInside)
            {
                validatedCenters.Add(p);
                Debug.Log(p.X+" "+ p.Y);
            }
        }

        drawCenters(mat, new Scalar(127, 127, 127), 20, validatedCenters);
        webCam.imgHilo = true;


    }


    private void ClampPoint(Point p, int minX, int maxX, int minY, int maxY)
    {
        p.X=Mathf.Clamp(p.X, minX, maxX);
        p.Y=Mathf.Clamp(p.Y, minY, maxY);
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
            Cv2.GaussianBlur(processedImg, processedImg, new Size(5,5), 0);
            Cv2.Threshold(processedImg, processedImg, threshold, 255, ThresholdTypes.BinaryInv);
            Cv2.FindContours(processedImg, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

            ComputeCenter(processedImg, contours, out centers);
            

            //ProcessContour();
            if (newIMGReady)
            {

                //collider2D.pathCount = 0;

                //collider2D.pathCount++;
                //collider2D.SetPath(collider2D.pathCount - 1, PointsToVector2(points));
                //newIMGReady = false;
                //Debug.Log(collider2D.pathCount);




                //if (validatedPoints != null)
                //{
                //    drawCollider(collider2D, validatedPoints.ToArray());
                //    validatedPoints.Clear();
                //}
                if (points != null)
                {
                    drawCollider(collider2D, points);
                    //validatedPoints.Clear();
                }
                if (centers.Count > 0)
                {
                    SetColliders(interactor, centers);
                }
            }
            webCam.setImgProcessed(processedImg);
        }
    }

    private void ComputeCenter(Mat processedImg, Point[][] contours, out List<Point> centersList)
    {
        Moments m;
        centersList = centers;
        centersList.Clear();
        foreach (var cont in contours)
        {
            double area= Cv2.ContourArea(cont);
            if (area > minArea)
            {
                m = Cv2.Moments(cont);
                Point c = new(m.M10 / m.M00, m.M01 / m.M00);
                if (!centersAux.Contains(c))
                {
                    centersList.Add(c);
                }
            }
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

    private void drawCenters(Mat image, Scalar color, int thickness, List<Point> centers)
    {
        for (int i = 1; i < centers.Count; i++)
        {
            Cv2.Circle(image, centers[i],6,color,thickness);
        }
        centersAux.Clear();
        centersAux = centers;
        centers.Clear();
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

    //Take collider and set it to the position of the centers
    private void SetColliders(GameObject interactor, List<Point> centers)
    {
        RestartCollider();
        for (int i = 0; i < centers.Count; i++)
        {
            if (i >= interactorPool.Count) break;
            Vector2 aux = cam.ScreenToWorldPoint(new Vector3(centers[i].X, (centers[i].Y), 0));
            aux.y = -aux.y;
            interactorPool[i].transform.position =aux;

        }
    }

    private void RestartCollider()
    {
        foreach(GameObject go in interactorPool)
        {
            go.transform.position = new Vector3(1000, 1000, 0);

        }
    }
}

using OpenCvSharp;
using UnityEngine;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

public class ContourFinder : MonoBehaviour
{
    [Header("Contour Attributes")]
    [SerializeField] private float threshold = 90f;
    [SerializeField] private bool ShowProcessedImg = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float minArea = 5000f;

    private Mat img;
    private Mat processedImg = new Mat();
    private Point[][] contours;
    private List<Point> centers;
    private List<Point> centersAux;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;

    [Header("Camera")]
    public WebCam webCam;

    Thread t;
    bool threadStarted=false;
    Camera cam;

    //Limit Check
    int minX,maxX,minY,maxY;

    private void Awake()
    {
        cam = Camera.main;

        centers = new List<Point>();
        centersAux = new List<Point>();


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
            ValidateCenters();
    }
    /// <summary>
    /// Gets the a list of detected centers and validates them inside the limits
    /// </summary>
    public void ValidateCenters()
    {
        if (contours == null || webCam.imgHilo) return;
        Mat mat = processedImg;

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
            }
        }

        DrawCenters(mat, new Scalar(127, 127, 127), 20, validatedCenters);
        webCam.imgHilo = true;
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

            webCam.setImgProcessed(processedImg);
        }
    }
    /// <summary>
    /// Gets the list of contours returned by OpenCV and returns a list of centers.
    /// </summary>
    /// <param name="processedImg"></param>
    /// <param name="contours">Contours of the objects detected by OpenCV</param>
    /// <param name="centersList">The list of centers</param>
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
    private void DrawCenters(Mat image, Scalar color, int thickness, List<Point> centers)
    {
        for (int i = 1; i < centers.Count; i++)
        {
            Cv2.Circle(image, centers[i],6,color,thickness);
        }
        centersAux.Clear();
        centersAux = centers;
        centers.Clear();
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

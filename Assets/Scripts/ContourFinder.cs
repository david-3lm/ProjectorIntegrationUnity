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
    [SerializeField] private float threshold = 240f;
    [SerializeField] private bool ShowProcessedImg = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float minArea = 10000f;

    private Mat img;
    private Mat processedImg = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;


    Thread t;
    bool threadStarted=false;
    Camera cam;

    List<Point> centers;
    List<Point> centersAux;
    Point interactorCenter;
    Point c = new Point();

    //Limit Check
    int minX,maxX,minY,maxY;

    [Header("Interactor")]
    [SerializeField] bool DebugMode = false;
    [SerializeField]Interactor interactor;
    [SerializeField] float distanceInteractables = 1f;

    [Header("Object used to go to calibration scene")]
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

        centers = new List<Point>();
        centersAux = new List<Point>();

        //Adjust the limits
        minX =Limits.Instance.valuesX.Min();
        minY =Limits.Instance.valuesY.Min();
        maxX =Limits.Instance.valuesX.Max();
        maxY =Limits.Instance.valuesY.Max();

        t = new Thread(ThreadMethod);

        if (!interactor)
            interactor = FindObjectOfType<Interactor>();
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

            interactorCenter = ComputeCenter(contours);
            Cv2.Circle(processedImg, interactorCenter, 6, new Scalar(50,10), 10);
            SetImgProcessed(processedImg);

        }
    }

    /// <summary>
    /// Gets the list of contours returned by OpenCV and returns a list of centers.
    /// </summary>
    /// <param name="processedImg"></param>
    /// <param name="contours">Contours of the objects detected by OpenCV</param>
    /// <param name="centersList">The list of centers</param>
    private Point ComputeCenter(Point[][] contours)
    {
        Moments m;
        double biggestArea = minArea;

        for (int i = 1; i < contours.Length; i++)
        {
            double area = Cv2.ContourArea(contours[i]);
            if (area > minArea && area > biggestArea)
            {
                biggestArea = area;
                m = Cv2.Moments(contours[i]);
                double X =  m.M10 / m.M00;
                double Y = m.M01 / m.M00;
                //Checks if the contour is filled
                if (processedImg.At<Vec3b>((int)Y, (int)X)[0] == 0)
                {
                    c = new(X, Y);
                }
            }
        }
        //Debug.Log("X: " + c.X + " Y: " + c.Y);
        return c;
    }


    private Vector2[] PointsToVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
            vectorList[i] = cam.ScreenToWorldPoint(vectorList[i]);
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
        bool inside = false;

        //Lerp to change screen coords to projection coords using Limits.
        LerpVirtToCam(ref X, ref Y);
        Vec3b color = processedImg.At<Vec3b>((int)Y, (int)X);
        if (color[0] == 0) 
            inside = CheckPointInContour((int)X,(int)Y);
        if (inside)
            return new Vector3Int(color[0], color[1]);
        return new Vector3Int(255, 255);
    }

    public void LerpVirtToCam(ref float X, ref float Y)
    {
        float lerpX = X / Screen.width;
        float lerpY = Y / Screen.height;
        X = (int)Mathf.Lerp(minX, maxX, lerpX);
        Y = (int)Mathf.Lerp(minY, maxY, 1 - lerpY);
    }

    public void LerpCamToVirtual(ref float X, ref float Y)
    {
        float proportionX = (X - minX) / (maxX - minX);
        float proportionY = (Y - minY) / (maxY - minY);

        X = proportionX * Screen.width;
        Y = (1 - proportionY) * Screen.height;
    }
    /// <summary>
    /// Function that checks if the point is inside a contour, to avoid functionallity not desired
    /// </summary>
    /// <param name="X">Coord X</param>
    /// <param name="Y">Coord Y</param>
    /// <returns></returns>
    private bool CheckPointInContour(int X, int Y)
    {
        bool inside = false;

        foreach (var c in contours)
        {
            if (Cv2.PointPolygonTest(c, new Point2f(X, Y), false) >= 0)
                inside = (Cv2.ContourArea(c) >= minArea);
        }
        return (inside);
    }

    public void GetCenterData(ref Vector3 pos, ref float size)
    {
        Vector2 vec2;
        if (!DebugMode)
        {
            vec2 = new Vector2(interactorCenter.X, interactorCenter.Y);
            LerpCamToVirtual(ref vec2.x, ref vec2.y);
        }

        //pos = new Vector3(cam.ScreenToWorldPoint(vec2).x, cam.ScreenToWorldPoint(vec2).y, -9);
        //////////////////////DEBUG WITH MOUSE////////////////////////////////
        else
            vec2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        pos = cam.ScreenToWorldPoint(vec2) + (cam.transform.forward * distanceInteractables);
        size = 0.7f;
    }
}

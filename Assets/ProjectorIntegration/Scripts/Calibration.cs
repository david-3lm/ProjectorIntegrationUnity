using OpenCvSharp;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ProjectorIntegration
{
    public class Calibration : WebCam
    {
        #region Variables
        //Variables used for the camera
        private Mat img;
        private Mat processedImg = new Mat();
        [SerializeField] private bool ShowProcessedImg = true;
        [SerializeField] private float CurveAccuracy = 50f;
        [SerializeField] private float minArea = 10000f;

        //Variables for color mask in HSV lower and upper bound 0-50 red 50-100 green
        static double[,] d = { { 50, 100, 100 } };
        static double[,] u = { { 100, 255, 255 } };
        Mat lower = new Mat(3, 1, MatType.CV_64F, d);
        Mat upper = new Mat(3, 1, MatType.CV_64F, u);
        InputArray lowerBound;
        InputArray upperBound;


        //CONTOURS
        double area;
        Point[] points;
        private Point[][] contours;
        private HierarchyIndex[] hierarchy;
        int countContours;

        //Limits used to, get the boundings of the screen
        public int minX, minY, maxX, maxY;
        int timeWithLimits = 0;

        //String used to Change the scene after 5 seconds with the calibration done
        [SerializeField] string nextSceneString;

        #endregion

        private new void Awake()
        {
            base.Awake();
            lowerBound = new InputArray(lower);
            upperBound = new InputArray(upper);
        }

        void Start()
        {
            Limits.Instance.InitializeLists();
        }

        private new void Update()
        {
            base.Update();
            ProcessTexture(webCamTexture, ref renderedTexture);
        }

        ///<summary>
        ///Get the image from the camera, converts it to a texture, and uses that texture to mask a color and find the contours in the image.
        ///</summary>
        protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            img = OpenCvSharp.Unity.TextureToMat(input);

            Cv2.CvtColor(img, processedImg, ColorConversionCodes.BGR2HSV);
            Cv2.InRange(processedImg, lowerBound, upperBound, processedImg);
            Cv2.FindContours(processedImg, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);
            ProcessContour();

            if (output == null)
                output = OpenCvSharp.Unity.MatToTexture(ShowProcessedImg ? processedImg : img);
            else
                OpenCvSharp.Unity.MatToTexture(ShowProcessedImg ? processedImg : img, output);
            return true;
        }

        ///<summary>
        ///Gets the contours and with that information draws the objects with an specific area.
        ///</summary>
        public void ProcessContour()
        {
            countContours = 0;
            foreach (Point[] contour in contours)
            {
                points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
                area = Cv2.ContourArea(contour);

                if (area > minArea)
                {
                    DrawContour(processedImg, new Scalar(127, 127, 127), 20, points);
                    countContours++;
                    GetLimits();
                }
            }
            TimeCalibration();
        }
        ///<summary>
        ///Gets the contours with specific area and draaws them for visualization
        ///</summary>
        private void DrawContour(Mat image, Scalar color, int thickness, Point[] points)
        {
            for (int i = 1; i < points.Length; i++)
            {
                Cv2.Line(image, points[i - 1], points[i], color, thickness);
            }
            Cv2.Line(image, points[points.Length - 1], points[0], color, thickness);
        }

        ///<summary>
        ///Gets the limits from the contours and saves the values in a Singleton called limits.
        ///</summary>
        public void GetLimits()
        {
            foreach (var p in points)
            {
                Limits.Instance.AddValues(p.X, p.Y);
            }

            if (Limits.Instance.valuesX.Count > 0)
            {
                Limits.Instance.GetLimts(out minX, out maxX, out minY, out maxY);
            }
        }

        //After 5 seconds with the 2 squares detected, it changes scenes.
        private void TimeCalibration()
        {
            if (countContours == 2)
            {
                timeWithLimits++;
                if (timeWithLimits > 5)
                {
                    webCamTexture.Stop();
                    webCamTexture = null;
                    SceneManager.LoadScene(nextSceneString);
                }
            }
            else
            {
                timeWithLimits = 0;
            }
        }
    }
}
using OpenCvSharp;
using UnityEngine;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Windows.WebCam;

namespace ProjectorIntegration
{
    public class ContourFinder : WebCam
    {
        #region Variables

        [Header("Contour Attributes")]
        [SerializeField] private float threshold = 240f;
        [SerializeField] private float minArea = 10000f;

        //OpenCV Image variables
        private Mat img;
        private Mat processedImg = new Mat();
        private Point[][] contours;
        private HierarchyIndex[] hierarchy;
        private Vector2[] vectorList;

        Camera cam;

        //Thread variables
        Thread t;
        bool threadStarted=false;

        //Centers variables
        Point interactorCenter;
        Point c = new Point();

        //Limit Check
        int minX,maxX,minY,maxY;

        [Header("Interactor")]
        [SerializeField] bool DebugMode = false; //True = use the interactor with MousePosition
        [SerializeField] Interactor interactor;
        [SerializeField] float distanceInteractables = 1f; 

        [Header("Object used to go to calibration scene")]
        [SerializeField] string CalibrationSceneName = "CalibrationScene";

        #endregion

        private new void Awake()
        {
            if (!Limits.Instance)
            {
                SceneManager.LoadScene(CalibrationSceneName);
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

            if (!interactor)
                interactor = FindObjectOfType<Interactor>();
        }
        private new void Update()
        {
            base.Update();
            ProcessTexture();
        }

        private void OnApplicationQuit()
        {
            t.Interrupt();
        }

        /// <summary>
        /// Method executed by a thread that calls ValidateThread
        /// </summary>
        private void ThreadMethod()
        {
            while (t.IsAlive)
                ValidateThread();
        }

        /// <summary>
        /// Checks if the last image processing has ended and if it is detecting any contour.
        /// If true, the next iteration can proceed
        /// </summary>
        public void ValidateThread()
        {
            if (contours == null || imgHilo) return;

            imgHilo = true;
        }

        /// <summary>
        /// Override of the OpenCV class
        /// Process current texture
        /// </summary>
        /// <param name="input">WebCam texture</param>
        /// <param name="output">Processed texture</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the image and processes it with a B/W threshold.
        /// finds the contour and the center of the biggest area.
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
                SetImgProcessed(processedImg);
            }
        }

        /// <summary>
        /// Gets a list of contours and returns the center point of the biggest light area of the camera.
        /// </summary>
        /// <param name="contours">List of contours</param>
        /// <returns></returns>
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

                    //Checks if the contour is filled to avoid some problems
                    if (processedImg.At<Vec3b>((int)Y, (int)X)[0] == 0)
                    {
                        c = new(X, Y);
                    }
                }
            }
            return c;
        }

        /// <summary>
        /// Lerps the position of the WebCam to the position in the virtual camera
        /// </summary>
        /// <param name="X">X position in WebCam</param>
        /// <param name="Y">Y position in WebCam</param>
        public void LerpCamToVirtual(ref float X, ref float Y)
        {
            float proportionX = (X - minX) / (maxX - minX);
            float proportionY = (Y - minY) / (maxY - minY);

            X = proportionX * Screen.width;
            Y = (1 - proportionY) * Screen.height;
        }

        /// <summary>
        /// Returns the position transformed from webcam to virtual
        /// It is called from Interactor to get size and position of the collider.
        /// </summary>
        /// <param name="pos">Reference of a Vec3 where the position is saved</param>
        /// <param name="size">Size of the collider</param>
        public void GetCenterData(ref Vector3 pos, ref float size)
        {
            Vector2 vec2;
            if (!DebugMode)
            {
                vec2 = new Vector2(interactorCenter.X, interactorCenter.Y);
                LerpCamToVirtual(ref vec2.x, ref vec2.y);
            }
            //////////////////////DEBUG WITH MOUSE////////////////////////////////
            else
                vec2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            pos = cam.ScreenToWorldPoint(vec2) + (cam.transform.forward * distanceInteractables);
            size = 0.7f;
        }
    }
}

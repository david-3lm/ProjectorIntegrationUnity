using OpenCvSharp.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] WebCam webCam;
    [SerializeField] string nameScene;

    private void Awake()
    {
        webCam = GameObject.FindGameObjectWithTag("Image").GetComponent<WebCam>();
    }
    public void Changer()
    { 
        Destroy(webCam);
        SceneManager.LoadScene(nameScene); 
    }
    public void ChangerToCalib(GameObject contourFinder)
    {
        Destroy(contourFinder);
        Destroy(webCam);
        SceneManager.LoadScene(nameScene);
    }
}

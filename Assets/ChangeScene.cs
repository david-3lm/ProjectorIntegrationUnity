using OpenCvSharp.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] WebCam webCam;
    public void Changer()
    {
        
        Destroy(webCam );
        SceneManager.LoadScene("SampleScene");
    }
}

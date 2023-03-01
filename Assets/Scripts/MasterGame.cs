using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class MasterGame : MonoBehaviour
{
    [SerializeField] Emitter emitter;
    [SerializeField] ContourFinder contourFinder;
    private Thread threadWebcam;

    private void Awake()
    {
        //threadWebcam = new Thread(()=>contourFinder.CallProcessedTexture());
    }
}

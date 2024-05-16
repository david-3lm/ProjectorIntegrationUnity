using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.UI;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractiveObject : MonoBehaviour
{
    #region Variables
    protected Transform objTransform;

    #endregion

    #region Abstract
    /// <summary>
    /// Abstract function to override with desired behaviiour
    /// </summary>
    public abstract void InteractionEvent();

    // Start is called before the first frame update
    void Start()
    {
        objTransform = transform;
    }

    #endregion


    private void OnTriggerEnter(Collider collision)
    {
        InteractionEvent();
    }
}

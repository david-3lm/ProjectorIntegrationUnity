using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.UI;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class InteractableObject : MonoBehaviour
{
    #region Variables
    private ContourFinder cf;
    private Vector3 camPos;
    Vector3Int col;
    private float cooldown = 1f;
    protected Transform objTransform;

    //TESTING
    public bool button = false;
    bool entered = false;
    //END TESTING
    #endregion

    #region Abstract
    // Start is called before the first frame update
    void Start()
    {
        objTransform = transform;
        cf = GameObject.FindGameObjectWithTag("Image").GetComponent<ContourFinder>();
    }

    private void PositionToCam()
    {
        camPos = Camera.main.WorldToScreenPoint(transform.position);

        //Change reference point
        camPos.y = 1080 - camPos.y;
    }
    #endregion

    public abstract void InteractionEvent();

    private void OnTriggerEnter(Collider collision)
    {
        InteractionEvent();
    }
}

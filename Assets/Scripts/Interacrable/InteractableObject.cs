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

    //// Update is called once per frame
    void Update()
    {
        /*
        PositionToCam();
        col = cf.GetColorAt(camPos.x, camPos.y);
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }
        //If the intereactor is found in the position of the object:
        if (col.x == 0)
        {
            cooldown = 1f;
            InteractionEvent();
        }
        if (button && !entered)
        {
            entered = true;
            InteractionEvent();
        }
        entered = button;*/
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

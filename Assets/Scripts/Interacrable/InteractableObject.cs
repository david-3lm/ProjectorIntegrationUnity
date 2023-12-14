using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private ContourFinder cf;
    private Vector3 camPos;
    Vector3Int col;

    // Start is called before the first frame update
    void Start()
    {
        cf = GameObject.FindGameObjectWithTag("Image").GetComponent<ContourFinder>();
    }

    // Update is called once per frame
    void Update()
    {

        PositionToCam();
        col = cf.GetColorAt(camPos.x, camPos.y);

        //If the intereactor is found in the position of the object:
        if (col.x == 0)
            InteractionEvent();
    }

    private void PositionToCam()
    {
        camPos = Camera.main.WorldToScreenPoint(transform.position);

        //Change reference point
        camPos.y = 1080 - camPos.y;
        Debug.Log("PosCam= " + camPos.x + " " + camPos.y);
    }

    public virtual void InteractionEvent()
    {
        Debug.Log("Ey");
    }
}

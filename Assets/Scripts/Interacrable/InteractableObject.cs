using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
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
        col = cf.GetColorAt((int)camPos.x, (int)camPos.y);
        Debug.Log(col.ToString());
    }

    private void PositionToCam()
    {
        camPos = Camera.main.WorldToScreenPoint(transform.position);
        Debug.Log("Pos= "+ transform.position.x + " "+ transform.position.y);
        Debug.Log("PosCam= "+ camPos.x + " "+ camPos.y);
    }
}

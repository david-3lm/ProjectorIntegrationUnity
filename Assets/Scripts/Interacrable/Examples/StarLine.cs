using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLine : MonoBehaviour
{
    [SerializeField] public Star star1;
    [SerializeField] public Star star2;

    LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, star1.GetComponent<Transform>().position);
        line.SetPosition(1, star2.GetComponent<Transform>().position);
        line.enabled = false;
    }
    private void Update()
    {
        if (star1.activated && star2.activated)
        {
            line.enabled = true;
            line.SetPosition(0, star1.GetComponent<Transform>().position);
            line.SetPosition(1, star2.GetComponent<Transform>().position);
        }
        else if (line.enabled)
            line.enabled = false;
    }
}

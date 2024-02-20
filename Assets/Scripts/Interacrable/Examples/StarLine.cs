using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLine : MonoBehaviour
{
    [SerializeField] Star star1;
    [SerializeField] Star star2;

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
            line.enabled = true;
        else if (line.enabled)
            line.enabled = false;
    }
}

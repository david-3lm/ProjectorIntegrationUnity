using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLine : MonoBehaviour
{
    [SerializeField] Star star1;
    [SerializeField] Star star2;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    private void Update()
    {
        if (star1.activated && star2.activated)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}

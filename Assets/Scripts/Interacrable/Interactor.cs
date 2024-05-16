using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Interactor : MonoBehaviour
{
    [SerializeField]ContourFinder finder;
    SphereCollider col;

    Vector3 pos;
    float size;

    private void Awake()
    {
        if (!finder)
            finder = FindObjectOfType<ContourFinder>();
        col = GetComponent<SphereCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        print("Interactor started");
    }

    // Update is called once per frame
    void Update()
    {
        finder.GetCenterData(ref pos, ref size); //gets the position from the camera
        transform.position = pos;
        col.radius = size;
    }
}

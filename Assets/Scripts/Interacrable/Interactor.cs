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
        finder.GetCenterData(ref pos, ref size);
        //transform.position = new Vector3(pos.x, pos.y, -9);
        transform.position = pos;
        col.radius = size;
        //transform.position = finder.interactorPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Encontre algo");
    }
}

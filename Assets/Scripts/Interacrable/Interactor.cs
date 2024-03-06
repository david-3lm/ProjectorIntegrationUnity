using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactor : MonoBehaviour
{
    [SerializeField]ContourFinder finder;
    CircleCollider2D col;

    private void Awake()
    {
        if (!finder)
            finder = FindObjectOfType<ContourFinder>();
        col = GetComponent<CircleCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        print("Interactor started");
    }

    // Update is called once per frame
    void Update()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, -9);
        col.radius = 0.2f;
        //transform.position = finder.interactorPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Encontre algo");
    }
}

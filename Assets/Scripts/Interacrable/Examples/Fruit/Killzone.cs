using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Killzone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fruit") || collision.gameObject.CompareTag("Bomb"))
            Destroy(collision.gameObject);
    }
}

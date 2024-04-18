using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //0=right/1=center/2=left
    [SerializeField] List<Transform> positions;
    public event Action OnPlayerHit;
    public event Action OnPlayerFruit;
    
    public void MoveTo(int pos)
    {
        Vector3 p = positions[pos].position;
        transform.position = new Vector3(p.x, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
            OnPlayerFruit?.Invoke();
        else if (collision.gameObject.CompareTag("Bomb"))
            OnPlayerHit?.Invoke();
        else
            return;
        Destroy(collision.gameObject);
    }
}

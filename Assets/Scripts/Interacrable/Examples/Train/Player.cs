using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //0=right/1=center/2=left
    [SerializeField] List<Transform> positions;
    
    public void MoveTo(int pos)
    {
        Vector3 p = positions[pos].position;
        transform.position = new Vector3(p.x, transform.position.y, transform.position.z);
    }
}

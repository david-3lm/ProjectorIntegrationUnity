using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPosChanger : InteractableObject
{
    public override void InteractionEvent()
    {
        objTransform.position = new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.5f, 0.5f), 1);
        transform.position = objTransform.position;
    }
}

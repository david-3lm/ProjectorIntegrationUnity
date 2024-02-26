using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Star : InteractableObject
{
    public bool activated;

    private void Awake()
    {
        activated = false;
    }

    public override void InteractionEvent()
    {
        if (activated)
            return;
        activated = true;
        transform.localScale *= 5f;
    }
}

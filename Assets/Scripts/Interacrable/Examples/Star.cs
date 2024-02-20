using System.Collections;
using System.Collections.Generic;
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
        transform.localScale *= 10f;
    }
}

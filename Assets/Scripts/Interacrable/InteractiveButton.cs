using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveButton : InteractiveObject
{
    [SerializeField] UnityEvent onClick;
    public override void InteractionEvent()
    {
        onClick.Invoke();
    }
}

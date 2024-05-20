using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectorIntegration
{
    public class InteractiveButton : InteractiveObject
    {
        [SerializeField] UnityEvent onClick;
        public override void InteractionEvent()
        {
            onClick.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectorIntegration
{
    public class IPosChanger : InteractiveObject
    {
        public override void InteractionEvent()
        {
            objTransform.position = new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.5f, 0.5f), 1);
            transform.position = objTransform.position;
        }
    }
}

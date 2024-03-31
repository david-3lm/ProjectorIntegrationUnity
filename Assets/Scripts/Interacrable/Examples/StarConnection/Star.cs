using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Star : InteractiveObject
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
        StartCoroutine("Anim");
        GetComponent<ParticleSystem>().Play();
    }

    IEnumerator Anim()
    {
        int i = 1;
        Vector3 sum = transform.localScale;
        while (i < 8)
        {
            transform.localScale += sum;
            yield return new WaitForSeconds(0.001f);
            i++;
        }
        while (i > 4)
        {
            transform.localScale -= sum;
            yield return new WaitForSeconds(0.001f);
            i--;
        }
        yield return null;
    }
}

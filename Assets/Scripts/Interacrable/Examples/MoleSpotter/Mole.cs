using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : InteractableObject
{
    bool activated;
    float timeAlive;
    private void Awake()
    {
        activated = false;
        timeAlive = 0;
    }

    public override void InteractionEvent()
    {
        activated = true;
        StartCoroutine("AnimDeath");
    }


    IEnumerator AnimDeath()
    {
        //change Sprite
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
        yield return null;
    }
}

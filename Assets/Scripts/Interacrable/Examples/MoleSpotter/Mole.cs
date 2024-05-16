using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mole : InteractiveObject
{
    float timeAlive;
    MoleGameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<MoleGameManager>();
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
    }
    public override void InteractionEvent()
    {
        StartCoroutine("AnimDeath");
    }

    private void OnDestroy()
    {
        gameManager.MoleSpotted(timeAlive);
    }

    IEnumerator AnimDeath()
    {
        //change Sprite
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
        yield return null;
    }
}

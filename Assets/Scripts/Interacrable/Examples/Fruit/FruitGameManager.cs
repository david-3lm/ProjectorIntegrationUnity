using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGameManager : MonoBehaviour
{
    [SerializeField] GameObject bombPrefab;
    [SerializeField] GameObject fruitPrefab;
    [SerializeField] Player player;
    [SerializeField] List<Transform> spawners;
    [SerializeField] GameObject GameOverUI;
    bool playing;
    float cooldown = 1.5f;

    private void Awake()
    {
        player.OnPlayerHit += GameOver;
        player.OnPlayerFruit += FruitCollected;
        playing = true;
        GameOverUI.SetActive(false);
    }

    private void Update()
    {
        if (playing)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0 )
            {
                Spawn();
                cooldown = 2f;
            }
        }
    }

    private void FruitCollected()
    {
        Debug.Log("Fruit collected");
    }
    private void GameOver()
    {
        GameOverUI.SetActive(true);
        Debug.Log("Muerte");
    }

    private void Spawn()
    {
        int num = UnityEngine.Random.Range(0,100);
        Transform t = spawners[num % 3];
        GameObject go;

        if (num % 2 == 0)
            go = Instantiate(fruitPrefab);
        else
            go = Instantiate(bombPrefab);
        go.transform.position = t.position;
    }
}

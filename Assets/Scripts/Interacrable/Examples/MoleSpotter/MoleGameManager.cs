using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleGameManager : MonoBehaviour
{
    [SerializeField] GameObject molePrefab;
    [SerializeField] List<Transform> positions;
    float MAX_TIME = 30f;
    float COOLDOWN = 3f;
    bool gameOver;
    float count = 0;
    GameObject mole;


    // Start is called before the first frame update
    void Start()
    {
        print("Started");
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.timeSinceLevelLoadAsDouble > MAX_TIME)
        { gameOver = true; return; }

        print(count);
        count += Time.deltaTime;
        if (count >= COOLDOWN)
        {
            mole = Instantiate(molePrefab);
            mole.transform.position = positions[Random.Range(0, positions.Count)].position;
            
            count = 0;
        }
    }
}

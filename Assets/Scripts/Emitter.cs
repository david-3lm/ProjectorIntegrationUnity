using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{

    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnRate = 0.1f;
    [SerializeField] private int maxParticles = 4;
    [SerializeField] private Vector2 sizeRange;

    private GameObject[] pool;

    // Start is called before the first frame update
    void Start()
    {
        InitializePool();
        Spawn();
    }
    private void InitializePool()
    {
        pool = new GameObject[maxParticles];
        for (int i = 0; i < maxParticles; i++)
        {
            var particle = Instantiate(spawnPrefab);
            particle.SetActive(false);
            pool[i] = particle;
        }
    } 

    // Update is called once per frame
    private void Spawn()
    {
        foreach (var particle in pool)
        {
            if (!particle.activeSelf)
            {
                particle.transform.position = transform.TransformPoint(Random.insideUnitSphere * 0.5f);
                particle.transform.localScale= Random.Range(sizeRange.x, sizeRange.y)* Vector3.one;
                particle.SetActive(true);
                break;
            }
        }
        Invoke("Spawn", spawnRate);
        
    }
}

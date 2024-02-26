using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarGameManager : MonoBehaviour
{
    [SerializeField] List<Constellation> constellations;
    [SerializeField] Constellation activeConstellation;
    [SerializeField] GameObject button;
    [SerializeField] Transform startPos;

    private void Start()
    {
        StartGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (activeConstellation.AllStars())
            button.SetActive(true);
    }

    private Constellation GetConstellation()
    {
        return constellations[Random.Range(0, constellations.Count)];
    }

    private void StartGame()
    {
        button.SetActive(false);
        activeConstellation = Instantiate(GetConstellation(), startPos);
    }

    public void RestartGame()
    {
        activeConstellation.RestartGame();
        StartGame();
    }

}

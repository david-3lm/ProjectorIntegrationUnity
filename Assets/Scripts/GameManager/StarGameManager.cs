using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StarGameManager : MonoBehaviour
{
    [SerializeField] List<Constellation> constellations;
    [SerializeField] Constellation activeConstellation;
    [SerializeField] GameObject button;
    [SerializeField] Transform startPos;
    Camera cam;
    int idx = 0;

    private void Start()
    {
        cam = Camera.main;
        StartGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (activeConstellation.AllStars() && Input.GetKeyDown(KeyCode.Space))
            GoNextConstellation();
    }

    private void GoNextConstellation()
    {
        idx++;
        if (idx < constellations.Count)
        {
            activeConstellation = constellations[idx];
            MoveCameraToConstellation();
        }
    }

    private Constellation GetConstellation()
    {
        return constellations[Random.Range(0, constellations.Count)];
    }

    private void StartGame()
    {
        button.SetActive(false);
        activeConstellation = constellations[0];
        MoveCameraToConstellation();
    }

    public void RestartGame()
    {
        activeConstellation.RestartGame();
        StartGame();
    }

    void MoveCameraToConstellation()
    {
        cam.transform.forward = activeConstellation.transform.position - cam.transform.position;
    }

}

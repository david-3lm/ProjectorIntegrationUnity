using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StarGameManager : MonoBehaviour
{
    [SerializeField] List<Constellation> constellations;
    [SerializeField] Constellation activeConstellation;
    [SerializeField] GameObject button;
    [SerializeField] Transform startPos;
    [SerializeField] TextMeshProUGUI text;
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
        StartCoroutine("MoveCameToConstellation");
    }

    IEnumerator MoveCameToConstellation()
    {
        Quaternion startRotation = cam.transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(activeConstellation.transform.position - cam.transform.position);

        float animationDuration = 1f;
        float timeElapsed = 0;

        while (timeElapsed < animationDuration)
        {
            cam.transform.rotation = Quaternion.Lerp(startRotation, endRotation, timeElapsed / animationDuration);
            timeElapsed += Time.deltaTime;
            string formattedText = string.Format("Az: {0:00}°<br>Alt: {1:00}°", cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.x);
            text.text = formattedText;
            yield return null;
        }

        cam.transform.rotation = endRotation;
    }

}

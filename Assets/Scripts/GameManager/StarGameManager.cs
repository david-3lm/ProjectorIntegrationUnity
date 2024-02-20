using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarGameManager : MonoBehaviour
{
    [SerializeField] List<Star> stars;
    [SerializeField] GameObject button;

    private void Start()
    {
        button.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (AllStars())
            button.SetActive(true);
    }

    private bool AllStars()
    {
        foreach (Star star in stars)
        {
            if (!star.activated)
                return false;
        }
        return true;
    }
    public void RestartGame()
    {
        foreach(Star star in stars)
        {
            star.transform.localScale /= 10f;
            star.activated = false;
        }
        button.SetActive(false);
    }
}

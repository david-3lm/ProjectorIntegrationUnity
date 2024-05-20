using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace ProjectorIntegration
{
    public class MoleGameManager : MonoBehaviour
    {
        [SerializeField] GameObject molePrefab;
        [SerializeField] List<Transform> positions;
        [SerializeField] UnityEngine.UI.Slider slider;
        [SerializeField] TextMeshProUGUI pointsUI;
        [SerializeField] TextMeshProUGUI gameOverPointsUI;
        [SerializeField] GameObject GameOverUI;
        float MAX_TIME = 30f;
        float COOLDOWN = 3f;
        bool gameOver;
        float count = 2;
        GameObject mole;
        float points = 0;

        // Start is called before the first frame update
        void Start()
        {
            print("Started");
            gameOver = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (gameOver) 
            {
                if (!GameOverUI.activeInHierarchy)
                {
                    GameOverUI.SetActive(true);
                    gameOverPointsUI.text = ((int)points).ToString();
                }
                return;
            }

            slider.value = 1 - (Time.timeSinceLevelLoad / MAX_TIME);

            if (Time.timeSinceLevelLoadAsDouble > MAX_TIME)
            { gameOver = true; return; }
            pointsUI.text = ((int)points).ToString();
            count += Time.deltaTime;
            if (count >= COOLDOWN)
            {
                SpawnMole();
            }
        }

        public void MoleSpotted(float time)
        {
            points += 100 / time;
            if(!gameOver)
                SpawnMole();
        }

        public void SpawnMole()
        {
            mole = Instantiate(molePrefab);
            mole.transform.position = positions[Random.Range(0, positions.Count)].position;

            count = 0;
        }
    }
}

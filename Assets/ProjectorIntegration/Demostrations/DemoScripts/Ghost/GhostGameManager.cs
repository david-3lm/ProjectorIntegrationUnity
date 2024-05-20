using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProjectorIntegration
{
    public class GhostGameManager : MonoBehaviour
    {
        Ghost ghost;
        [SerializeField] TextMeshProUGUI textScore;
        [SerializeField] GameObject gameOverUI;
        [SerializeField] float time = 30f;
        int score;
        private void Awake()
        {
            gameOverUI.SetActive(false);
            score = 0;
            ghost = FindObjectOfType<Ghost>();
            ghost.hit += GhostHit;
        }

        private void Update()
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                gameOverUI.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            ghost.hit -= GhostHit;
            ghost = null;
        }
        private void GhostHit()
        {
            score += 1;
            textScore.text = score.ToString();
        }
    }
}

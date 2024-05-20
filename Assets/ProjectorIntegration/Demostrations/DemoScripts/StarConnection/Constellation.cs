using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectorIntegration
{
    public class Constellation : MonoBehaviour
    {
        [SerializeField] List<Star> stars;

    
        [SerializeField] GameObject starPrefab;
    
        // Start is called before the first frame update
        void Start()
        {
            foreach (Star star in stars)
            {
                star.enabled = true;
            }
        }

        public bool AllStars()
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
            Destroy(gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectorIntegration
{
    public class Ghost : InteractiveObject
    {

        [SerializeField] Sprite hitSprite;
        [SerializeField] Sprite normalSprite;
        SpriteRenderer sr;
        Vector2 min = new Vector2(-7.56f, -2.7f);
        Vector2 max = new Vector2(7.56f, 4.5f);
        public event Action hit;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            sr.sprite = normalSprite;
            ChangePosition();
        }

        public override void InteractionEvent()
        {
            StartCoroutine("Anim"); 
        }

        IEnumerator Anim()
        {
            sr.sprite = hitSprite;
            hit.Invoke();
            yield return new WaitForSeconds(1f);
            sr.sprite = normalSprite;
            ChangePosition();
        }

        private void ChangePosition()
        {
            float x = UnityEngine.Random.Range(min.x, max.x);
            float y = UnityEngine.Random.Range(min.y, max.y);

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}

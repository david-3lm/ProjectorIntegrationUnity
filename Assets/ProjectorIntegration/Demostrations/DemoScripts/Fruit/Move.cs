using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectorIntegration
{
    enum Position
    {
        right,
        center,
        left
    }
    public class Move : InteractiveObject
    {
        [SerializeField] Position position;
        Player player;


        private void Awake()
        {
            player = FindObjectOfType<Player>();
        }

        public override void InteractionEvent()
        {
            player.MoveTo((int)position);
        }
    }
}

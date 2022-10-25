using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.SetActive(false);
    }
}

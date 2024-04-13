using UnityEngine;
using Unity.Netcode;

public class BulletController : NetworkBehaviour
{
    private float speed = 20f;

    public Rigidbody2D bullet;

    private void Start()
    {
        bullet.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        Debug.Log("PLAYER DAMAGE: " + PlayerController.playerPower);

        if (collision.CompareTag("Boss"))
        {
            BossController boss = collision.GetComponent<BossController>();

            if (boss != null)
            {
                boss.GetDamage(PlayerController.playerPower);
                DestroyBulletServerRpc();
            }
        }
        else if (collision.CompareTag("Background"))
        {
            DestroyBulletServerRpc();
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyBulletServerRpc()
    {
        Destroy(gameObject);
    }
}

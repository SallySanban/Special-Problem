using UnityEngine;
using Unity.Netcode;

public class BulletController : NetworkBehaviour
{
    private float speed = 20f;
    public static int damage = 5;

    public Rigidbody2D bullet;

    private void Start()
    {
        bullet.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        Debug.Log("PLAYER DAMAGE: " + damage);

        if (collision.CompareTag("Boss"))
        {
            BossController boss = collision.GetComponent<BossController>();

            if (boss != null)
            {
                boss.GetDamage(damage);
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

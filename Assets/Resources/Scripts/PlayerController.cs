using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject healthBar;
    [SerializeField] public GameObject bulletPrefab;

    Transform healthBarFill;
    private enum PlayerState
    {
        Idle,
        Walk,
        Hurt,
        Die,
        Revive
    }

    private NetworkVariable<PlayerState> state = new NetworkVariable<PlayerState>();

    private int maxPlayerHealth = 100;
    private NetworkVariable<int> playerHealth = new(0);
    private float maxHealthBarWidth;

    private void Start()
    {
        healthBarFill = healthBar.transform.GetChild(1).transform;
        maxHealthBarWidth = healthBarFill.localScale.x;

        playerHealth.Value = maxPlayerHealth;
    }

    private void Update()
    {
        AnimatePlayer();
        ControlPlayer();
    }

    private void ControlPlayer()
    {
        if (!IsOwner) return;

        Vector3 move = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            healthBar.transform.eulerAngles = new Vector3(0, 0, 0);

            move.x -= 2f;

            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            healthBar.transform.eulerAngles = new Vector3(0, 0, 0);

            move.x += 2f;

            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
        else
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerShootServerRpc();
        }

        float speed = 3f;
        transform.position += move * speed * Time.deltaTime;
    }

    private void AnimatePlayer()
    {
        if(state.Value == PlayerState.Idle)
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", false);
        }
        else if(state.Value == PlayerState.Walk)
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", true);
        }
        else if (state.Value == PlayerState.Hurt)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Hurt");
        }
        else if (state.Value == PlayerState.Die)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Die");

            StartCoroutine(Wait());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetDamageServerRpc(int damage)
    {
        playerHealth.Value = playerHealth.Value - damage;
        Debug.Log("PLAYER HEALTH: " + playerHealth.Value);

        float healthFill = (maxHealthBarWidth / maxPlayerHealth) * playerHealth.Value;
        healthBarFill.localScale = new Vector3(healthFill, healthBarFill.localScale.y, healthBarFill.localScale.z);

        UpdatePlayerStateServerRpc(PlayerState.Hurt);

        if (playerHealth.Value <= 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Die);
        }
    }

    [ServerRpc]
    private void PlayerShootServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerStateServerRpc(PlayerState playerState)
    {
        state.Value = playerState;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);

        DestroyPlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyPlayerServerRpc()
    {
        Destroy(gameObject);
    }
}

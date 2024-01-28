using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject healthBar;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public TextMeshProUGUI nameText;

    Transform healthBarFill;
    private enum PlayerState
    {
        Idle,
        Walk,
        Hurt,
        Die
    }

    private NetworkVariable<PlayerState> state = new NetworkVariable<PlayerState>();

    private int maxPlayerHealth = 100;
    private NetworkVariable<int> playerHealth = new(100);
    private float maxHealthBarWidth;

    //private NetworkVariable<string> playerName = new NetworkVariable<string>();

    private void Start()
    {
        //playerName.Value = Player.playerName;

        //nameText.text = playerName.Value;
        //healthBarFill = healthBar.transform.GetChild(1).transform;
        //maxHealthBarWidth = healthBarFill.localScale.x;
    }

    private void Update()
    {
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

            AnimatePlayerServerRpc("Walk");
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            healthBar.transform.eulerAngles = new Vector3(0, 0, 0);

            move.x += 2f;

            AnimatePlayerServerRpc("Walk");
        }
        else
        {
            AnimatePlayerServerRpc("Idle");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerShootServerRpc();
        }

        float speed = 3f;
        transform.position += move * speed * Time.deltaTime;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AnimatePlayerServerRpc(string animation)
    {
        if(animation == "Idle")
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", false) ;
        }
        else if (animation == "Walk")
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", true);
        }
        else if (animation == "Hurt")
        {
            gameObject.GetComponent<NetworkAnimator>().SetTrigger("Hurt");
        }
        else if (animation == "Die")
        {
            gameObject.GetComponent<NetworkAnimator>().SetTrigger("Die");
            
            StartCoroutine(Wait());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetDamageServerRpc(int damage)
    {
        playerHealth.Value = playerHealth.Value - damage;
        Debug.Log("PLAYER HEALTH: " + playerHealth.Value);

        //float healthFill = (maxHealthBarWidth / maxPlayerHealth) * playerHealth.Value;
        //healthBarFill.localScale = new Vector3(healthFill, healthBarFill.localScale.y, healthBarFill.localScale.z);

        AnimatePlayerServerRpc("Hurt");

        if (playerHealth.Value <= 0)
        {
            AnimatePlayerServerRpc("Die");
        }
    }

    [ServerRpc]
    private void PlayerShootServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
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

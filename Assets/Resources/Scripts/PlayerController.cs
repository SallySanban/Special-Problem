using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject healthBar;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public TextMeshProUGUI nameText;

    Transform healthBarFill;

    private int maxPlayerHealth = 100;
    private NetworkVariable<int> playerHealth = new(100);
    private float maxHealthBarWidth;    
    
    private NetworkVariable<float> playerHealthBarFillValue = new(0);

    public NetworkVariable<bool> playerActive = new(true);

    private void Start()
    {
        healthBarFill = healthBar.transform.GetChild(1).transform;
        maxHealthBarWidth = healthBarFill.localScale.x;
    }

    private void Update()
    {
        ControlPlayer();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) playerHealthBarFillValue.OnValueChanged += DecreaseHealthBar;
    }

    private void ControlPlayer()
    {
        if (!IsOwner) return;

        if (playerActive.Value == false) return;

        if (GameObject.FindGameObjectWithTag("Boss") == null) return;

        Vector3 move = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A))
        {  
            transform.eulerAngles = new Vector3(0, 180, 0);

            move.x -= 2f;

            AnimatePlayerServerRpc("Walk");
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);

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

            RevivePlayerClientRpc();
        }
        else if (animation == "Revive")
        {
            gameObject.GetComponent<NetworkAnimator>().SetTrigger("Revive");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetDamageServerRpc(int damage)
    {
        if(playerHealth.Value > 0)
        {
            playerHealth.Value = playerHealth.Value - damage;
            Debug.Log("PLAYER HEALTH: " + playerHealth.Value);

            playerHealthBarFillValue.Value = (maxHealthBarWidth / maxPlayerHealth) * playerHealth.Value;
            healthBarFill.localScale = new Vector3(playerHealthBarFillValue.Value, healthBarFill.localScale.y, healthBarFill.localScale.z);

            AnimatePlayerServerRpc("Hurt");

            if (playerHealth.Value <= 0)
            {
                AnimatePlayerServerRpc("Die");
            }
        }
    }

    private void DecreaseHealthBar(float previousValue, float newValue)
    {
        healthBarFill.localScale = new Vector3(playerHealthBarFillValue.Value, healthBarFill.localScale.y, healthBarFill.localScale.z);
    }

    [ServerRpc]
    private void PlayerShootServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    private void RevivePlayerClientRpc()
    {
        StartCoroutine(Revive());
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerActiveServerRpc(bool activeStatus)
    {
        playerActive.Value = activeStatus;
    }

    [ServerRpc(RequireOwnership = false)]
    private void MaxPlayerHealthServerRpc()
    {
        playerHealth.Value = maxPlayerHealth;
    }

    IEnumerator Revive()
    {
        yield return new WaitForSeconds(2f);

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        UpdatePlayerActiveServerRpc(false);

        AnimatePlayerServerRpc("Revive");

        if (IsOwner)
        {
            StartCoroutine(CombatManager.Instance.ShowDeathScreen());
        }

        yield return new WaitForSeconds(5f);

        transform.position = new Vector3(0, 0, 0);

        MaxPlayerHealthServerRpc();
        healthBarFill.localScale = new Vector3(maxHealthBarWidth, healthBarFill.localScale.y, healthBarFill.localScale.z);

        UpdatePlayerActiveServerRpc(true);

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}

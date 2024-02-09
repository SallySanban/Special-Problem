using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;
using Unity.Collections;
using System;

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
            
            StartCoroutine(Wait());
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

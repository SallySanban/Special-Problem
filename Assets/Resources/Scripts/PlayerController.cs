using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;
using UnityEngine.UI;
using System;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject healthBar;
    [SerializeField] public GameObject powerBar;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public TextMeshProUGUI nameText;

    Transform healthBarFill;
    Transform powerBarFill;

    private int maxPlayerHealth = 100;
    private NetworkVariable<int> playerHealth = new(100);
    private float maxHealthBarWidth;

    private int[] playerPowerList = { 5, 10, 15, 20, 25 };
    public int playerPowerIndex = 0;
    private int maxPlayerPower;
    public static int playerPower;
    private float maxPowerBarWidth;

    private NetworkVariable<float> playerHealthBarFillValue = new(0);
    private NetworkVariable<float> playerPowerBarFillValue = new(0);

    public NetworkVariable<bool> playerActive = new(true);

    private NetworkVariable<bool> isHurt = new(false);
    private NetworkVariable<bool> isDead = new(false);

    public NetworkVariable<int> pickedChoice = new(-1); //1 for correct, 0 for incorrect, 2 for not yet picked

    private void Start()
    {
        healthBarFill = healthBar.transform.GetChild(1).transform;
        maxHealthBarWidth = healthBarFill.localScale.x;

        UpdatePickedChoiceServerRpc(-1);

        playerPower = playerPowerList[playerPowerIndex];
        maxPlayerPower = playerPowerList[playerPowerList.Length - 1];

        GetPower(playerPowerIndex);
    }

    private void Update()
    {
        ControlPlayer();
    }

    public override void OnNetworkSpawn()
    {
        powerBarFill = powerBar.transform.GetChild(1).transform;
        maxPowerBarWidth = powerBarFill.localScale.x;

        if (!IsServer) playerHealthBarFillValue.OnValueChanged += DecreaseHealthBar;
        if (!IsServer) playerPowerBarFillValue.OnValueChanged += IncreasePowerBar;
    }

    private void ControlPlayer()
    {
        if (!IsOwner) return;

        if (playerActive.Value == false) return;

        if (GameObject.FindGameObjectWithTag("Boss") == null) return;

        if(isHurt.Value == true || isDead.Value == true) return;

        if (CombatPromptsManager.instance.combatPromptOnScreen)
        {
            AnimatePlayerServerRpc("Idle");
            return;
        }

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
            UpdateIsHurtServerRpc(true);

            gameObject.GetComponent<NetworkAnimator>().SetTrigger("Hurt");
        }
        else if (animation == "Die")
        {
            UpdateIsDeadServerRpc(true);

            gameObject.GetComponent<NetworkAnimator>().SetTrigger("Die");

            RevivePlayerClientRpc();
        }
        else if (animation == "Revive")
        {
            UpdateIsDeadServerRpc(false);

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

            //PlayPunchClientRpc();

            AnimatePlayerServerRpc("Hurt");

            if (playerHealth.Value <= 0)
            {
                AnimatePlayerServerRpc("Die");
            }
        }
    }

    public void GetPower(int index)
    {
        playerPower = playerPowerList[index];

        UpdatePlayerPowerBarFillValueServerRpc();
        powerBarFill.localScale = new Vector3(playerPowerBarFillValue.Value, powerBarFill.localScale.y, powerBarFill.localScale.z);
    }

    private void DecreaseHealthBar(float previousValue, float newValue)
    {
        healthBarFill.localScale = new Vector3(playerHealthBarFillValue.Value, healthBarFill.localScale.y, healthBarFill.localScale.z);
    }

    private void IncreasePowerBar(float previousValue, float newValue)
    {
        powerBarFill.localScale = new Vector3(playerPowerBarFillValue.Value, powerBarFill.localScale.y, powerBarFill.localScale.z);
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

    [ClientRpc]
    private void PlayPunchClientRpc()
    {
        AudioManager.instance.playSoundEffect("Punch");
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerActiveServerRpc(bool activeStatus)
    {
        playerActive.Value = activeStatus;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateIsHurtServerRpc(bool hurtStatus)
    {
        isHurt.Value = hurtStatus;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateIsDeadServerRpc(bool deadStatus)
    {
        isDead.Value = deadStatus;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePickedChoiceServerRpc(int choice)
    {
        pickedChoice.Value = choice;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerPowerBarFillValueServerRpc()
    {
        playerPowerBarFillValue.Value = (maxPowerBarWidth / maxPlayerPower) * playerPower;
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

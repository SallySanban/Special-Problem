using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Playables;
using Unity.Netcode.Components;
using System;

public class BossController : NetworkBehaviour
{
    private GameObject bossHealthBar;
    private Transform bossHealthBarFill;

    //private int maxBossHealth = 100;
    private int maxBossHealth = (Player.choicesIncorrect <= 0) ? 100 : 100 * Player.choicesIncorrect;
    private float maxBossHealthBarWidth;

    private int bossHealth;
    private bool isFlipped = false;

    [SerializeField] public Transform attackPoint;
    public float attackRange = 1.3f;
    private int damage = 20;

    private NetworkVariable<float> bossHealthBarFillValue = new(0);

    private void Start()
    {
        bossHealthBar = GameObject.FindGameObjectWithTag("Boss Health Bar");
        bossHealthBarFill = bossHealthBar.transform.GetChild(1).transform;

        maxBossHealthBarWidth = bossHealthBarFill.localScale.x;

        bossHealth = maxBossHealth;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) bossHealthBarFillValue.OnValueChanged += DecreaseHealthBar;
    }

    public void LookAtPlayer(Transform player)
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if(transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if(transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void AttackPlayer()
    {
        if (!IsServer) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, LayerMask.GetMask("Player"));

        if (hitPlayer == null) return;

        if (hitPlayer.TryGetComponent(out PlayerController playerController))
        {
            playerController.GetDamageServerRpc(damage);
        }
    }

    public void GetDamage(int damage)
    {
        if (!IsServer) return;

        if(bossHealth > 0)
        {
            bossHealth = bossHealth - damage;

            bossHealthBarFillValue.Value = (maxBossHealthBarWidth / maxBossHealth) * bossHealth;
            bossHealthBarFill.localScale = new Vector3(bossHealthBarFillValue.Value, bossHealthBarFill.localScale.y, bossHealthBarFill.localScale.z);

            gameObject.GetComponent<NetworkAnimator>().SetTrigger("Hurt");

            if (bossHealth <= 0)
            {
                gameObject.GetComponent<NetworkAnimator>().SetTrigger("Die");

                StartCoroutine(Wait());
            }
        }
    }

    private void DecreaseHealthBar(float previousValue, float newValue)
    {
        bossHealthBarFill.localScale = new Vector3(bossHealthBarFillValue.Value, bossHealthBarFill.localScale.y, bossHealthBarFill.localScale.z);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}

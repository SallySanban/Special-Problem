using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BossController : NetworkBehaviour
{
    public static BossController instance;

    private GameObject bossHealthBar;
    private Transform bossHealthBarFill;

    public float bossSize;

    private int maxBossHealth;
    private float maxBossHealthBarWidth;
    
    private int bossHealth;
    private bool isFlipped = false;

    private int healthCheckpointIncrement;
    private int bossHealthCheckpoint;

    [SerializeField] public Transform attackPoint;
    public float attackRange = 0.94f;
    private int damage = 20;

    private NetworkVariable<float> bossHealthBarFillValue = new(0);

    private void Awake()
    {
        instance = this;

        bossSize = (Player.choicesIncorrect <= 0) ? 0.4f : 0.4f + (Player.choicesIncorrect * 0.02f);
        maxBossHealth = (Player.choicesIncorrect <= 0) ? 400 : 500 * Player.choicesIncorrect;
    }

    private void Start()
    {
        bossHealthBar = GameObject.FindGameObjectWithTag("Boss Health Bar");
        bossHealthBarFill = bossHealthBar.transform.GetChild(1).transform;

        maxBossHealthBarWidth = bossHealthBarFill.localScale.x;

        bossHealth = maxBossHealth;

        healthCheckpointIncrement = maxBossHealth / 5;

        bossHealthCheckpoint = maxBossHealth;
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

        if (bossHealth > 0)
        {
            if(bossHealth - damage <= 0)
            {
                bossHealth = 0;
            }
            else
            {
                bossHealth = bossHealth - damage;
            }
            
            Debug.Log("BOSS HEALTH: " + bossHealth);

            bossHealthBarFillValue.Value = (maxBossHealthBarWidth / maxBossHealth) * bossHealth;
            bossHealthBarFill.localScale = new Vector3(bossHealthBarFillValue.Value, bossHealthBarFill.localScale.y, bossHealthBarFill.localScale.z); //decreases health of server boss

            //PlaySoundEffectClientRpc("Punch");

            gameObject.GetComponent<NetworkAnimator>().SetTrigger("Hurt");

            if(bossHealth <= (bossHealthCheckpoint - healthCheckpointIncrement) && bossHealth != 0 && bossHealth != maxBossHealth)
            {
                bossHealthCheckpoint = bossHealthCheckpoint - healthCheckpointIncrement;

                ShowChoice();
            }

            if (bossHealth <= 0)
            {
                gameObject.GetComponent<NetworkAnimator>().SetTrigger("Die");

                StartCoroutine(Wait());
            }
        }
    }

    public void MaxBossHealthBar()
    {
        bossHealth = maxBossHealth;

        bossHealthBarFillValue.Value = (maxBossHealthBarWidth / maxBossHealth) * bossHealth;
        bossHealthBarFill.localScale = new Vector3(maxBossHealthBarWidth, bossHealthBarFill.localScale.y, bossHealthBarFill.localScale.z);

        bossHealthCheckpoint = bossHealthCheckpoint + healthCheckpointIncrement;
    }

    //decreases health of client boss
    private void DecreaseHealthBar(float previousValue, float newValue)
    {
        bossHealthBarFill.localScale = new Vector3(bossHealthBarFillValue.Value, bossHealthBarFill.localScale.y, bossHealthBarFill.localScale.z);
    }

    [ClientRpc]
    public void PlaySoundEffectClientRpc(string soundEffect)
    {
        AudioManager.instance.playSoundEffect(soundEffect);
    }

    private void ShowChoice()
    {
        int combatPromptId = bossHealthCheckpoint / healthCheckpointIncrement;

        CombatPromptsManager.instance.ShowPromptClientRpc(combatPromptId);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);

        CombatManager.Instance.EndScene();

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class CombatManager : NetworkBehaviour
{
    public static CombatManager Instance;

    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] public GameObject deathCount;

    private Vector3 bossPosition = new Vector3(6.37f, 1.43f, 0);

    private GameObject[] players;
    public List<GameObject> activePlayers = new List<GameObject>();
    public bool addingPlayers = false;

    //private bool done = false;

    GameObject boss;

    private void Awake()
    {
        Instance = this;

        NetworkManager.Singleton.OnServerStarted += SpawnBoss;
    }

    private void Update()
    {
        if (!IsServer) return;

        players = GameObject.FindGameObjectsWithTag("Player");

        addingPlayers = true;

        activePlayers.Clear();

        foreach (GameObject p in players)
        {
            if (p.GetComponent<PlayerController>().playerActive.Value != false)
            {
                activePlayers.Add(p);
            }
        }

        addingPlayers = false;

        //if (PlayerController.done && !done)
        //{
        //    Debug.Log("NAME");
        //    foreach (GameObject p in players)
        //    {
        //        Debug.Log(p.GetComponent<PlayerController>().playerName.Value);
        //    }

        //    done = true;
        //}

        if (boss != null)
        {
            if (CombatPromptsManager.instance.combatPromptOnScreen)
            {
                boss.GetComponent<Animator>().SetInteger("noPlayers", 0);
                boss.GetComponent<Animator>().ResetTrigger("Hurt");
                boss.GetComponent<Animator>().ResetTrigger("Attack");
            }
            else
            {
                boss.GetComponent<Animator>().SetInteger("noPlayers", activePlayers.Count);
            }
        }
    }

    private void SpawnBoss()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnBoss;

        boss = Instantiate(bossPrefab, bossPosition, Quaternion.Euler(0f, 180f, 0f));
        boss.transform.localScale = new Vector3(BossController.instance.bossSize, BossController.instance.bossSize, BossController.instance.bossSize);
        boss.GetComponent<NetworkObject>().Spawn();
    }

    public void EndScene()
    {
        if (!IsServer) return;

        foreach (GameObject p in players)
        {
            Destroy(p);
        }

        NetworkManager.Singleton.SceneManager.LoadScene("VisualNovel", LoadSceneMode.Single);
    }

    public IEnumerator ShowDeathScreen()
    {
        deathScreen.SetActive(true);

        int seconds = 5;

        while (seconds > 0)
        {
            deathCount.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = seconds.ToString();

            seconds--;

            yield return new WaitForSeconds(1f);
        }

        deathScreen.SetActive(false);
    }
}

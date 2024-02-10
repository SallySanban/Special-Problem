using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] public GameObject deathCount;

    private Vector3 bossPosition = new Vector3(6.37f, 1.43f, 0);

    private GameObject[] players;
    public List<GameObject> activePlayers = new List<GameObject>();
    public bool addingPlayers = false;

    GameObject boss;

    private void Awake()
    {
        Instance = this;

        NetworkManager.Singleton.OnServerStarted += SpawnBoss;
    }

    private void Update()
    {
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

        if (boss != null)
        {
            boss.GetComponent<Animator>().SetInteger("noPlayers", activePlayers.Count);
        }
    }

    private void SpawnBoss()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnBoss;

        boss = Instantiate(bossPrefab, bossPosition, Quaternion.Euler(0f, 180f, 0f));
        boss.GetComponent<NetworkObject>().Spawn();
    }

    public IEnumerator ShowDeathScreen()
    {
        deathCount.SetActive(true);

        int seconds = 5;

        while (seconds > 0)
        {
            deathCount.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = seconds.ToString();

            seconds--;

            yield return new WaitForSeconds(1f);
        }

        deathCount.SetActive(false);
    }
}

using UnityEngine;
using Unity.Netcode;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;

    Vector3 bossPosition = new Vector3(6.37f, 1.43f, 0);

    public static GameObject[] players;

    GameObject boss;

    private void Awake()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnBoss;
    }

    private void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        if (boss != null)
        {
            boss.GetComponent<Animator>().SetInteger("noPlayers", players.Length);
        }
    }

    private void SpawnBoss()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnBoss;

        boss = Instantiate(bossPrefab, bossPosition, Quaternion.Euler(0f, 180f, 0f));
        boss.GetComponent<NetworkObject>().Spawn();
    }
}

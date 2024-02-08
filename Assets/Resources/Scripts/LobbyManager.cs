using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject playerNameTemplate;
    [SerializeField] Transform playerNameParent;

    private List<GameObject> playerList = new List<GameObject>();

    private UnityTransport transport;

    private Lobby connectedLobby;

    private void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();
    }

    

    private void Start()
    {
        CreateOrJoinLobby();
    }

    private async void CreateOrJoinLobby()
    {
        await Authenticate();

        connectedLobby = await JoinLobby() ?? await CreateLobby();

        if (connectedLobby != null) ShowPlayers(connectedLobby);
    }

    private async Task Authenticate()
    {
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(Player.playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in Player Id: " + AuthenticationService.Instance.PlayerId + " Player Name: " + Player.playerName);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            string lobbyName = "Lobby";
            int maxPlayers = 4;
            int randomNumber = Random.Range(0, 3);

            Allocation lobbyAllocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(lobbyAllocation.AllocationId);

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        "BossLevel", new DataObject(DataObject.VisibilityOptions.Public, Player.choicesIncorrect.ToString(), DataObject.IndexOptions.N1)
                    },
                    {
                        "JoinCodeKey", new DataObject(DataObject.VisibilityOptions.Public, joinCode)
                    }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            StartCoroutine(HandleLobbyHeartbeat(lobby.Id, 15));

            transport.SetHostRelayData(lobbyAllocation.RelayServer.IpV4, (ushort) lobbyAllocation.RelayServer.Port, lobbyAllocation.AllocationIdBytes, lobbyAllocation.Key, lobbyAllocation.ConnectionData);

            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;

            try
            {
                ILobbyEvents lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callbacks);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            Debug.Log("Created Lobby Name: " + lobby.Name + " Max Players: " + lobby.MaxPlayers);

            //NetworkManager.Singleton.StartHost();

            return lobby;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);

            return null;
        }
    }

    private async Task<Lobby> JoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = GetPlayer(),
                Filter = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.N1, Player.choicesIncorrect.ToString(), QueryFilter.OpOptions.EQ)
                }
            };

            Lobby joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);

            connectedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinedLobby.Data["JoinCodeKey"].Value);

            SetTransformAsClient(joinAllocation);

            //NetworkManager.Singleton.StartClient();

            return joinedLobby;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);

            return null;
        }
    }

    private void SetTransformAsClient(JoinAllocation allocation)
    {
        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort) allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
    }

    [ContextMenu("List Players")]
    private void ListPlayers()
    {
        Debug.Log("Players in Lobby");
        foreach (Unity.Services.Lobbies.Models.Player player in connectedLobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }

    private void ShowPlayers(Lobby lobby)
    {
        foreach(GameObject player in playerList)
        {
            Destroy(player);
        }

        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            GameObject playerNamePlate = Instantiate(playerNameTemplate, playerNameParent);

            playerNamePlate.GetComponentInChildren<TMP_Text>().text = player.Data["PlayerName"].Value;
            playerNamePlate.SetActive(true);

            playerList.Add(playerNamePlate);
        }
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Player.playerName)
                    }
                }
        };
    }

    private IEnumerator HandleLobbyHeartbeat(string lobbyId, float waitTime)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTime);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void OnLobbyChanged(ILobbyChanges changes)
    {
        if (!changes.LobbyDeleted)
        {
            changes.ApplyToLobby(connectedLobby);
            ShowPlayers(connectedLobby);
        }
    }
}

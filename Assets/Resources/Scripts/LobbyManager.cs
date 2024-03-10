using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject playerNameTemplate;
    [SerializeField] Transform playerNameParent;

    [SerializeField] Button startGameButton;

    private List<GameObject> playerList = new List<GameObject>();

    public static int maxPlayers = 4;

    private Lobby connectedLobby;

    private void Start()
    {
        CreateOrJoinLobby();

        startGameButton.gameObject.SetActive(false);
    }

    private async void CreateOrJoinLobby()
    {
        await Authenticate();

        connectedLobby = await JoinLobby() ?? await CreateLobby();

        await SubscribeToLobbyEvents(connectedLobby);

        if (connectedLobby != null)
        {
            ShowPlayers(connectedLobby);

            if(IsLobbyHost()) startGameButton.gameObject.SetActive(true);

            startGameButton.onClick.AddListener(StartGame);
        }
    }

    private async Task Authenticate()
    {
        InitializationOptions initializationOptions = new InitializationOptions();

        string filteredName = Regex.Replace(Player.playerName, "[^a-zA-Z0-9]", "");

        if (filteredName.Length > 30)
        {
            filteredName = filteredName.Substring(0, 30);
        }

        initializationOptions.SetProfile(filteredName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in Player Id: " + AuthenticationService.Instance.PlayerId + " Player Name: " + Player.playerName);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void StartGame()
    {
        try
        {
            string joinCode = await RelayManager.Instance.StartRelay();

            if(joinCode != null)
            {
                connectedLobby = await Lobbies.Instance.UpdateLobbyAsync(connectedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            "JoinCodeKey", new DataObject(DataObject.VisibilityOptions.Public, joinCode)
                        }
                    }
                });
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task SubscribeToLobbyEvents(Lobby lobby)
    {
        try
        {
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;

            ILobbyEvents lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callbacks);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            string lobbyName = "Lobby";

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        "BossLevel", new DataObject(DataObject.VisibilityOptions.Public, Player.choicesIncorrect.ToString(), DataObject.IndexOptions.N1)
                    },
                    {
                        "JoinCodeKey", new DataObject(DataObject.VisibilityOptions.Public, "0")
                    }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            StartCoroutine(HandleLobbyHeartbeat(lobby.Id, 15));

            Debug.Log("Created Lobby Name: " + lobby.Name + " Max Players: " + lobby.MaxPlayers);

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

            return joinedLobby;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);

            return null;
        }
    }

    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(connectedLobby.Id);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private bool IsLobbyHost()
    {
        return connectedLobby != null && connectedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    

    private void ShowPlayers(Lobby lobby)
    {
        foreach(GameObject player in playerList)
        {
            Destroy(player);
        }

        playerList.Clear();

        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
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

            if (connectedLobby.Data["JoinCodeKey"].Value != "0")
            {
                if (!IsLobbyHost())
                {
                    RelayManager.Instance.JoinRelay(connectedLobby.Data["JoinCodeKey"].Value);
                }
            }

            if(connectedLobby.Players.Count == 0)
            {
                DeleteLobby();
            }
        }
    }
}

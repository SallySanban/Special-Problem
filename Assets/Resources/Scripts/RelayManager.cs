using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

public class RelayManager : MonoBehaviour
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject grass;

    public static RelayManager Instance;

    private UnityTransport transport;

    private void Awake()
    {
        Instance = this;

        transport = FindObjectOfType<UnityTransport>();
    }

    public async Task<string> StartRelay()
    {
        try
        {
            Allocation lobbyAllocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(lobbyAllocation.AllocationId);

            transport.SetHostRelayData(lobbyAllocation.RelayServer.IpV4, (ushort)lobbyAllocation.RelayServer.Port, lobbyAllocation.AllocationIdBytes, lobbyAllocation.Key, lobbyAllocation.ConnectionData);

            NetworkManager.Singleton.StartHost();

            lobbyUI.SetActive(false);
            grass.SetActive(true);

            return joinCode;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);

            return null;
        }
        
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            SetTransformAsClient(joinAllocation);

            NetworkManager.Singleton.StartClient();

            lobbyUI.SetActive(false);
            grass.SetActive(true);
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void SetTransformAsClient(JoinAllocation allocation)
    {
        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
    }
}

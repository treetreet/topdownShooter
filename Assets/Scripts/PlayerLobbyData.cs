using Unity.Netcode;
using UnityEngine;
using System;
using Unity.Collections;

public class PlayerLobbyData : NetworkBehaviour
{
    public static event Action<PlayerLobbyData> OnPlayerSpawned;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> TeamId = new NetworkVariable<int>(0); // 0: 선택 안함, 1: Red, 2: Blue

    [ServerRpc]
    public void SetTeamServerRpc(int team)
    {
        TeamId.Value = team;
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SubmitNameServerRpc("Player " + OwnerClientId);
        }

        OnPlayerSpawned?.Invoke(this);
        

        PlayerName.OnValueChanged += (prev, curr) => Debug.Log($"Name changed: {curr}");
    }

    public override void OnNetworkDespawn()
    {
        LobbyManager.Instance.UnregisterPlayer(this);
    }

    [ServerRpc]
    private void SubmitNameServerRpc(string name)
    {
        PlayerName.Value = name;
    }
}
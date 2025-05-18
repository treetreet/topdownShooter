using Unity.Netcode;
using System;
using Unity.Collections;

public class PlayerLobbyData : NetworkBehaviour
{
    public static event Action<PlayerLobbyData> OnPlayerSpawned;
    public static event Action<PlayerLobbyData> OnPlayerDespawn;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> teamId = new NetworkVariable<int>(0); // 0: 선택 안함, 1: Red, 2: Blue

    [ServerRpc]
    public void SetTeamServerRpc(int team)
    {
        teamId.Value = team;
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SubmitNameServerRpc("Player " + OwnerClientId);
        }

        OnPlayerSpawned?.Invoke(this);
        

        //playerName.OnValueChanged += (prev, curr) => Debug.Log($"Name changed: {curr}");
    }

    public override void OnNetworkDespawn()
    {
        OnPlayerDespawn?.Invoke(this);
    }
    [ServerRpc]
    private void SubmitNameServerRpc(string ownPlayerName)
    {
        playerName.Value = ownPlayerName;
    }
}
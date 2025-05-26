using Unity.Netcode;
using System;
using Unity.Collections;
using UnityEngine;

public class PlayerLobbyData : NetworkBehaviour
{
    enum Team
    {
        None = 0,
        Red = 1,
        Blue = 2
    }
    public static event Action<PlayerLobbyData> OnPlayerSpawned;
    public static event Action<PlayerLobbyData> OnPlayerDespawn;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> teamId = new NetworkVariable<int>(0); // 0: 선택 안함, 1: Red, 2: Blue
    
    private PlayerMovement _playerMovement; //event 호출로 나중에 리펙토링
    private SpriteRenderer _playerSpriteRenderer;

    [ServerRpc]
    public void SetTeamServerRpc(int team)
    {
        teamId.Value = team;
        if (team == (int)Team.Red)
        {
            _playerSpriteRenderer.color = Color.red;
            gameObject.layer = LayerMask.NameToLayer("Red");
        }
        else if (team == (int)Team.Blue)
        {
            _playerSpriteRenderer.color = Color.blue;
            gameObject.layer = LayerMask.NameToLayer("Blue");
        }
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SubmitNameServerRpc("Player " + OwnerClientId);
            _playerMovement = gameObject.GetComponent<PlayerMovement>();
            //_playerMovement.enabled = false;
            _playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
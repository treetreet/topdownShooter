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
    }
    public override void OnNetworkSpawn()
    {
        _playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        teamId.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateSpriteColor(newValue);
        };
        
        UpdateSpriteColor(teamId.Value);
        
        if (IsOwner)
        {
            SubmitNameServerRpc("Player " + OwnerClientId);
            _playerMovement = gameObject.GetComponent<PlayerMovement>();
            //_playerMovement.enabled = false;
        }

        OnPlayerSpawned?.Invoke(this);
        

        //playerName.OnValueChanged += (prev, curr) => Debug.Log($"Name changed: {curr}");
    }
    private void UpdateSpriteColor(int team)
    {
        if (_playerSpriteRenderer == null) return;

        switch (team)
        {
            case 1: // Red
                _playerSpriteRenderer.color = Color.red;
                gameObject.layer = LayerMask.NameToLayer("Red");
                break;
            case 2: // Blue
                _playerSpriteRenderer.color = Color.blue;
                gameObject.layer = LayerMask.NameToLayer("Blue");
                break;
            default: // None
                _playerSpriteRenderer.color = Color.white;
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
        }
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
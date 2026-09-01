using Unity.Netcode;
using System;
using Unity.Collections;
using UnityEngine;

public class PlayerNetworkData : NetworkBehaviour
{
    public static event Action<PlayerNetworkData> OnPlayerSpawned;
    public static event Action<PlayerNetworkData> OnPlayerDespawn;
    
    /// <summary>
    /// 1 red
    /// 2 blue
    /// </summary>
    public NetworkVariable<int> teamId = new NetworkVariable<int>(0); // 0: 선택 안함, 1: Red, 2: Blue
    
    private SpriteRenderer _playerSpriteRenderer;


    public override void OnNetworkSpawn()
    {
        _playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        teamId.OnValueChanged += OnTeamIdChanged;
        OnPlayerSpawned?.Invoke(this);
    }
    public override void OnNetworkDespawn()
    {
        teamId.OnValueChanged += OnTeamIdChanged;
        OnPlayerDespawn?.Invoke(this);
    }
    private void OnTeamIdChanged(int oldValue, int newValue)
    {
        UpdateSpriteColor(newValue);
        SetTeamServerRpc(newValue);
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
    
    [ServerRpc]
    private void SetTeamServerRpc(int team)
    {
        teamId.Value = team;
    }
}
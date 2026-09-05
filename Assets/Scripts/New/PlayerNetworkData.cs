using Unity.Netcode;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNetworkData : NetworkBehaviour
{
    /// <summary>
    /// 1 red
    /// 2 blue
    /// </summary>
    public NetworkVariable<int> teamId = new NetworkVariable<int>(0);

    private SpriteRenderer _playerSpriteRenderer;

    public override void OnNetworkSpawn()
    {
        _playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
        teamId.OnValueChanged += OnTeamIdChanged;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
        teamId.OnValueChanged -= OnTeamIdChanged;
    }

    private void OnPlayerConnected(ulong clientId)
    {
        UpdateSpriteColor(teamId.Value);
    }

    private void OnTeamIdChanged(int oldValue, int newValue)
    {
        UpdateSpriteColor(newValue);
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
    public void SetTeamServerRpc(int team)
    {
        teamId.Value = team;
    }
}
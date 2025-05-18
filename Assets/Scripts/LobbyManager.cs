using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }
    public List<PlayerLobbyData> playerDataList = new();

    public delegate void PlayerAdded(PlayerLobbyData playerData);
    public delegate void PlayerRemoved();
    public event PlayerAdded OnPlayerAdded;             // 플레이어 로비 입장 후 init
    public event PlayerRemoved OnPlayerRemoved;
    public event Action<bool> OnStartConditionChanged;  // 버튼 활성화 상태 변경 알림

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        PlayerLobbyData.OnPlayerSpawned += RegisterPlayer;
    }


    public void RegisterPlayer(PlayerLobbyData data)
    {
        if(data == null) Debug.LogWarning("register player data is null");
        playerDataList.Add(data);
        data.TeamId.OnValueChanged += (_, _) => CheckStartCondition();
        CheckStartCondition();
        OnPlayerAdded?.Invoke(data);
    }

    public void UnregisterPlayer(PlayerLobbyData data)
    {
        playerDataList.Remove(data);
        CheckStartCondition();
        OnPlayerRemoved?.Invoke();
    }


    public void CheckStartCondition()
    {
        int redCount = playerDataList.Count(p => p.TeamId.Value == 1);
        int blueCount = playerDataList.Count(p => p.TeamId.Value == 2);
        bool allSelected = playerDataList.All(p => p.TeamId.Value != 0);
        bool canStart = allSelected && redCount >= 1 && blueCount >= 1;
        
        Debug.Log("[Check Start Condition]" + redCount + " red, " + blueCount + " blue, " + allSelected + " all selected, " + playerDataList.Count + " player data Count");

        OnStartConditionChanged?.Invoke(canStart);
    }
    
    private void OnDestroy()
    {
        PlayerLobbyData.OnPlayerSpawned -= RegisterPlayer;
    }
}
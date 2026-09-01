using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }
    public List<PlayerNetworkData> playerDataList = new();

    public delegate void PlayerAdded(PlayerNetworkData playerData);
    public delegate void PlayerRemoved();

    public event PlayerAdded OnPlayerAdded;
    public event PlayerRemoved OnPlayerRemoved;
    public event Action<bool> OnStartConditionChanged;  // 버튼 활성화 상태 변경 알림
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        PlayerNetworkData.OnPlayerSpawned += RegisterPlayer;
        PlayerNetworkData.OnPlayerDespawn += UnregisterPlayer;
    }

    private void RegisterPlayer(PlayerNetworkData data)
    {
        if(data == null) Debug.LogWarning("register player data is null");
        playerDataList.Add(data);
        data.teamId.OnValueChanged += (_, _) => CheckStartCondition();
        CheckStartCondition();
        OnPlayerAdded?.Invoke(data);
    }

    private void UnregisterPlayer(PlayerNetworkData data)
    {
        playerDataList.Remove(data);
        CheckStartCondition();
        OnPlayerRemoved?.Invoke();
    }


    public void CheckStartCondition()
    {
        int redCount = playerDataList.Count(p => p.teamId.Value == 1);
        int blueCount = playerDataList.Count(p => p.teamId.Value == 2);
        bool allSelected = playerDataList.All(p => p.teamId.Value != 0);
        bool canStart = allSelected && redCount >= 1 && blueCount >= 1;
        
        Debug.Log("[Check Start Condition]" + redCount + " red, " + blueCount + " blue, " + allSelected + " all selected, " + playerDataList.Count + " player data Count");

        OnStartConditionChanged?.Invoke(canStart);
    }

    public void OnSceneStart()
    {
        int k = 0;
        foreach(PlayerNetworkData i in playerDataList)
        {
            PlayerMovement p = i.gameObject.GetComponent<PlayerMovement>();
            switch (k)
            {
                case 0:
                    p.RespawnPoint = new Vector2(0, 4);
                    p.gameObject.transform.position = new Vector2(0, 4);
                    break;
                case 1:
                    p.RespawnPoint = new Vector2(0, -4);
                    p.gameObject.transform.position = new Vector2(0, -4);
                    break;
                case 2:
                    p.RespawnPoint = new Vector2(-2.828f, 2.828f);
                    p.gameObject.transform.position = new Vector2(-2.828f, 2.828f);
                    break;
                case 3:
                    p.RespawnPoint = new Vector2(2.828f, -2.828f);
                    p.gameObject.transform.position = new Vector2(2.828f, -2.828f);
                    break;
                case 4:
                    p.RespawnPoint = new Vector2(-4, 0);
                    p.gameObject.transform.position = new Vector2(-4, 0);
                    break;
                case 5:
                    p.RespawnPoint = new Vector2(4, 0);
                    p.gameObject.transform.position = new Vector2(4, 0);
                    break;
            }
            k++;
        }
    }
    
    private void OnDestroy()
    {
        PlayerNetworkData.OnPlayerSpawned -= RegisterPlayer;
    }
}
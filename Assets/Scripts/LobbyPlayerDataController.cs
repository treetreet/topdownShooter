using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerDataController
{
    public PlayerLobbyData OwnPlayerData { get; private set; }
    public PlayerLobbyData[] PlayerLobbyDatas { get; private set; }

    //Insert OwnPlayerData, PlayerLobbyDatas
    public void FindClientPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerLobbyDatas = new PlayerLobbyData[players.Length];

        for (int i = 0; i < PlayerLobbyDatas.Length; i++)
        {
            PlayerLobbyDatas[i] = players[i].GetComponent<PlayerLobbyData>();
        }

        foreach (var obj in PlayerLobbyDatas)
        {
            Debug.Log(obj.ToString());
            if (obj.IsOwner)
            {
                Debug.Log(obj.ToString() + " is own");
                OwnPlayerData = obj;
                break;
            }
        }
    }
    
    //gameStartButton interactable
    public void CheckStartConditions(Button gameStartButton)
    {
        int redPlayerCount = 0;
        int bluePlayerCount = 0;
        foreach (var playerLobbyData in PlayerLobbyDatas)
        {
            PlayerData.Team teamColor = playerLobbyData.PlayerData.GetTeam();

            if (teamColor == PlayerData.Team.None)
            {
                Debug.LogWarning($"플레이어 {OwnPlayerData.GetPlayerName()} 의 팀이 None입니다.");
                return;
            }
            else if(teamColor == PlayerData.Team.Red) redPlayerCount++;
            else if(teamColor == PlayerData.Team.Blue) bluePlayerCount++;
        }

        //red와 blue 가 1명 이상의 팀원을 지니고, none 이 0일 경우 start button활성화
        if (redPlayerCount != 0 && bluePlayerCount != 0)
        {
            gameStartButton.interactable = true;
            Debug.Log("Start Button interactabled!!");
        }
    }

    [ServerRpc]
    public void ChangePlayerTeam(int teamColor)
    {
        OwnPlayerData.PlayerData.ChangeTeam(teamColor);
    }
}
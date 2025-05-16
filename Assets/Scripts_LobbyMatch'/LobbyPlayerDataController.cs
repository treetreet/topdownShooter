using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerDataController : MonoBehaviour
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
            Debug.Log(obj.ToString() + obj.IsOwner);
            if (obj.IsOwner)
            {
                Debug.Log(obj.ToString() + " is own");
                OwnPlayerData = obj;
                break;
            }
        }
        
        if(OwnPlayerData == null) Debug.Log("No own player");
    }
    
    //gameStartButton interactable
    public void CheckStartConditions(Button gameStartButton)
    {
        Debug.Log("CheckStartConditions");
        int redPlayerCount = 0;
        int bluePlayerCount = 0;
        foreach (var playerLobbyData in PlayerLobbyDatas)
        {
            PlayerLobbyData.Team teamColor = playerLobbyData.GetTeam();

            Debug.LogWarning($"플레이어 {playerLobbyData.GetPlayerName()} 의 팀이 {teamColor}입니다.");
            if (teamColor == PlayerLobbyData.Team.None)
            {
                gameStartButton.interactable = false;
                return;
            }
            else
            {
                gameStartButton.interactable = false;
                
                if (teamColor == PlayerLobbyData.Team.Red) redPlayerCount++;
                else if (teamColor == PlayerLobbyData.Team.Blue) bluePlayerCount++;
            }
        }

        //red와 blue 가 1명 이상의 팀원을 지니고, none 이 0일 경우 start button활성화
        if (redPlayerCount != 0 && bluePlayerCount != 0)
        {
            gameStartButton.interactable = true;
            Debug.Log("Start Button interactabled!!");
        }
        else if (redPlayerCount == 0) Debug.LogWarning("RedPlayerCount is 0");
        else if (bluePlayerCount == 0) Debug.LogWarning("BluePlayerCount is 0");
    }

    [ServerRpc]
    public void ChangePlayerTeam(int teamColor)
    {
        OwnPlayerData.ChangeTeamServerRpc(teamColor);
    }
}

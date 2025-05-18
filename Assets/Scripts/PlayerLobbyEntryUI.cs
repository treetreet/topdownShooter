using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLobbyEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button redTeamButton;
    [SerializeField] private Button blueTeamButton;
    [SerializeField] private TMP_Text teamText;

    private PlayerLobbyData boundData;

    private void Awake()
    {
        blueTeamButton = GameObject.Find("Blue Team Button").GetComponent<Button>();
        redTeamButton = GameObject.Find("Red Team Button").GetComponent<Button>();
    }
    
    //button click event binding
    public void Bind(PlayerLobbyData data)
    {
        boundData = data;
        
        if(boundData == null) Debug.LogWarning("PlayerLobbyEntryUI: boundData is null");

        redTeamButton.onClick.AddListener(() =>
        {
            if (boundData.IsOwner)
            {
                boundData.SetTeamServerRpc(1); // Red
                LobbyManager.Instance.CheckStartCondition();
            }
        });

        blueTeamButton.onClick.AddListener(() =>
        {
            if (boundData.IsOwner)
            {
                boundData.SetTeamServerRpc(2); // Blue
                LobbyManager.Instance.CheckStartCondition();
            }
        });

        /*data.TeamId.OnValueChanged += (_, newTeam) =>
        {
            teamText.text = GetTeamName(newTeam);
        };

        teamText.text = GetTeamName(data.TeamId.Value);*/
    }
    
    private string GetTeamName(int teamId)
    {
        return teamId switch
        {
            1 => "Red",
            2 => "Blue",
            _ => "No Team"
        };
    }
}
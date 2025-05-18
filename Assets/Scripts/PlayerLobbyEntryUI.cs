using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLobbyEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button redTeamButton;
    [SerializeField] private Button blueTeamButton;

    private PlayerLobbyData _boundData;

    private void Awake()
    {
        blueTeamButton = GameObject.Find("Blue Team Button").GetComponent<Button>();
        redTeamButton = GameObject.Find("Red Team Button").GetComponent<Button>();
    }
    
    //button click event binding
    public void Bind(PlayerLobbyData data)
    {
        _boundData = data;
        
        if(_boundData == null) Debug.LogWarning("PlayerLobbyEntryUI: boundData is null");

        redTeamButton.onClick.AddListener(() =>
        {
            if (_boundData.IsOwner)
            {
                _boundData.SetTeamServerRpc(1); // Red
                LobbyManager.Instance.CheckStartCondition();
            }
        });

        blueTeamButton.onClick.AddListener(() =>
        {
            if (_boundData.IsOwner)
            {
                _boundData.SetTeamServerRpc(2); // Blue
                LobbyManager.Instance.CheckStartCondition();
            }
        });
    }
}
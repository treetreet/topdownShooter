using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class PlayerLobbyEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button redTeamButton;
    [SerializeField] private Button blueTeamButton;

    private PlayerNetworkData _playerData;
    private NetworkObject _player;
    
    public void Start()
    {
        _player = NetworkManager.Singleton.LocalClient.PlayerObject;
        _playerData = _player.GetComponent<PlayerNetworkData>();
    }
    
    
    
    //button click event binding
    public void Bind(PlayerNetworkData data)
    {
        blueTeamButton = GameObject.Find("Blue Team Button").GetComponent<Button>();
        redTeamButton = GameObject.Find("Red Team Button").GetComponent<Button>();
        
        _playerData = data;
        
        if(_playerData == null) Debug.LogWarning("PlayerLobbyEntryUI: boundData is null");

        redTeamButton.onClick.AddListener(() =>
        {
            if (_playerData.IsOwner)
            {
                LobbyManager.Instance.CheckStartCondition();
            }
        });

        blueTeamButton.onClick.AddListener(() =>
        {
            if (_playerData.IsOwner)
            {
                LobbyManager.Instance.CheckStartCondition();
            }
        });
    }
}
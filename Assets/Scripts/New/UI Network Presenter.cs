using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UINetworkPresenter : NetworkBehaviour
{
    [Header("UI View")]
    [SerializeField] private TextMeshProUGUI _blueTeamPlayers;
    [SerializeField] private TextMeshProUGUI _redTeamPlayers;
    [SerializeField] private Image _blueTeamImage;
    [SerializeField] private Image _redTeamImgae;
    
    [Header("UI Model")]
    [SerializeField] private Sprite _blueSprite;
    [SerializeField] private Sprite _blueSelectedSprite;
    [SerializeField] private Sprite _redSprite;
    [SerializeField] private Sprite _redSelectedSprite;
    
    private NetworkVariable<int> blueTeamCount = new NetworkVariable<int>();
    private NetworkVariable<int> redTeamCount = new NetworkVariable<int>();
    
    public override void OnNetworkSpawn()
    {
        blueTeamCount.OnValueChanged += UpdateView;
        redTeamCount.OnValueChanged += UpdateView;
        
        UpdateView(0, 0);
        
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
        
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            PlayerConnected(clientId);
        }
    }
    public override void OnNetworkDespawn()
    {
        blueTeamCount.OnValueChanged -= UpdateView;
        redTeamCount.OnValueChanged -= UpdateView;
        
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
    }
    
    private void PlayerConnected(ulong clientId)
    {
        ChangeTeamText();

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client)) return;
        if (client.PlayerObject == null) return;

        client.PlayerObject.GetComponent<PlayerNetworkTeamData>().teamId.OnValueChanged += ChangeTeamCount;
    }

    private void PlayerDisconnected(ulong clientId)
    {
        ChangeTeamCount(0,0);
    }
    
    private void ChangeTeamCount(int oldValue, int newValue)
    {
        Debug.Log("teamId is Changed" +  oldValue + " -> " + newValue);
        
        int blueTeamPlayers = 0;
        int redTeamPlayers = 0;
        
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject player = client.PlayerObject;
            if (player == null) continue;
            PlayerNetworkTeamData playerTeamData = player.GetComponent<PlayerNetworkTeamData>();
            if (playerTeamData == null) continue;
            
            NetworkVariable<int> teamId = playerTeamData.teamId;
            switch (teamId.Value)
            {
                case 1: redTeamPlayers++; break;
                case 2: blueTeamPlayers++; break;
            }
        }
        
        blueTeamCount.Value = blueTeamPlayers;
        redTeamCount.Value = redTeamPlayers;
    }

    #region UpdateView
    private void UpdateView(int oldValue, int newValue)
    {
        ChangeTeamImage();
        ChangeTeamText();
    }

    private void ChangeTeamText()
    {
        _blueTeamPlayers.text = blueTeamCount.Value.ToString();
        _redTeamPlayers.text = redTeamCount.Value.ToString();
    }

    private void ChangeTeamImage()
    {
        NetworkObject player = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (player == null) return;
        PlayerNetworkTeamData playerTeamData = player.GetComponent<PlayerNetworkTeamData>();
        if (playerTeamData == null) return;

        NetworkVariable<int> teamId = playerTeamData.teamId;

        switch (teamId.Value)
        {
            case 1:
                _redTeamImgae.sprite = _redSelectedSprite;
                _blueTeamImage.sprite = _blueSprite;
                break;
            case 2:
                _redTeamImgae.sprite = _redSprite;
                _blueTeamImage.sprite = _blueSelectedSprite;
                break;
            default:
                _blueTeamImage.sprite = _blueSprite;
                _redTeamImgae.sprite = _redSprite;
                break;
        }
    }
    #endregion
}

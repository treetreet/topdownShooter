using TMPro;
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
    
    public static UINetworkPresenter Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        Instance = this;

        if (NetworkManager.Singleton.LocalClient.PlayerObject == null) return;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetworkData>().teamId.OnValueChanged += UpdateView;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        
        if (NetworkManager.Singleton.LocalClient.PlayerObject == null) return;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetworkData>().teamId.OnValueChanged -= UpdateView;
    }
    
    public void OnPlayerSpawned(PlayerNetworkData playerData)
    {
        playerData.teamId.OnValueChanged += UpdateView;
    }
    public void OnPlayerDespawned(PlayerNetworkData playerData)
    {
        playerData.teamId.OnValueChanged -= UpdateView;
        UpdateView(0,0);
    }
    
    private void UpdateView(int oldValue, int newValue)
    {
        Debug.Log($"UpdateView {oldValue} -> {newValue}");
        ChangeTeamText();
        ChangeTeamImage();
    }

    private void ChangeTeamImage()
    {
        NetworkVariable<int> teamId = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetworkData>().teamId;

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

    private void ChangeTeamText()
    {
        int blueTeamPlayers = 0;
        int redTeamPlayers = 0;
        
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkVariable<int> teamId = client.PlayerObject.GetComponent<PlayerNetworkData>().teamId;
            switch (teamId.Value)
            {
                case 1: redTeamPlayers++; break;
                case 2: blueTeamPlayers++; break;
            }
        }
        
        _blueTeamPlayers.text = blueTeamPlayers.ToString();
        _redTeamPlayers.text = redTeamPlayers.ToString();
    }
}

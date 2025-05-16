using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    [SerializeField] private Transform playerListParent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject[] hostOnlyUIList;
    [SerializeField] private GameObject[] clientOnlyUIList;
    
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button teamRedButton;
    [SerializeField] private Button teamBlueButton;

    private LobbyPlayerDataController _playerDataController;

    private void Awake()
    {
        Instance = this;
        _playerDataController = GetComponent<LobbyPlayerDataController>();
    }

    public void SetUp()
    {
        _playerDataController.FindClientPlayer();
        InitializeUI();
    }

    private void InitializeUI()
    {
        if(_playerDataController.OwnPlayerData == null) Debug.LogWarning("No client player found");
        else
        {
            teamRedButton.onClick.AddListener(() => LobbyManager.Instance._playerDataController.ChangePlayerTeamServerRpc((int)PlayerLobbyData.Team.Red));
            teamBlueButton.onClick.AddListener(() => LobbyManager.Instance._playerDataController.ChangePlayerTeamServerRpc((int)PlayerLobbyData.Team.Blue));

            teamRedButton.onClick.AddListener(() => LobbyManager.Instance._playerDataController.CheckStartConditions(gameStartButton));
            teamBlueButton.onClick.AddListener(() => LobbyManager.Instance._playerDataController.CheckStartConditions(gameStartButton));
        }
    }

    public void HostOnlySetUI()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            foreach (var hostOnlyUI in hostOnlyUIList)
            {
                hostOnlyUI.SetActive(true);
                gameStartButton.interactable = false;
            }
        }
    }
    public void ClientOnlySetUI()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            foreach (var clientOnlyUI in clientOnlyUIList)
            {
                clientOnlyUI.SetActive(true);
            }
        }
    }
    public void UpdatePlayerListUI()
    {
        //Lobby의 player List가 변경될 때마다 실행
        Debug.Log($"UpdatePlayerListUI");
        
    }
}
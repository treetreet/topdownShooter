using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayerDataController : MonoBehaviour
{
    [SerializeField] private GameObject playerEntryPrefab;  //PlayerEnterUI pref
    [SerializeField] private Transform contentParent;       //PlayerEnterUI parent
    [SerializeField] private Button startGameButton;
    private GameObject _entry;
    private void Start()
    {
        LobbyManager.Instance.OnStartConditionChanged += (canStart) =>
        {
            startGameButton.interactable = canStart;
        };
        LobbyManager.Instance.OnPlayerAdded += StartButtonSet;
        LobbyManager.Instance.OnPlayerRemoved += RemovedPlayerUI;
        
        startGameButton.interactable = false;
        
        startGameButton.onClick.AddListener(StartGameServerRpc);
        startGameButton.onClick.AddListener(LobbyManager.Instance.OnSceneStart);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        // 씬 전환, 게임 시작 로직 실행
        Debug.Log("게임 시작!");
        NetworkManager.Singleton.SceneManager.LoadScene("UI_Game", LoadSceneMode.Single);
    }

    private void RemovedPlayerUI()
    {
        Destroy(_entry);
    }

    private void StartButtonSet(PlayerNetworkData playerData)
    {
        startGameButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnPlayerAdded -= StartButtonSet;
        LobbyManager.Instance.OnPlayerRemoved -= RemovedPlayerUI;
    }
}
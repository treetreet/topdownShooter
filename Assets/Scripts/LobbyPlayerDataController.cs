using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayerDataController : MonoBehaviour
{
    [SerializeField] private GameObject playerEntryPrefab;  //PlayerEnterUI pref
    [SerializeField] private Transform contentParent;       //PlayerEnterUI parent
    [SerializeField] private Button startGameButton;

    private void Start()
    {
        LobbyManager.Instance.OnStartConditionChanged += (canStart) =>
        {
            startGameButton.interactable = canStart;
        };
        LobbyManager.Instance.OnPlayerAdded += AddPlayerUI;

        //startGameButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
        startGameButton.interactable = false;
        
        startGameButton.onClick.AddListener(() =>
        {
            StartGameServerRpc(); // 게임 시작 요청
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        // 씬 전환, 게임 시작 로직 실행
        Debug.Log("게임 시작!");
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    private void AddPlayerUI(PlayerLobbyData playerData)
    {
        GameObject entry = Instantiate(playerEntryPrefab, contentParent);
        var ui = entry.GetComponent<PlayerLobbyEntryUI>();
        ui.Bind(playerData);
    }
    
    IEnumerator StartAfterHostCheck()
    {
        yield return new WaitForSeconds(0.5f);
        startGameButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
    }
}
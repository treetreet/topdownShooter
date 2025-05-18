using System;
using System.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//
public class LobbyPlayerDataController : MonoBehaviour
{
    [SerializeField] private GameObject playerEntryPrefab;  //PlayerEnterUI pref
    [SerializeField] private Transform contentParent;       //PlayerEnterUI parent
    [SerializeField] private Button startGameButton;
    private GameObject entry;
    private void Start()
    {
        LobbyManager.Instance.OnStartConditionChanged += (canStart) =>
        {
            startGameButton.interactable = canStart;
        };
        LobbyManager.Instance.OnPlayerAdded += AddPlayerUI;
        LobbyManager.Instance.OnPlayerAdded += StartButtonSet;

        LobbyManager.Instance.OnPlayerRemoved += RemovedPlayerUI;
        
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

    //Button UI 관련 동기화.
    private void AddPlayerUI(PlayerLobbyData playerData)
    {
        entry = Instantiate(playerEntryPrefab, contentParent);
        var ui = entry.GetComponent<PlayerLobbyEntryUI>();
        ui.Bind(playerData);
    }

    private void RemovedPlayerUI()
    {
        Destroy(entry);
    }

    private void StartButtonSet(PlayerLobbyData playerData)
    {
        startGameButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnPlayerAdded -= AddPlayerUI;
        LobbyManager.Instance.OnPlayerAdded -= StartButtonSet;

        LobbyManager.Instance.OnPlayerRemoved -= RemovedPlayerUI;
    }
}
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : NetworkBehaviour
{
    private string pendingSceneName;

    public void NetworkChangeScene(string sceneName)
    {
        if (IsSpawned)
        {
            ChangeSceneServerRpc(sceneName);
        }
        else
        {
            // 아직 스폰 안 됐으면 스폰될 때까지 대기
            pendingSceneName = sceneName;
            StartCoroutine(WaitForSpawnThenChangeScene());
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private System.Collections.IEnumerator WaitForSpawnThenChangeScene()
    {
        yield return new WaitUntil(() => IsSpawned);
        ChangeSceneServerRpc(pendingSceneName);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSceneServerRpc(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
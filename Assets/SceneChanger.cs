using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : NetworkBehaviour
{
    public void NetworkChangeScene(string sceneName)
    {
        ChangeSceneServerRpc(sceneName);
    }
    
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    [ServerRpc]
    private void ChangeSceneServerRpc(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

}

using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : NetworkBehaviour
{
    public void NetworkChangeScene(string sceneName)
    {
        if (IsHost)
        {
            Debug.Log("Host" + sceneName + "Called");
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Client"  + sceneName + "Called");
            ChangeScene(sceneName);
        }
        Debug.Log(sceneName + "Loaded");
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
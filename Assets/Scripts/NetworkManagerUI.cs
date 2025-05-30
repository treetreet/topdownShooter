using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverbutton;
    [SerializeField] private Button clientbutton;
    [SerializeField] private Button Hostbutton;
    void Start()
    {
        serverbutton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        clientbutton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        Hostbutton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

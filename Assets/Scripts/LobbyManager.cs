using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    [SerializeField] private Transform playerListParent;
    [SerializeField] private GameObject playerListItemPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePlayerListUI()
    {
        //Lobby의 player List가 변경될 때마다 실행
        Debug.Log($"UpdatePlayerListUI");
    }
}
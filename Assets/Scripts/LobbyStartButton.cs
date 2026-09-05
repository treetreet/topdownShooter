using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyStartButton : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    private void Start()
    {
        startGameButton = GetComponent<Button>();
        startGameButton.interactable = NetworkManager.Singleton.IsHost;
        startGameButton.onClick.AddListener(StartGameServerRpc);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        // 씬 전환, 게임 시작 로직 실행
        if (!StartCondition()) return; 
        
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject player = client.PlayerObject;
            if (player == null) continue;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement == null) continue;

            playerMovement.isStarted = true;
        }
        
        NetworkManager.Singleton.SceneManager.LoadScene("UI_Game", LoadSceneMode.Single);
    }


    private bool StartCondition()
    {
        int playerCountGap = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject player = client.PlayerObject;
            if (player == null) continue;
            PlayerNetworkTeamData playerTeamData = player.GetComponent<PlayerNetworkTeamData>();
            if (playerTeamData == null) continue;

            //red팀이면 +1, blue팀이면 -1, 팀 선택 인원이 없다면 return false
            switch (playerTeamData.teamId.Value)
            {
                case 1 :  playerCountGap++; break;
                case 2 :  playerCountGap--; break;
                case 0 : return false;
            }
        }
        
        // 팀원이 동일하면 true
        return playerCountGap == 0;
    }
}
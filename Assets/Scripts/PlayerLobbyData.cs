using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerLobbyData : NetworkBehaviour
{
    //서버(호스트)에 저장될 이름
    private readonly NetworkVariable<FixedString32Bytes> _playerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            //로컬에서 실행
            string playerName = "플레이어" + Random.Range(10000, 99999);
            SetPlayerNameServerRpc(playerName);
            
            LobbyManager.Instance.UpdatePlayerListUI();
            
            Debug.Log(playerName + "is join this lobby");
        }
    }

    public override void OnNetworkDespawn()
    {
        LobbyManager.Instance.UpdatePlayerListUI();
    }
    
    [ServerRpc]
    private void SetPlayerNameServerRpc(string nameValue)
    {
        _playerName.Value = nameValue;
    }

    public string GetPlayerName() => _playerName.Value.ToString();
}


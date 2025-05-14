using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public class PlayerLobbyData : NetworkBehaviour
{
    private readonly NetworkVariable<FixedString32Bytes> _playerName = new NetworkVariable<FixedString32Bytes>();
    private NetworkVariable<PlayerLobbyData>[] _playerLobbyDatas;

    public readonly PlayerData PlayerData = new PlayerData();
    
    public override void OnNetworkSpawn()
    {
        LobbyManager.Instance.HostOnlySetUI();
        LobbyManager.Instance.ClientOnlySetUI();
        LobbyManager.Instance.SetUp();

        if (IsOwner)
        {
            //로컬에서 실행
            string playerName = "플레이어" + Random.Range(10000, 99999);
            Debug.Log(playerName + "is join this lobby");
            SetPlayerNameServerRpc(playerName);
            
            LobbyManager.Instance.UpdatePlayerListUI();
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


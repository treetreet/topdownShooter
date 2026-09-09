using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace New
{
    public class PlayerSpawnPosition : NetworkBehaviour
    {
        [SerializeField] private List<Transform> playerTransform;
        private const ulong EmptySlot = ulong.MaxValue;
        private List<ulong> networkObjects = new List<ulong>(){EmptySlot,EmptySlot,EmptySlot,EmptySlot};

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return; // 서버(호스트)에서만 배정 처리 -> 콜백 이후 player obj 생성 순서 보장

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDespawn;

            // 이미 접속해있는 클라이언트들도 처리 (호스트 자신 포함)
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                HandleClientConnected(clientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDespawn;
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (networkObjects.Contains(clientId)) return;

            int slot = -1;
            for (int i = 0; i < playerTransform.Count; i++)
            {
                if (networkObjects[i] == EmptySlot)
                {
                    networkObjects[i] = clientId;
                    slot = i;
                    break;
                }
            }

            if (slot == -1)
            {
                Debug.LogError("Lobby is Full");
                return;
            }

            var client = NetworkManager.Singleton.ConnectedClients[clientId];
            if (client.PlayerObject != null)
            {
                client.PlayerObject.transform.position = playerTransform[slot].position;
            }
        }

        private void PlayerDespawn(ulong clientId)
        {
            int i = networkObjects.FindIndex(c => c == clientId);
            if (i >= 0)
            {
                networkObjects[i] = EmptySlot;
            }
        }
    }
}
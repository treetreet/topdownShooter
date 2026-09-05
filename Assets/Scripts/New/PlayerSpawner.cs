using Unity.Netcode;
using UnityEngine;

namespace New
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] redSpawnPoints;
        private bool[] redSpawn = new bool[3];
        [SerializeField] private Transform[] blueSpawnPoints;
        private bool[] blueSpawn = new bool[3];
        
        private void Start()
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                NetworkObject player = client.PlayerObject;
                if (player == null) continue;
                PlayerNetworkTeamData team = player.GetComponent<PlayerNetworkTeamData>();
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (team == null || playerMovement == null) continue;

                playerMovement.RespawnPoint = GetRandomSpawnPoint(team.teamId.Value).position;
                playerMovement.transform.position = playerMovement.RespawnPoint;
            }
        }
        
        private Transform GetRandomSpawnPoint(int team)
        {
            if (team == 1)
            {
                for (var i = 0; i < redSpawn.Length; i++)
                {
                    if (redSpawn[i]) continue;

                    redSpawn[i] = true;
                    return redSpawnPoints[i];
                }
            }
            else
            {
                for (var i = 0; i < blueSpawn.Length; i++)
                {
                    if (blueSpawn[i]) continue;

                    blueSpawn[i] = true;
                    return blueSpawnPoints[i];
                }
            }
            return null;
        }
    }
}
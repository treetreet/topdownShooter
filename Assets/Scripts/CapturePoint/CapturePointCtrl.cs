    using System;
    using System.Collections.Generic;
    using TreeEditor;
    using Unity.Netcode;
    using UnityEngine;

    public class CapturePointCtrl : NetworkBehaviour
    {
        enum Team
        {
            None = 0,
            Red = 1,
            Blue = 2
        }
        
        public int scorePerSecond = 10; 
        private Dictionary<Team, HashSet<NetworkObject>> _playersInZone = new();

        private float score = 0f;
        private float debugTimer = 0f;

        private void Awake()
        {
            _playersInZone.Add(Team.Red, new HashSet<NetworkObject>());
            _playersInZone.Add(Team.Blue, new HashSet<NetworkObject>());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsServer || !other.CompareTag("Player")) return;
            
            Debug.Log(other.gameObject.tag + " - " + other.gameObject.layer);
            if (other.gameObject.layer == LayerMask.NameToLayer("Red"))
            {
                Debug.Log("Red 입장");
                _playersInZone[Team.Red].Add(other.gameObject.GetComponent<NetworkObject>());
            }

            else if (other.gameObject.layer == LayerMask.NameToLayer("Blue"))
            {
                Debug.Log("Blue 입장");
                _playersInZone[Team.Blue].Add(other.gameObject.GetComponent<NetworkObject>());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsServer || !other.CompareTag("Player")) return;
            
            if (other.gameObject.layer == LayerMask.NameToLayer("Red"))
            {
                Debug.Log("Red 퇴장");
                _playersInZone[Team.Red].Remove(other.gameObject.GetComponent<NetworkObject>());
            }

            if (other.CompareTag("BluePlayer"))
            {
                Debug.Log("Blue 퇴장");
                _playersInZone[Team.Blue].Remove(other.gameObject.GetComponent<NetworkObject>());
            }
        }

        private void Update()
        {
            if(!IsServer) return;
            
            /*if (BlueplayerInside && RedplayerInside)
            {
                   
            }
            else if (BlueplayerInside && !RedplayerInside)
            {
                score += scorePerSecond * Time.deltaTime;
            }
            else if (RedplayerInside && !BlueplayerInside)
            {
                score += -scorePerSecond * Time.deltaTime;
            }
            debugTimer += Time.deltaTime;*/

            if (debugTimer >= 1f)
            {
                Debug.Log($"점령 점수: {score:F2}");
                debugTimer = 0f;
            }
            
            if (score >= 100)
            {
                Debug.Log("BlueTeam Win!");
            }
            else if (score <= -100)
            {
                Debug.Log("RedTeam Win!");
            }
        }
    }

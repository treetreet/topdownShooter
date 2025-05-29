using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CapturePointCtrl : NetworkBehaviour
{
    enum Team
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Contesting = 3
    }

    public int scorePerSecond = 10;
    private readonly Dictionary<Team, HashSet<NetworkObject>> _playersInZone = new();

    private Team _inZoneTeam = Team.None;
    private Team _zoneOwnerTeam = Team.None;
    
    [SerializeField] private float _zoneGauge = 0f;
    
    [SerializeField] private float _redScore = 0f;
    [SerializeField] private float _blueScore = 0f;
    
    
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

        CheckInZoneTeam();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer || !other.CompareTag("Player")) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            Debug.Log("Red 퇴장");
            _playersInZone[Team.Red].Remove(other.gameObject.GetComponent<NetworkObject>());
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Blue"))
        {
            Debug.Log("Blue 퇴장");
            _playersInZone[Team.Blue].Remove(other.gameObject.GetComponent<NetworkObject>());
        }

        CheckInZoneTeam();
    }

    void CheckInZoneTeam()
    {
        bool inRed = _playersInZone[Team.Red].Count > 0;
        bool inBlue = _playersInZone[Team.Blue].Count > 0;
        
        if (inRed)
        {
            _inZoneTeam = inBlue ? Team.Contesting : Team.Red;
        }
        else
        {
            _inZoneTeam = inBlue ? Team.Blue : Team.None;
        }
    }

    void Update()
    {
        if (!IsServer) return;
        
        if (_inZoneTeam == Team.None)
        {
            ScoreUp(_inZoneTeam);
        }
        else if (_inZoneTeam == Team.Red || _inZoneTeam == Team.Blue)
        {
            if (_inZoneTeam == _zoneOwnerTeam)
            {
                ScoreUp(_inZoneTeam);
            }
            else
            {
                ZoneGaugeUp(_inZoneTeam);
            }
        }
    }

    void ScoreUp(Team team)
    {
        if (team == Team.Red)
        {
            _redScore += scorePerSecond * Time.deltaTime;
            if (_redScore >= 100)
            {
                _redScore = 100;
                Debug.Log("Red Win!");
            }
        }
        else if (team == Team.Blue)
        {
            _blueScore += scorePerSecond * Time.deltaTime;
            if (_blueScore >= 100)
            {
                _blueScore = 100;
                Debug.Log("Blue Win!");
            }
        }
    }

    void ZoneGaugeUp(Team team)
    {
        if (team == Team.Red)
        {
            _zoneGauge += scorePerSecond * Time.deltaTime;
            if (_zoneGauge >= 100)
            {
                _zoneGauge = 100;
                _zoneOwnerTeam = team;
            }
            else if (_zoneOwnerTeam == Team.Blue && _zoneGauge >= 0)
            {
                _zoneOwnerTeam = Team.None;
            }
        }
        else if (team == Team.Blue)
        {
            _zoneGauge -= scorePerSecond * Time.deltaTime;
            if (_zoneGauge <= -100)
            {
                _zoneGauge = -100;
                _zoneOwnerTeam = team;
            }
            else if (_zoneOwnerTeam == Team.Red && _zoneGauge <= 0)
            {
                _zoneOwnerTeam = Team.None;
            }
        }
    }
}
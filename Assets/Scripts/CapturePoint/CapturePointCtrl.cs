using System.Collections.Generic;
using UI;
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

    [SerializeField] private int scorePerSecond = 10;
    private readonly Dictionary<Team, HashSet<NetworkObject>> _playersInZone = new();
    [Space(10f)]
    [SerializeField] private WinUI _winUI;
    
    [Space(10f)]
    [SerializeField] private NetworkVariable<Team> _inZoneTeam = new(Team.None);
    [SerializeField] private NetworkVariable<Team> _zoneOwnerTeam = new(Team.None);
    
    
    private NetworkVariable<float> _zoneGauge = new(0f);
    private NetworkVariable<float> _redScore = new(0f);
    private NetworkVariable<float> _blueScore = new(0f);
    
    public override void OnNetworkSpawn()
    {
        _zoneGauge.OnValueChanged += OnZoneGaugeChanged;
        _redScore.OnValueChanged += OnRedScoreChanged;
        _blueScore.OnValueChanged += OnBlueScoreChanged;
        
        if (!IsServer)
            return;

        _playersInZone[Team.Red] = new HashSet<NetworkObject>();
        _playersInZone[Team.Blue] = new HashSet<NetworkObject>();
    }

    private void OnZoneGaugeChanged(float previous, float current)
    {
        UIManager.Instance.OccGauge = current;
    }

    private void OnRedScoreChanged(float previous, float current)
    {
        Debug.Log($"Red Score changed for {previous} to {current}");
        
        UIManager.Instance.RedScore = (int)current;
    }

    private void OnBlueScoreChanged(float previous, float current)
    {
        Debug.Log($"Blue Score changed for {previous} to {current}");
        
        UIManager.Instance.BlueScore = (int)current;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        NetworkObject player = other.GetComponent<NetworkObject>();
        if (player == null) return;
        
        if (IsClient)
        {
            CapturePointEnterServerRpc(player.OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CapturePointEnterServerRpc(ulong clientId)
    {
        NetworkObject player =
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (player == null)
            return;

        if (player.gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            _playersInZone[Team.Red].Add(player);
        }
        else if (player.gameObject.layer == LayerMask.NameToLayer("Blue"))
        {
            _playersInZone[Team.Blue].Add(player);
        }

        CheckInZoneTeam();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        NetworkObject player = other.GetComponent<NetworkObject>();

        if (player == null)
            return;

        if (IsClient)
        {
            CapturePointExitServerRpc(player.OwnerClientId);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void CapturePointExitServerRpc(ulong clientId)
    {
        NetworkObject player =
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (player == null)
            return;

        if (player.gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            _playersInZone[Team.Red].Remove(player);
        }
        else if (player.gameObject.layer == LayerMask.NameToLayer("Blue"))
        {
            _playersInZone[Team.Blue].Remove(player);
        }

        CheckInZoneTeam();
    }

    void CheckInZoneTeam()
    {
        bool inRed = _playersInZone[Team.Red].Count > 0;
        bool inBlue = _playersInZone[Team.Blue].Count > 0;
        
        if (inRed)
        {
            _inZoneTeam.Value = inBlue ? Team.Contesting : Team.Red;
        }
        else
        {
            _inZoneTeam.Value = inBlue ? Team.Blue : Team.None;
        }
    }

    void Update()
    {
        if (!IsServer) return;
        
        if (_inZoneTeam.Value == Team.Red || _inZoneTeam.Value == Team.Blue)
        {
            if (_inZoneTeam.Value == _zoneOwnerTeam.Value)
            {
                ScoreUp();
            }
            else
            {
                ZoneGaugeUp(_inZoneTeam.Value);
            }
        }
    }

    void ScoreUp()
    {
        Debug.Log(_inZoneTeam.Value + "Score Up");
        if (_inZoneTeam.Value == Team.Red)
        {
            _redScore.Value += scorePerSecond * Time.deltaTime;
            if (_redScore.Value >= 100)
            {
                _redScore.Value = 100;
                _winUI.RedWin();
            }
        }
        else if (_inZoneTeam.Value == Team.Blue)
        {
            _blueScore.Value += scorePerSecond * Time.deltaTime;
            if (_blueScore.Value >= 100)
            {
                _blueScore.Value = 100;
                _winUI.BlueWin();
            }
        }
    }

    void ZoneGaugeUp(Team team)
    {
        if (team == Team.Red)
        {
            _zoneGauge.Value += scorePerSecond * Time.deltaTime;
            if (_zoneGauge.Value >= 100)
            {
                _zoneGauge.Value = 100;
                _zoneOwnerTeam.Value = team;
            }
            else if (_zoneOwnerTeam.Value == Team.Blue && _zoneGauge.Value >= 0)
            {
                _zoneOwnerTeam.Value = Team.None;
            }
        }
        else if (team == Team.Blue)
        {
            _zoneGauge.Value -= scorePerSecond * Time.deltaTime;
            if (_zoneGauge.Value <= -100)
            {
                _zoneGauge.Value = -100;
                _zoneOwnerTeam.Value = team;
            }
            else if (_zoneOwnerTeam.Value == Team.Red && _zoneGauge.Value <= 0)
            {
                _zoneOwnerTeam.Value = Team.None;
            }
        }
    }
}
using Unity.Netcode;
using UnityEngine;

public class PlayerData
{
    public enum Team
    {
        None,
        Red,
        Blue
    };
    
    private readonly NetworkVariable<int> _team = new NetworkVariable<int>();
    
    public Team GetTeam() => (Team)_team.Value;
    
    [ServerRpc]
    public void ChangeTeam(int teamColor)
    {
        Debug.Log($"Change team {teamColor}");
        _team.Value = teamColor;
    }
}
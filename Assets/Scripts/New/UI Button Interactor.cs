using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace New
{
    public class UIButtonInteractor : MonoBehaviour
    {
        [SerializeField] private Button _redTeamButton;
        [SerializeField] private Button _blueTeamButton;

        private void Awake()
        {
            _redTeamButton.onClick.AddListener(OnClickRedTeamButton);
            _blueTeamButton.onClick.AddListener(OnClickBlueTeamButton);
        }

        private void OnClickRedTeamButton()
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetworkTeamData>().SetTeamServerRpc(1);
        }

        private void OnClickBlueTeamButton()
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetworkTeamData>().SetTeamServerRpc(2);
        }
    }
}
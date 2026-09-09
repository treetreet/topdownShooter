using Unity.Netcode;
using UnityEngine;

namespace UI
{
    public class WinUI : MonoBehaviour
    {
        [SerializeField] private GameObject redPanel;
        [SerializeField] private GameObject bluePanel;
        
        public void BlueWin()
        {
            GameEndSetting();
            bluePanel.SetActive(true);
        }

        public void RedWin()
        {
            GameEndSetting();
            redPanel.SetActive(true);
        }

        private void GameEndSetting()
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                client.PlayerObject.GetComponent<PlayerMovement>().isStarted = false;
            }
            Time.timeScale = 0;
        }
    }
}
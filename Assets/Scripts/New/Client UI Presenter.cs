using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace New
{
    public class ClientUIPresenter : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI ammoText;

        private PlayerMovement playerData;

        private void Start()
        {
            var player = NetworkManager.Singleton.LocalClient.PlayerObject;
            playerData = player.GetComponent<PlayerMovement>();

            playerData._hp.OnValueChanged += UpdateHpSlider;
            playerData._currentAmmo.OnValueChanged += UpdateAmmoText;
        }

        private void OnDestroy()
        {
            playerData._hp.OnValueChanged -= UpdateHpSlider;
            playerData._currentAmmo.OnValueChanged -= UpdateAmmoText;
        }

        private void UpdateHpSlider(float oldValue, float value)
        {
            hpSlider.value = value / 100f;
        }

        private void UpdateAmmoText(int oldValue, int value)
        {
            ammoText.text = value + " / 30";
        }
    }
}
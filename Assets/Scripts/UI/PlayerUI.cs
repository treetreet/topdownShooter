using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 플레이어마다 다르게 보는 UI 
// 예) 남은 총알 개수, 체력
// 팀 점수같이 모든 플레이어가 똑같이 보는 값은 일단 추가 안함.
public class PlayerUI : MonoBehaviour
{
    TextMeshProUGUI _bulletText;
    Slider _hpSlider;

    // TODO temp
    int _remainBullet = 10;
    int _maxBullet = 30;
    int _hp = 50;
    int _maxHp = 100;


    void Start()
    {
        // Controller 연결
        _bulletText = GameObject.Find("BulletText").GetComponent<TextMeshProUGUI>();
        _hpSlider = GameObject.Find("HpSlider").GetComponent <Slider>();


        RefreshBulletText();
        RefreshHp();
    }


    public void RefreshPlayerUI()
    {
        RefreshBulletText();
        RefreshHp();
    }
    public void RefreshBulletText()
    {
        // TODO 원래 Controller 에서 남은 총알 개수, 최대 총알 개수 받아와서 업데이트하는 함수
        _bulletText.text = _remainBullet + " / " + _maxBullet;
    }

    public void RefreshHp()
    {
        // TODO 원래 Controller 에서 받아와서 업데이트하는 함수
        _hpSlider.value = _hp;
    }
}

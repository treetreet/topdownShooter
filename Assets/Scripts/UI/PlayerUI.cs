using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �÷��̾�� �ٸ��� ���� UI 
// ��) ���� �Ѿ� ����, ü��
// �� �������� ��� �÷��̾ �Ȱ��� ���� ���� �ϴ� �߰� ����.
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
        // Controller ����
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
        // TODO ���� Controller ���� ���� �Ѿ� ����, �ִ� �Ѿ� ���� �޾ƿͼ� ������Ʈ�ϴ� �Լ�
        _bulletText.text = _remainBullet + " / " + _maxBullet;
    }

    public void RefreshHp()
    {
        // TODO ���� Controller ���� �޾ƿͼ� ������Ʈ�ϴ� �Լ�
        _hpSlider.value = _hp;
    }
}

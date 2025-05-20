using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CheckBox : MonoBehaviour
{
    Image _checkImage;

    bool _isActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _checkImage = transform.Find("Check").GetComponent<Image>();

        if (_isActive)
            _checkImage.enabled = true;
        else
            _checkImage.enabled = false;
    }


    public void ChangeCheckBox()
    {
        if (_isActive)
        {
            _isActive = false;
            _checkImage.enabled = false;
            // TODO 설정
        }
        else
        {
            _isActive = true;
            _checkImage.enabled = true;
            // TODO 설정
        }
    }
}

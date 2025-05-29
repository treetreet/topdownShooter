using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum TeamColor
    {
        Red,
        Blue
    }

    static UIManager s_instance;
    public static UIManager Instance { get { return s_instance; } }

    Canvas _optionPage;
    Slider _occGauge;
    Image _fillAreaImage;
    TextMeshProUGUI _redScoreText;
    TextMeshProUGUI _blueScoreText;



    private void Start()
    {
        _optionPage = GameObject.Find("OptionPage").GetComponent<Canvas>();
        if (_optionPage)
            _optionPage.enabled = false;

        _occGauge = GameObject.Find("OccGauge").GetComponent<Slider>();
        _redScoreText = GameObject.Find("RedScoreText").GetComponent<TextMeshProUGUI>();
        _blueScoreText = GameObject.Find("BlueScoreText").GetComponent<TextMeshProUGUI>();

        _fillAreaImage = _occGauge.transform.Find("Fill").GetComponent<Image>();


        RefreshOccGauge(TeamColor.Red);
    }


    public void OnPauseClicked()
    {
        _optionPage.enabled = true;
    }

    public void OnPauseBackClicked()
    {
        _optionPage.enabled = false;
    }


    public void RefreshScoreUI()
    {
        RefreshRedScoreText();
        RefreshBlueScoreText();
    }

    public void RefreshOccGauge(TeamColor teamColor)
    {
        // TODO
        switch (teamColor)
        {
            case TeamColor.Red:
                _fillAreaImage.color = Color.red;
                break;
            case TeamColor.Blue:
                _fillAreaImage.color = Color.blue;
                break;
        }

        // temp
        _occGauge.value = 30;
    }

    public void RefreshRedScoreText()
    {
        // TODO
        _redScoreText.text = "30";
    }
    public void RefreshBlueScoreText()
    {
        // TODO
        _blueScoreText.text = "30";
    }
}

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

    public float OccGauge
    {
        get { return _occGauge.value; }
        set
        {

            if (value > 0)
            {
                _occGauge.value = value;
                RefreshOccGauge(TeamColor.Red);
            }
            else
            {
                _occGauge.value = value * -1;
                RefreshOccGauge(TeamColor.Blue);
            }
        }
    }

    public int RedScore
    {
        set
        {
            _redScoreText.text = value.ToString();
        }
    }
    public int BlueScore
    {
        set
        {
            _blueScoreText.text = value.ToString();
        }
    }

    private void Awake()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("UIManager");
            if (go == null)
            {
                go = new GameObject { name = "UIManager" };
                go.AddComponent<SoundManager>();
            }

            s_instance = go.GetComponent<UIManager>();
        }
    }

    private void Start()
    {
        _optionPage = GameObject.Find("OptionPage").GetComponent<Canvas>();
        if (_optionPage)
            _optionPage.gameObject.SetActive(false);

        _occGauge = GameObject.Find("OccGauge").GetComponent<Slider>();
        _occGauge.value = 0;
        _redScoreText = GameObject.Find("RedScoreText").GetComponent<TextMeshProUGUI>();
        _blueScoreText = GameObject.Find("BlueScoreText").GetComponent<TextMeshProUGUI>();

        _fillAreaImage = _occGauge.transform.Find("Fill").GetComponent<Image>();


        //RefreshOccGauge(TeamColor.Red);
    }


    public void OnPauseClicked()
    {
        _optionPage.gameObject.SetActive(true);
    }

    public void OnPauseBackClicked()
    {
        _optionPage.gameObject.SetActive(false);
    }


    //public void RefreshScoreUI()
    //{
    //    RefreshRedScoreText();
    //    RefreshBlueScoreText();
    //}

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
    }

    public void RefreshRedScoreText(int value)
    {
        // TODO
        _redScoreText.text = value.ToString();
    }
    public void RefreshBlueScoreText(int value)
    {
        // TODO
        _blueScoreText.text = value.ToString();
    }
}

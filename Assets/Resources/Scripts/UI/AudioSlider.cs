using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class AudioSlider : MonoBehaviour
{
    Slider _audioSlider;
    TextMeshProUGUI _valueText;
    TextMeshProUGUI _nameText;

    private void Start()
    {
        _audioSlider = GetComponent<Slider>();
        _valueText = transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
        _nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();


        float Mixervalue;
        SoundManager.Instance.MasterMixer.GetFloat(_nameText.text, out Mixervalue);

        _audioSlider.value = Mixervalue;
    }

    public void AudioControl(int type)
    {
        float value = _audioSlider.value;
        _valueText.text = $"{(int)(((value + 40) / 40.0f) * 100)}";

        SoundManager.Instance.SetMixerValue((Sound)type, value);

        // Effect 사운드 조절시 사운드 한번 출력

        //if ((Sound)type == Sound.Effect)
        //    if (Input.GetMouseButtonDown(0))
        //        SoundManager.Instance.Play("Player_Shot");
    }
}

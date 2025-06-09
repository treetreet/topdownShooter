using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum Sound
{
    BGM,
    Effect,

    MaxCount
}

public class SoundManager : MonoBehaviour
{
    static SoundManager s_instance;
    public static SoundManager Instance { get { return s_instance; } }

    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount];
    private AudioMixer _masterMixer;


    private string _curBGM;

    public AudioMixer MasterMixer { get { return _masterMixer; } }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {

    }

    public void Init()
    {
        if (s_instance == null)
        {
            GameObject sound = GameObject.Find("SoundManager");
            if (sound == null)
            {
                sound = new GameObject { name = "SoundManager" };
                sound.AddComponent<SoundManager>();

                string[] soundNames = System.Enum.GetNames(typeof(Sound));
                for (int i = 0; i < soundNames.Length - 1; i++)
                {
                    GameObject go = new GameObject { name = soundNames[i] };
                    _audioSources[i] = go.AddComponent<AudioSource>();
                    go.transform.parent = sound.transform;
                }

                _audioSources[(int)Sound.BGM].loop = true;
            }
            else
            {
                string[] soundNames = System.Enum.GetNames(typeof(Sound));
                for (int i = 0; i < soundNames.Length - 1; i++)
                    _audioSources[i] = transform.Find($"{soundNames[i]}").GetComponent<AudioSource>();
            }
            DontDestroyOnLoad(sound);
            s_instance = sound.GetComponent<SoundManager>();
        }

        _masterMixer = Resources.Load<AudioMixer>("Sound/Master");
    }

    public void Play(string key, Sound type = Sound.Effect, float volumeScale = 1.0f)
    {
        AudioClip audioClip;

        if (type == Sound.BGM && key == _curBGM)
            return;

        if (!_audioClips.TryGetValue(key, out audioClip))
        {
            string path = "";

            switch (type)
            {
                case Sound.BGM:
                    path = "Sound/BGM/";
                    break;
                case Sound.Effect:
                    path = "Sound/Effect/";
                    break;
            }

            path += key;
            audioClip = Resources.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.Log("사운드 로드 실패!!!");
                return;
            }

            _audioClips.Add(key, audioClip);
        }

        if (type == Sound.BGM)
        {
            AudioSource audioSource = _audioSources[(int)Sound.BGM];
            if (audioSource.isPlaying)
                audioSource.Stop();

            //audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();

            _curBGM = key;
        }
        else if (type == Sound.Effect)
        {
            AudioSource audioSource = _audioSources[(int)Sound.Effect];
            //audioSource.pitch = pitch;

            if (volumeScale == 1.0f)
                audioSource.PlayOneShot(audioClip);
            else
                audioSource.PlayOneShot(audioClip, volumeScale);
        }
    }

    public void StopCurBGM()
    {
        AudioSource audioSource = _audioSources[(int)Sound.BGM];
        audioSource.Stop();
    }

    public void SetMixerValue(Sound type, float value)
    {
        if (value <= -40.0f)
            value = -80.0f;

        switch (type)
        {
            case Sound.BGM:
                _masterMixer.SetFloat("BGM", value);
                break;
            case Sound.Effect:
                _masterMixer.SetFloat("Effect", value);
                break;
            case Sound.MaxCount:
                _masterMixer.SetFloat("Master", value);
                break;
        }
    }


    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }
}

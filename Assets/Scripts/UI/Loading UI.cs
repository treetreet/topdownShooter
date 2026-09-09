using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI  loadingText;

    private void OnEnable()
    {
        StartCoroutine(LoadingTextChange());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void FailToJoinLobby()
    {
        StartCoroutine(FailToJoinLobbyCoroutine());
    }

    private IEnumerator FailToJoinLobbyCoroutine()
    {
        loadingText.color = Color.red;
        loadingText.text = "Fail to join lobby";
        
        yield return new WaitForSecondsRealtime(2f);
        gameObject.SetActive(false);
    }

    private IEnumerator LoadingTextChange()
    {
        loadingText.color = Color.black;

        while (true)
        {
            loadingText.text = "Loading";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading...";
        }
    }
}

using UnityEngine;

public class ShowUIGroup : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject sessionListUI;
    [SerializeField] private GameObject loadingUI;
    
    private void Start()
    {
        GoMain();
    }

    public void GoMain()
    {
        DisableAllUI();
        mainUI.SetActive(true);
    }
    
    public void GoSessionList()
    {
        DisableAllUI();
        sessionListUI.SetActive(true);
    }

    public void JoiningLobby()
    {
        loadingUI.SetActive(true);
    }

    public void JoinFailLobby()
    {
        loadingUI.GetComponent<LoadingUI>().FailToJoinLobby();
    }
    

    private void DisableAllUI()
    {
        mainUI.SetActive(false);
        sessionListUI.SetActive(false);
        loadingUI.SetActive(false);
    }
}

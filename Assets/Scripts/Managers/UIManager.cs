using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager s_instance;
    public static UIManager Instance { get { return s_instance; } }
}

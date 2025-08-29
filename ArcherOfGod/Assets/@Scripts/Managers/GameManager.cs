using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject StartObject;
    public GameObject WinObject;
    public GameObject LoseObject;
    public bool GameStart = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void BattleFinish(bool win)
    {
        if (win == true)
        {
            WinObject.SetActive(true);
        }
        else
        {
            LoseObject.SetActive(true);
        }
        GameStart = false;
    }
    private void Start()
    {
        WinObject.SetActive(false);
        LoseObject.SetActive(false);
    }
}

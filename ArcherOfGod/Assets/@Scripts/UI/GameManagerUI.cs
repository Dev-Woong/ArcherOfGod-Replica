using UnityEngine;

public class GameManagerUI : MonoBehaviour
{
    public void StartUiAnimationEvent()
    {
        GameManager.Instance.GameStart = true;
        gameObject.SetActive(false);
    }
}

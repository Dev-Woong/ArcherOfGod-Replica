using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool BattleStart()
    {
        return true;
    }
    public bool BattleFinish()
    {
        return false;
    }
    void Update()
    {
        
    }
}

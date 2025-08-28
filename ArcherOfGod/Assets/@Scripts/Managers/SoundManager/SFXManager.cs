using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sfxSource;
    public static SFXManager s_Instance { get; private set; }

    private void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAttackSFX(AudioClip clip)
    {
        if (clip != null)
            _sfxSource.PlayOneShot(clip);

    }
}


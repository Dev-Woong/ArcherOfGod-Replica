using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    [SerializeField]private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClip;
    int randomBGMIndex;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomBGMIndex = Random.Range(0, _audioClip.Length);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _audioClip[randomBGMIndex];
        _audioSource.Play();
    }
}

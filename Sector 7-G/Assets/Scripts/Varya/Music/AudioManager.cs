using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip digitClip;
    [SerializeField] private AudioClip callSuccessClip;
    [SerializeField] private AudioClip callFailClip;
    [SerializeField] private AudioClip problemClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClick() => Play(clickClip);
    public void PlayDigit() => Play(digitClip);
    public void PlayCallSuccess() => Play(callSuccessClip);
    public void PlayCallFail() => Play(callFailClip);
    public void PlayProblem() => Play(problemClip);

    private void Play(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
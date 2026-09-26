using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Playlist")]
    [Tooltip("Треки будут играть по очереди по кругу. Можно положить 1, 2, 3 — сколько нужно.")]
    [SerializeField] private AudioClip[] playlist;

    [Header("Settings")]
    [SerializeField] private bool shuffle = false;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.3f;

    private AudioSource _source;
    private int _currentIndex;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();

        _source.loop = false;        
        _source.playOnAwake = false;
        _source.volume = volume;

        if (playlist == null || playlist.Length == 0) return;

        _currentIndex = shuffle ? Random.Range(0, playlist.Length) : 0;
    }

    private void Start()
    {
        if (playlist == null || playlist.Length == 0) return;
        PlayCurrent();
    }

    private void Update()
    {
        if (_source == null) return;
        if (playlist == null || playlist.Length == 0) return;

        if (!_source.isPlaying)
            NextTrack();
    }

    private void NextTrack()
    {
        if (playlist.Length == 1)
        {
            PlayCurrent();  
            return;
        }

        if (shuffle)
        {
            int newIndex;
            do { newIndex = Random.Range(0, playlist.Length); }
            while (newIndex == _currentIndex && playlist.Length > 1);
            _currentIndex = newIndex;
        }
        else
        {
            _currentIndex = (_currentIndex + 1) % playlist.Length;
        }

        PlayCurrent();
    }

    private void PlayCurrent()
    {
        _source.clip = playlist[_currentIndex];
        _source.Play();
    }
}
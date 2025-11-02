using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [System.Serializable]
    public class SFXEntry
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private List<SFXEntry> soundEffects = new List<SFXEntry>();
    private Dictionary<string, AudioClip> sfxDictionary;
    private AudioSource audioSource;
    private AudioSource musicSource;


    void Start()
    {
        
    }

    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        StopLoopingMusic();
        InitializeSFXDictionary();
    }

    private void InitializeSFXDictionary()
    {
        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (var entry in soundEffects)
        {
            if (!sfxDictionary.ContainsKey(entry.name))
            {
                sfxDictionary.Add(entry.name, entry.clip);
            }
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {

            audioSource.PlayOneShot(clip);

        }
        else
        {
            Debug.LogWarning($"Sound effect '{name}' not found!");
        }
    }


    public void PlayLoopingMusic(string name, float startTime = 0f)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.time = startTime;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip '{name}' not found!");
        }
    }

    public void StopLoopingMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}

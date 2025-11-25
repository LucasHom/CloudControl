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
    private Coroutine fadeCoroutine;

    // Added new fields for volume and fade settings
    [Header("Music Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;   // added
    [Range(0f, 1f)] public float sfxVolume = 1f;     // added
    public float fadeInDuration;
    public float fadeOutDuration;

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
            return; // added
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true; // added
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
            audioSource.PlayOneShot(clip, sfxVolume); // changed (added volume param)
        }
        else
        {
            Debug.LogWarning($"Sound effect '{name}' not found!");
        }
    }

    public void PlaySFX(string name, float volumeScale)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            // volumeScale multiplies your global sfxVolume
            audioSource.PlayOneShot(clip, sfxVolume * Mathf.Clamp01(volumeScale));
        }
        else
        {
            Debug.LogWarning($"Sound effect '{name}' not found!");
        }
    }


    public void PlayLoopingMusic(string name, float startTime = 0f, bool fadeIn = true)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            // Stop any existing fade (so old fade-out doesn't interfere)
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            musicSource.clip = clip;
            musicSource.time = startTime;
            musicSource.volume = fadeIn ? 0f : musicVolume;
            musicSource.Play();

            if (fadeIn)
                fadeCoroutine = StartCoroutine(FadeInMusic());
        }
        else
        {
            Debug.LogWarning($"Music clip '{name}' not found!");
        }
    }

    public void StopLoopingMusic(bool fadeOut = true)
    {
        if (musicSource.isPlaying)
        {
            // Stop any existing fade before starting fade-out
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            if (fadeOut)
                fadeCoroutine = StartCoroutine(FadeOutMusic());
            else
                musicSource.Stop();
        }
    }

    // Added fade-in coroutine
    private IEnumerator FadeInMusic() // added
    {
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            float delta = (Time.timeScale == 0f) ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsed += delta;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
            musicSource.volume = Mathf.SmoothStep(0f, musicVolume, t); // Smooth fade
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    // Fade-out coroutine
    private IEnumerator FadeOutMusic() // added
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            float delta = (Time.timeScale == 0f) ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsed += delta;
            float t = Mathf.Clamp01(elapsed / fadeOutDuration);
            musicSource.volume = Mathf.SmoothStep(startVolume, 0f, t);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = musicVolume; // reset for next play
    }

    // Added methods for changing volume at runtime
    public void SetMusicVolume(float volume) // added
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume) // added
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}

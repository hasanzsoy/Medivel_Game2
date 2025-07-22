using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class NamedAudioClip
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Ses Kütüphanesi")]
    public NamedAudioClip[] musicClips;
    public NamedAudioClip[] sfxClips;

    private Dictionary<string, AudioClip> musicLibrary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxLibrary = new Dictionary<string, AudioClip>();

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            BuildLibraries();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void BuildLibraries()
    {
        musicLibrary.Clear();
        sfxLibrary.Clear();
        foreach (var item in musicClips)
        {
            if (item != null && item.clip != null && !string.IsNullOrEmpty(item.name))
                musicLibrary[item.name] = item.clip;
        }
        foreach (var item in sfxClips)
        {
            if (item != null && item.clip != null && !string.IsNullOrEmpty(item.name))
                sfxLibrary[item.name] = item.clip;
        }
    }

    private void Start()
    {
        LoadVolumes();
        SetupSliders();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAudioSourcesAndSliders();
        LoadVolumes();
        SetupSliders();
    }

    private void FindAudioSourcesAndSliders()
    {
        musicSource = GameObject.FindWithTag("MusicSource")?.GetComponent<AudioSource>();
        sfxSource = GameObject.FindWithTag("SFXSource")?.GetComponent<AudioSource>();
        musicSlider = GameObject.FindWithTag("MusicSlider")?.GetComponent<Slider>();
        sfxSlider = GameObject.FindWithTag("SFXSlider")?.GetComponent<Slider>();
    }

    private void LoadVolumes()
    {
        float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        float sfxVol = PlayerPrefs.GetFloat(SfxVolumeKey, 0.5f);
        if (musicSource != null) musicSource.volume = musicVol;
        if (sfxSource != null) sfxSource.volume = sfxVol;
    }

    private void SetupSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.value = musicSource != null ? musicSource.volume : PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxSource != null ? sfxSource.volume : PlayerPrefs.GetFloat(SfxVolumeKey, 0.5f);
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float value)
    {
        if (sfxSource != null)
            sfxSource.volume = value;
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
        PlayerPrefs.Save();
    }

    // Müzik çalmak için
    public void PlayMusic(string name, bool loop = true)
    {
        if (musicLibrary.TryGetValue(name, out var clip) && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Müzik bulunamadý: {name}");
        }
    }

    // Efekt çalmak için
    public void PlaySFX(string name)
    {
        if (sfxLibrary.TryGetValue(name, out var clip) && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxSource.volume);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX bulunamadý: {name}");
        }
    }

    // Doðrudan AudioClip ile de çalabilirsin
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, sfxSource.volume);
    }
}
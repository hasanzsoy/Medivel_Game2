using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }

    public static AudioManager Instance;

    [Header("Mixer ve Gruplar")]
    public AudioMixer audioMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup deathGroup;
    public AudioMixerGroup footstepGroup;
    public AudioMixerGroup jumpGroup;
    public AudioMixerGroup swordGroup;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource deathSource;
    public AudioSource footstepSource; 
    public AudioSource jumpSource;
    public AudioSource swordSource;

    [Header("Ses Kütüphanesi")]
    public NamedAudioClip[] musicClips;
    public NamedAudioClip[] sfxClips;

    private Dictionary<string, AudioClip> musicLibrary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxLibrary = new Dictionary<string, AudioClip>();

    // Exposed parameter isimleri
    private const string MasterVolumeParam = "MasterVolume";
    private const string MusicVolumeParam = "MusicVolume";
    private const string SFXVolumeParam = "SFXVolume";
    private const string DeathVolumeParam = "DeathVolume";
    private const string FootstepVolumeParam = "FootstepVolume";
    private const string JumpVolumeParam = "JumpVolume";
    private const string SwordVolumeParam = "SwordVolume";

    // PlayerPrefs anahtarlarý
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string DeathVolumeKey = "DeathVolume";
    private const string FootstepVolumeKey = "FootstepVolume";
    private const string JumpVolumeKey = "JumpVolume";
    private const string SwordVolumeKey = "SwordVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildLibraries();
            LoadVolumes();
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
            if (item != null && item.clip != null && !string.IsNullOrEmpty(item.name))
                musicLibrary[item.name] = item.clip;
        foreach (var item in sfxClips)
            if (item != null && item.clip != null && !string.IsNullOrEmpty(item.name))
                sfxLibrary[item.name] = item.clip;
    }

    // --- SLIDER FONKSÝYONLARI ---
    public void SetMasterVolume(float value) => SetVolume(MasterVolumeParam, MasterVolumeKey, value);
    public void SetMusicVolume(float value) => SetVolume(MusicVolumeParam, MusicVolumeKey, value);
    public void SetSFXVolume(float value) => SetVolume(SFXVolumeParam, SFXVolumeKey, value);
    public void SetDeathVolume(float value) => SetVolume(DeathVolumeParam, DeathVolumeKey, value);
    public void SetFootstepVolume(float value) => SetVolume(FootstepVolumeParam, FootstepVolumeKey, value);
    public void SetSwordVolume(float value) => SetVolume(SwordVolumeParam, SwordVolumeKey, value);

    private void SetVolume(string mixerParam, string prefsKey, float value)
    {
        // Unity mixer için volume genelde -80 ile 0 dB arasýdýr, slider 0-1 arasý olmalý
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(mixerParam, dB);
        PlayerPrefs.SetFloat(prefsKey, value);
        PlayerPrefs.Save();
    }

    private void LoadVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFXVolumeKey, 1f));
        SetDeathVolume(PlayerPrefs.GetFloat(DeathVolumeKey, 1f));
        SetFootstepVolume(PlayerPrefs.GetFloat(FootstepVolumeKey, 1f));
        SetSwordVolume(PlayerPrefs.GetFloat(SwordVolumeKey, 1f));
    }

    // --- EVENT BAZLI SES OYNATMA ---

    public void PlayMainMenuMusic()
    {
        PlayMusic("mainmenu");
    }

    // Oyun müziði için
    public void PlayGameMusic(string name)
    {
        PlayMusic(name);
    }

    public void PlayMusic(string name, bool loop = true)
    {
        if (musicLibrary.TryGetValue(name, out var clip) && musicSource != null)
        {
            musicSource.outputAudioMixerGroup = musicGroup;
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Müzik bulunamadý: {name}");
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxLibrary.TryGetValue(name, out var clip) && sfxSource != null)
        {
            sfxSource.outputAudioMixerGroup = sfxGroup;
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX bulunamadý: {name}");
        }
    }

    public void PlayDeath(string name)
    {
        if (sfxLibrary.TryGetValue(name, out var clip) && deathSource != null)
        {
            deathSource.outputAudioMixerGroup = deathGroup;
            deathSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Death SFX bulunamadý: {name}");
        }
    }

    public void PlayFootstep(string name)
    {
        if (sfxLibrary.TryGetValue(name, out var clip) && footstepSource != null)
        {
            footstepSource.outputAudioMixerGroup = footstepGroup;
            footstepSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Footstep SFX bulunamadý: {name}");
        }
    }
    public void PlayJump(string name)
    {
        if (sfxLibrary.TryGetValue(name, out var clip) && footstepSource != null)
        {
            jumpSource.outputAudioMixerGroup = jumpGroup;
            jumpSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Footstep SFX bulunamadý: {name}");
        }
    }

    public void PlaySword(string name)
    {
        if (sfxLibrary.TryGetValue(name, out var clip) && swordSource != null)
        {
            swordSource.outputAudioMixerGroup = swordGroup;
            swordSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Sword SFX bulunamadý: {name}");
        }
    }
}


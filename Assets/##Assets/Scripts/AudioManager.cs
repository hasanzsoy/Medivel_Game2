using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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

    [Header("Music Crossfade")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;
    public float crossfadeDuration = 2f;


    private bool isCombat = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildLibraries();
            LoadVolumes();
            activeMusicSource = musicSourceA;
            inactiveMusicSource = musicSourceB;

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        AttachSlidersFromConvertibleSource();
        PlaySceneMusic();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        AttachSlidersFromConvertibleSource();
        LoadVolumes();
        PlaySceneMusic();
    }

    // Crossfade ile müzik çalma
    public void PlayMusicCrossfade(string name, bool loop = true)
    {
        if (!musicLibrary.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"[AudioManager] Müzik bulunamadý: {name}");
            return;
        }

        // Eðer aktif kaynakta zaten bu müzik çalýyorsa, tekrar baþlatma
        if (activeMusicSource.clip == clip && activeMusicSource.isPlaying)
            return;

        // Eðer aktif kaynakta müzik yoksa veya çalmýyorsa, direkt baþlat
        if (!activeMusicSource.isPlaying)
        {
            activeMusicSource.outputAudioMixerGroup = musicGroup;
            activeMusicSource.clip = clip;
            activeMusicSource.loop = loop;
            activeMusicSource.volume = 1f;
            activeMusicSource.Play();
            inactiveMusicSource.Stop();
            inactiveMusicSource.volume = 1f;
            return;
        }

        // Sadece aktif kaynakta müzik çalýyorsa ve yeni müzik geliyorsa crossfade baþlat
        inactiveMusicSource.outputAudioMixerGroup = musicGroup;
        inactiveMusicSource.clip = clip;
        inactiveMusicSource.loop = loop;
        inactiveMusicSource.volume = 0f;
        inactiveMusicSource.Play();
        StartCoroutine(CrossfadeMusicCoroutine());
    }


    private IEnumerator CrossfadeMusicCoroutine()
    {
        float timer = 0f;
        float startVolume = activeMusicSource.volume;
        while (timer < crossfadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / crossfadeDuration;
            activeMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            inactiveMusicSource.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        activeMusicSource.Stop();
        activeMusicSource.volume = 1f;
        // Swap sources
        var temp = activeMusicSource;
        activeMusicSource = inactiveMusicSource;
        inactiveMusicSource = temp;
    }

    // Sahne ve savaþ durumuna göre crossfade ile müzik çal
    private void PlaySceneMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "AnaMenu")
        {
            PlayMusicCrossfade(musicClips[0].name); // Main menu müziði
        }
        else if (sceneName == "Harita")
        {
            PlayMusicCrossfade(isCombat ? musicClips[2].name : musicClips[1].name);
        }
    }

    public void SetCombatState(bool combat)
    {
        if (isCombat == combat) return;
        isCombat = combat;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Harita")
        {
            PlayMusicCrossfade(isCombat ? musicClips[2].name : musicClips[1].name);
        }
    }

    public void PlayMusicByIndex(int index)
    {
        if (musicClips != null && index >= 0 && index < musicClips.Length)
        {
            var clipName = musicClips[index].name;
            PlayMusicCrossfade(clipName);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Ýstenen müzik indexi ({index}) yok!");
        }
    }

    private void AttachSlidersFromConvertibleSource()
    {
        var convertible = GameObject.Find(nameof(ConvertibleSource))?.GetComponent<ConvertibleSource>();
        if (convertible == null)
        {
            Debug.LogWarning("[AudioManager] ConvertibleSource bulunamadý!");
            return;
        }

        AttachSlider(convertible.masterSlider, SetMasterVolume, PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        AttachSlider(convertible.musicSlider, SetMusicVolume, PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
        AttachSlider(convertible.sfxSlider, SetSFXVolume, PlayerPrefs.GetFloat(SFXVolumeKey, 1f));
        AttachSlider(convertible.deathSlider, SetDeathVolume, PlayerPrefs.GetFloat(DeathVolumeKey, 1f));
        AttachSlider(convertible.footstepSlider, SetFootstepVolume, PlayerPrefs.GetFloat(FootstepVolumeKey, 1f));
        AttachSlider(convertible.jumpSlider, SetJumpVolume, PlayerPrefs.GetFloat(JumpVolumeKey, 1f));
        AttachSlider(convertible.swordSlider, SetSwordVolume, PlayerPrefs.GetFloat(SwordVolumeKey, 1f));
    }

    private void AttachSlider(UnityEngine.UI.Slider slider, System.Action<float> setFunc, float value)
    {
        if (slider == null) return;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(setFunc.Invoke);
        slider.value = value;
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
    public void SetJumpVolume(float value) => SetVolume(JumpVolumeParam, JumpVolumeKey, value);
    public void SetSwordVolume(float value) => SetVolume(SwordVolumeParam, SwordVolumeKey, value);

    private void SetVolume(string mixerParam, string prefsKey, float value)
    {
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
        SetJumpVolume(PlayerPrefs.GetFloat(JumpVolumeKey, 1f));
        SetSwordVolume(PlayerPrefs.GetFloat(SwordVolumeKey, 1f));
    }

    // --- EVENT BAZLI SES OYNATMA ---
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
        if (sfxLibrary.TryGetValue(name, out var clip) && jumpSource != null)
        {
            jumpSource.outputAudioMixerGroup = jumpGroup;
            jumpSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Jump SFX bulunamadý: {name}");
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
using UnityEngine;

public class CoreGameInfrastructure : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource MusicAudioSource;

    public AudioSource SFX_AudioSource;

    private SavedDataManager savedDataManager;

    public void Play_SFX_ExtendedOneShot(AudioClip Audio, float Volume, float StereoPan = 0.0f, float Pitch = 1.0f)
    // This method is used to play a one shot with additional settings for pan and pitch adjustment which is not
    // otherwise available when playing a one-shot sound. This is done by making new AudioSource objects.
    {
        GameObject NewAudioSource = new GameObject("SFX_ExtendedOneShot");
        AudioSource ExtendedAudioSourceComponent = NewAudioSource.AddComponent<AudioSource>();
        ExtendedOneShot ExtendedOneShotComponent = NewAudioSource.AddComponent<ExtendedOneShot>();
        ExtendedAudioSourceComponent.clip = Audio;
        ExtendedAudioSourceComponent.volume = Volume;
        ExtendedAudioSourceComponent.panStereo = StereoPan;
        ExtendedAudioSourceComponent.pitch = Pitch;
        ExtendedAudioSourceComponent.Play();
        ExtendedOneShotComponent.Lifetime = Audio.length;
        NewAudioSource.transform.parent = SFX_AudioSource.transform;
    }

    private void Awake()
    {
        if (Registry.CoreGameInfrastructureObject == null)
        {
            DontDestroyOnLoad(gameObject);
            Registry.CoreGameInfrastructureObject = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        savedDataManager = new SavedDataManager();
        savedDataManager.Load();
    }

    public void Save()
    {
        savedDataManager.Save();
    }

    public void Quit()
    {
        savedDataManager.Save();
        Application.Quit();
    }

    private void Update()
    {
        if (Registry.IsPaused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        MusicAudioSource.volume = Registry.Music_Volume * Registry.Master_Volume;
    }
}
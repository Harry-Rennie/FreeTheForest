using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


/// This class is used for controlling the in game music.
/// It is attached to the MusicManager game object.
/// It finds all the audiosources attached to the Musicanager game object and creates a list of AudioInfo objects.
/// When a scene is loaded, it finds the audiosource with the name of the scene and plays it.
/// </summary>
public class MusicManager : MonoBehaviour
{
    #region Singleton
    public static MusicManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This will make the object persist between scenes.
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate instances.
        }
    }
    #endregion

    private List<AudioInfo> audioInfos { get; } = new List<AudioInfo>();

    // This is the initial overall SFX volume. This is what SFX slider in the settings menu is set to when the game starts.
    public float InitSFXLevel = 0.25f;

    private void Start()
    {
        // find all audiosources in this object
        foreach (AudioSource audioSource in GetComponents<AudioSource>())
        {
            AudioInfo newAudioInfo = new AudioInfo(audioSource, audioSource.volume);
            // set the volume of the audiosource to the initial SFX level relative to its initial volume.
            newAudioInfo.Player.volume = (newAudioInfo.InitVolume * InitSFXLevel);
            audioInfos.Add(newAudioInfo);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Play(scene.name);
    }

    /// <summary>
    /// This method plays the music with same name as the scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene.</param>
    public void Play(string sceneName)
    {
        // Find the audiosource with supplied name and play it
        foreach (AudioInfo audioInfo in audioInfos)
        {
            if (audioInfo.name == sceneName)
            {
                audioInfo.Player.Play();
            }
        }
    }

    /// <summary>
    /// This method is used to change the overall music volume.
    /// </summary>
    /// <param name="volume">The new volume.</param>
    public void ChangeMusicVolume(float volume)
    {
        // Change the volume of each audiosource relative to its initial volume
        foreach (AudioInfo audioInfo in audioInfos)
        {
            audioInfo.Player.volume = (audioInfo.InitVolume * volume);
        }
    }
}

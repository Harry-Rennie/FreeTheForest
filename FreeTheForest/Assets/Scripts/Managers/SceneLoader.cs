using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    public static List<string> SceneNames = new List<string>() { };
    public static List<AsyncOperation> LoadedScenes;
    public static SceneLoader instance;
    private int _loadedSceneCount = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        LoadedScenes = new List<AsyncOperation>();
        LoadScenes();
    }

    private async void LoadScenes()
    {
        if (SceneNames.Count == 0)
        {
            return;
        }
        foreach (string sceneName in SceneNames)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            LoadedScenes.Add(asyncOperation);
        }
        bool allScenesLoaded = false;

        foreach (AsyncOperation asyncOperation in LoadedScenes)
        {
            if (asyncOperation.progress < 0.9f)
            {
                allScenesLoaded = false;
            }
        }

        if (allScenesLoaded)
        {
            // All scenes have finished loading
            foreach (AsyncOperation asyncOperation in LoadedScenes)
            {
                asyncOperation.allowSceneActivation = true;
            }
        }

        await Task.Yield();
        _loadedSceneCount = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Increment the loaded scene count
        _loadedSceneCount++;
    }
}
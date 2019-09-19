// This class handles loading and unloading application scenes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private int currentSceneIndex;

    private static SceneLoader _SceneLoader;
    public static SceneLoader sceneLoader
    {
        get
        {
            if (_SceneLoader == null)
            {
                _SceneLoader = GameObject.FindObjectOfType<SceneLoader>();
            }
            return _SceneLoader;
        }
    }

    public Material DarkSkybox;
    public Material NightSkybox;

    bool started = false;


    private void Awake()
    {
        if (!started)
        {
            _SceneLoader = this;
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            currentSceneIndex = 1;
            started = true;
        }
    }

    public void ChangeScene(int nextSceneIndex)
    {
        SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Additive);
        StartCoroutine(UnloadScene(currentSceneIndex));
        currentSceneIndex = nextSceneIndex;

        switch (nextSceneIndex)
        {
            case 0:
                Debug.Log("Scene load index error - 0");
                break;
            case 1:
                SetSkybox(DarkSkybox);
                break;
            case 2:
                break;
            case 3:
                SetSkybox(NightSkybox);
                break;
            default:
                Debug.Log("unable to swap skybox");
                return;
        }
    }

    IEnumerator UnloadScene(int index)
    {
        yield return null;
        SceneManager.UnloadSceneAsync(index);
    }

    void SetSkybox(Material newSkybox)
    {
        RenderSettings.skybox = newSkybox;
    }
}
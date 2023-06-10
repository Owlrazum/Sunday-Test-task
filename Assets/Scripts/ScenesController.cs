using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    AsyncOperation _loadingScene;

    void Awake()
    {
        ApplicationDelegatesContainer.LoadGallery += LoadGallery;

        UIDelegatesContainer.GetSceneLoadingProgress += UpdateSceneLoadingProgress;

        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    { 
        ApplicationDelegatesContainer.LoadGallery -= LoadGallery;

        UIDelegatesContainer.GetSceneLoadingProgress -= UpdateSceneLoadingProgress;
    }

    void LoadGallery()
    {
        SceneManager.LoadScene(1);
        StartCoroutine(DemonstrateLoadScreen());
    }

    IEnumerator DemonstrateLoadScreen()
    {
        yield return new WaitForSeconds(1);
        _loadingScene = SceneManager.LoadSceneAsync(2);
        _loadingScene.allowSceneActivation = true;
    }

    float UpdateSceneLoadingProgress()
    {
        if (_loadingScene != null)
        { 
            return _loadingScene.progress;
        }
        else
        {
            return -1;
        }
    }
}
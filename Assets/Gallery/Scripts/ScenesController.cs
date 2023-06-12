using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    AsyncOperation _loadingScene;

    [SerializeField]
    int _gallerySceneIndex = 2;

    [SerializeField]
    int _previewSceneIndex = 3;

    bool _isReturningToGallery;
    float _progress;


    void Awake()
    {
        ApplicationDelegatesContainer.LoadGallery += LoadGallery;
        ApplicationDelegatesContainer.LoadPreview += LoadPreview;

        UIDelegatesContainer.GetSceneLoadingProgress += GetSceneLoadingProgress;

        ApplicationDelegatesContainer.OnGalleryReturnCommand += OnGalleryReturnCommand;

        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    { 
        ApplicationDelegatesContainer.LoadGallery -= LoadGallery;
        ApplicationDelegatesContainer.LoadPreview -= LoadPreview;

        UIDelegatesContainer.GetSceneLoadingProgress -= GetSceneLoadingProgress;

        ApplicationDelegatesContainer.OnGalleryReturnCommand -= OnGalleryReturnCommand;
    }

    void LoadGallery()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene(1);
        StartCoroutine(DemonstrateLoadScreen(_gallerySceneIndex));
    }

    void LoadPreview()
    {
        SceneManager.LoadScene(1);
        StartCoroutine(DemonstrateLoadScreen(_previewSceneIndex));
    }

    IEnumerator DemonstrateLoadScreen(int sceneIndex)
    {
        yield return new WaitForSeconds(1);
        _loadingScene = SceneManager.LoadSceneAsync(sceneIndex);
        _loadingScene.allowSceneActivation = false;
        yield return new WaitForSeconds(1);
        _loadingScene.allowSceneActivation = true;
        if (_isReturningToGallery)
        {
            ApplicationDelegatesContainer.OnReturnedToGallery();
            _isReturningToGallery = false;
        }
        _loadingScene = null;
    }

    float GetSceneLoadingProgress()
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

    void OnGalleryReturnCommand()
    {
        _isReturningToGallery = true;
    }
}
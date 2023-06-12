using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    Button _button;
    Image _image;

    CanvasScaler _scaler;

    void Awake()
    {
        TryGetComponent(out _scaler);
        transform.GetChild(0).TryGetComponent(out _button);
        _button.onClick.AddListener(OnClick);
        transform.GetChild(1).TryGetComponent(out _image);

        Image image = ApplicationDelegatesContainer.GetImageToPreview();
        _image.sprite = image.sprite;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        { 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClick();
            }
        }

        if (Screen.width > Screen.height)
        {
            _scaler.referenceResolution = new Vector2(1920, 1080);
        }
        else 
        {
            _scaler.referenceResolution = new Vector2(1080, 1920);
        }
    }

    void OnClick()
    {
        ApplicationDelegatesContainer.OnGalleryReturnCommand();
        ApplicationDelegatesContainer.LoadGallery();
    }
}
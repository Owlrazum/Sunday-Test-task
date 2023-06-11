using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    Button _button;
    Image _image;

    void Awake()
    {
        transform.GetChild(0).TryGetComponent(out _button);
        _button.onClick.AddListener(OnClick);
        transform.GetChild(1).TryGetComponent(out _image);

        Image image = ApplicationDelegatesContainer.GetImageToPreview();
        _image.sprite = image.sprite;
    }

    void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    void OnClick()
    {
        ApplicationDelegatesContainer.OnGalleryReturnCommand();
        ApplicationDelegatesContainer.LoadGallery();
    }
}
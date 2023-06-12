using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        transform.GetChild(0).TryGetComponent(out _button);
        _button.onClick.AddListener(OnClick);

        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OnClick()
    {
        ApplicationDelegatesContainer.LoadGallery();
    }
}
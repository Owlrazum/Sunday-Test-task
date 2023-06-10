using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    [SerializeField]
    RectTransform _imagesParent;

    [SerializeField]
    int _columnSize = 33;

    [SerializeField]
    Vector2Int _leftRightPos = new Vector2Int(-400, 400);

    [SerializeField]
    float _scrollRectHeight = 1800;

    [SerializeField]
    int _columnsGap = 50;

    [SerializeField]
    int _columnsCount = 2;

    [SerializeField]
    int _imagesCount = 66;

    [SerializeField]
    Material _loadingMaterial;

    [SerializeField]
    Material _usualMaterial;

    [SerializeField]
    ScrollRect _scrollRect;

    List<List<Image>> _imagesByColumn;
    HashSet<int> _requestedImages;

    int _columnWidth;
    float _contentSize;

    int _lastLoadedImageId;
    float _loadedHeight;

    void Awake()
    {
        int count = _columnSize * _columnsCount;
        _requestedImages = new(count);
        _imagesByColumn = new List<List<Image>>(_columnsCount);
        for (int i = 0; i < _columnsCount; i++)
        {
            _imagesByColumn.Add(new(count / _columnsCount));
        }
        _columnWidth = (_leftRightPos.y - _leftRightPos.x) / _columnsCount;
        _columnWidth -= _columnsGap;

        _contentSize = (_columnWidth + _columnsGap) * _columnSize;
        _imagesParent.sizeDelta = new Vector2(_imagesParent.sizeDelta.x, _contentSize);

        _scrollRect.onValueChanged.AddListener(OnScroll);

        int imageId = 0;
        int columnId = imageId % _columnsCount;
        while (_loadedHeight <= _scrollRectHeight || columnId != 0)
        {
            CreateImage(imageId, columnId);
            StartCoroutine(Web.Request(imageId, FillTexture));
            // _requestedImages.Add(imageId);
            imageId++;
            columnId = imageId % _columnsCount;
        }
        _lastLoadedImageId = imageId;
    }

    void OnScroll(Vector2 scroll)
    {
        float targetHeight = _scrollRectHeight + (1 - scroll.y) * _contentSize;
        if (_loadedHeight > targetHeight)
        {
            return;
        }

        int imageId = _lastLoadedImageId;
        if (imageId == 66)
        {
            return;
        }
        int columnId = imageId % _columnsCount;
        while (_loadedHeight <= targetHeight || columnId != 0)
        {
            CreateImage(imageId, columnId);
            StartCoroutine(Web.Request(imageId, FillTexture));
            imageId++;
            columnId = imageId % _columnsCount;
        }
        _lastLoadedImageId = imageId;
    }

    void CreateImage(int imageId, int columnId)
    {
        GameObject imageGb = new GameObject("image");
        Image image = imageGb.AddComponent<Image>();
        image.material = _loadingMaterial;
        var column = _imagesByColumn[columnId];

        RectTransform rect = image.rectTransform;
        rect.SetParent(_imagesParent, false);
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0, 1);

        float posY = -_columnsGap;
        if (column.Count != 0)
        { 
            RectTransform prevRect = column[column.Count - 1].rectTransform;
            posY = prevRect.anchoredPosition.y - prevRect.sizeDelta.y - _columnsGap;
        }
        rect.anchoredPosition = new Vector2(
            _leftRightPos.x + columnId * (_columnWidth + _columnsGap)
            , posY); 
        rect.sizeDelta = new Vector2Int(_columnWidth, _columnWidth);

        _loadedHeight = Mathf.Max(Mathf.Abs(rect.anchoredPosition.y) + rect.sizeDelta.y + _columnsGap, _loadedHeight);
        column.Add(image);
    }

    void FillTexture(Texture2D texture, int imageId)
    {
        int columnId = imageId % _columnsCount;
        int imageByColumnId = imageId / _columnsCount;
        var column = _imagesByColumn[columnId];
        var image = column[imageByColumnId];
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 1)); ;
        image.sprite = sprite;
        image.material = _usualMaterial;

        RectTransform rect = image.rectTransform;
        Vector2 sizeDelta = rect.sizeDelta;

        // a partial solution to different aspect ratio, the logic of determining whether new image should be loaded is missing.
        // // In case the image's aspect ratio is not 1:1, change image's sizeDelta. 
        // // In addition, all images should be offset.
        // void AdjustImageAspectRatio()
        // {
        //     if (texture.width != texture.height)
        //     {
        //         int toReduceWidth = texture.width - _columnWidth;
        //         int toReduceHeight = texture.height / toReduceWidth * toReduceWidth;
        //         sizeDelta = new Vector2Int(_columnWidth, texture.height - toReduceHeight);
        //         float delta = sizeDelta.y - rect.sizeDelta.y;
        //         rect.sizeDelta = sizeDelta;

        //         for (int i = imageByColumnId + 1; i < _columnSize; i++)
        //         {
        //             column[i].rectTransform.anchoredPosition -= Vector2.down * delta;
        //         }
        //     }
        // }

        // AdjustImageAspectRatio();
    }
}
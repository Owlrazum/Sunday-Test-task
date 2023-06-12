using System;

using UnityEngine.UI;

public static class ApplicationDelegatesContainer
{
    public static Func<WebHandler> GetWebHandler;
    
    public static Action LoadGallery;
    public static Action LoadPreview;

    public static Func<Image> GetImageToPreview;
    
    public static Action OnGalleryReturnCommand;
    public static Action OnReturnedToGallery;
}
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

public class WebHandler : MonoBehaviour
{
    const string serverUrl = "http://data.ikppbb.com/test-task-unity-data/pics/";

    void Awake()
    {
        DontDestroyOnLoad(this);

        ApplicationDelegatesContainer.GetWebHandler += GetWebHandler;
    }

    WebHandler GetWebHandler()
    {
        return this;
    }

    public void StartRequest(int imageNumber, Action<Texture2D, int> textureToFill)
    {
        StartCoroutine(Request(imageNumber, textureToFill));
    }

    public static IEnumerator Request(int imageNumber, Action<Texture2D, int> textureToFill)
    {
        imageNumber++; // the server is starting counting from one.
        string imageUrl = serverUrl + imageNumber + ".jpg";
        using (var request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                // reduce imageNumber to 0 based counting as it was;
                textureToFill(DownloadHandlerTexture.GetContent(request), imageNumber - 1);
            }
        }
    }
}
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

public static class Web
{
    const string serverUrl = "http://data.ikppbb.com/test-task-unity-data/pics/";

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
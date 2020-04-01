using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class ImageLoader
{
    private static string GetImageUri(string fileName)
    {
        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, "images", fileName));
        return uri.AbsoluteUri;
    }

    public static IEnumerator LoadImage(string imagePath, Texture texture)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(GetImageUri(imagePath));
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            texture = null;
        }
        else
        {
            Debug.Log("Image succesfully loaded");
            texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/ImageCacher")]
public class ImageCacher : ScriptableObject
{
    [SerializeField] private List<Sprite> sprites;
    private Dictionary<string, int> dict;

    void OnEnable()
    {
        sprites = new List<Sprite>();
        dict = new Dictionary<string, int>();
    }

    public IEnumerator GetSpriteAsync(string imagePath, Action<Sprite> callback)
    {
        if (dict.ContainsKey(imagePath))
        {
            callback?.Invoke(sprites[dict[imagePath]]);
        }
        else
        {
            yield return LoadSpriteAsync(imagePath, callback);
        }
    }

    public void RegisterSprite(string imagePath, Sprite sprite)
    {
        if (dict.ContainsKey(imagePath)) { Debug.LogWarning($"the same key {imagePath} {dict[imagePath]}"); return; }
        dict.Add(imagePath, sprites.Count);
        sprites.Add(sprite);
    }

    private IEnumerator LoadSpriteAsync(string imagePath, Action<Sprite> callback)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(GetImageUri(imagePath));
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            callback?.Invoke(null);
        }
        else
        {
            Debug.Log("Image succesfully loaded");
            Texture2D t = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
            RegisterSprite(imagePath, sprite);
            callback?.Invoke(sprite);
        }
    }

    private static string GetImageUri(string fileName)
    {
        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, "images", fileName));
        return uri.AbsoluteUri;
    }
}
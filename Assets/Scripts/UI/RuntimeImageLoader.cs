using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class RuntimeImageLoader : ScriptableObject
{
    public Texture texture;
    [SerializeField]
    private string path;

    public string Path { get => path; set => path = value; }

    public IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(Path);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Image succesfully loaded");
            this.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace UI
{
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
                texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }

    }
}

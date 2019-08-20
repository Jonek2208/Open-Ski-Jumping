using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToTexture : MonoBehaviour
{
    public Font mFont;
    public List<MeshFilter> mPlaneFilters;

    void Awake()
    {
        foreach (MeshFilter MeshFilter in mPlaneFilters)
        {
            MeshFilter.GetComponent<MeshRenderer>().material.mainTexture = mFont.material.mainTexture;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // CreateFont("R");
    }
    void Start()
    {
        
    }

    private void CreateFont(string output)
    {
        mFont.RequestCharactersInTexture(output);

        for (int i = 0; i < output.Length; i++)
        {
            CharacterInfo character;
            mFont.GetCharacterInfo(output[i], out character);

            Vector2[] uvs = new Vector2[4];
            uvs[0] = character.uvBottomLeft;
            uvs[1] = character.uvTopRight;
            uvs[2] = character.uvBottomRight;
            uvs[3] = character.uvTopLeft;

            mPlaneFilters[i].mesh.uv = uvs;

            Vector3 newScale = mPlaneFilters[i].transform.localScale;
            newScale.x = character.glyphWidth * 0.02f;

            mPlaneFilters[i].transform.localScale = newScale;
        }
    }
}

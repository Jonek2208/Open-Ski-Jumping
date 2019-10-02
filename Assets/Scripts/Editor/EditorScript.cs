#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HillProfile;

[CustomEditor(typeof(MeshScript))]
public class EditorScript : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        MeshScript meshScript = (MeshScript)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            meshScript.hill = new HillProfile.Hill(meshScript.profileData);
            meshScript.GenerateMesh(meshScript.hill);
        }
    }
}

[CustomEditor(typeof(TerrainScript))]
public class EditorTerrainScript : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        TerrainScript terrainScript = (TerrainScript)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            terrainScript.GenerateTerrain();
        }
    }
}
#endif
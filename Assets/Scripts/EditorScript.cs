#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
#endif
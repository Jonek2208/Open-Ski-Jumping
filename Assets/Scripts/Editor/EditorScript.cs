#if UNITY_EDITOR
using Competition.Runtime;
using Data;
using Hills;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RuntimeData), true)]
    public class DatabaseObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            RuntimeData databaseObject = (RuntimeData)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Load Data from file"))
            {
                databaseObject.LoadData();
            }
            if (GUILayout.Button("Save Data to file"))
            {
                databaseObject.SaveData();
            }
        }
    }

// [CustomEditor(typeof(JumperInfo))]
// public class JumperInfoEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         JumperInfo value = (JumperInfo)target;
//         DrawDefaultInspector();
//         if (GUILayout.Button("Update Bound Competitor"))
//         {
//             value.UpdateBoundCompetitor();
//         }
//         if (GUILayout.Button("Refresh Shown Value"))
//         {
//             value.RefreshShownValue();
//         }
//     }
// }

    [CustomEditor(typeof(MeshScript))]
    public class EditorScript : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MeshScript meshScript = (MeshScript)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                meshScript.GenerateMesh();
            }
        }
    }

    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GameEvent gameEvent = (GameEvent)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Raise"))
            {
                Debug.Log("RAISED EVENT");
                gameEvent.Raise();
            }
        }
    }

    [CustomEditor(typeof(RuntimeJumpData))]
    public class RuntimeJumpDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GameEvent gameEvent = (GameEvent)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Raise"))
            {
                Debug.Log("RAISED EVENT");
                gameEvent.Raise();
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
}
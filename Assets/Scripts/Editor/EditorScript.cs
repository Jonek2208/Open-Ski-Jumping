#if UNITY_EDITOR
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEditor;
using UnityEngine;

namespace OpenSkiJumping.Editor
{
    [CustomEditor(typeof(RuntimeData), true)]
    public class DatabaseObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            RuntimeData databaseObject = (RuntimeData) target;
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

    [CustomEditor(typeof(GameConfigRuntime))]
    public class GameConfigEditorScript : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GameConfigRuntime gameConfig = (GameConfigRuntime) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Set translations"))
            {
                gameConfig.SetTranslations();
            }
        }
    }

    [CustomEditor(typeof(MeshScript))]
    public class EditorScript : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MeshScript meshScript = (MeshScript) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                meshScript.GenerateMesh();
            }
        }
    }
    
    [CustomEditor(typeof(HillsMapGenerator))]
    public class HillsMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (HillsMapGenerator) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Hills"))
            {
                item.GenerateHills();
            }
        }
    }

    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GameEvent gameEvent = (GameEvent) target;
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
            GameEvent gameEvent = (GameEvent) target;
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
            TerrainScript terrainScript = (TerrainScript) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                terrainScript.GenerateTerrain();
            }
        }
    }
}
#endif
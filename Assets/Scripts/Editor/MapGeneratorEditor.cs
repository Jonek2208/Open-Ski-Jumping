#if UNITY_EDITOR

using OpenSkiJumping.PerlinNoise;
using UnityEditor;
using UnityEngine;

namespace OpenSkiJumping.Editor
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (MapGenerator) target;
            if (DrawDefaultInspector())
            {
                if (item.autoUpdate) item.GenerateMap();
            }

            if (GUILayout.Button("Generate"))
                item.GenerateMap();
        }
    }
}
#endif
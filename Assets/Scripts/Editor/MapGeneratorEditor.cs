#if UNITY_EDITOR

using OpenSkiJumping.Hills.TerrainGenerator;
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

    [CustomEditor(typeof(OsmReader))]
    public class OsmReaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (OsmReader) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Read"))
                item.Read();
        }
    }

    [CustomEditor(typeof(ElevationData))]
    public class ElevationDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (ElevationData) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Read files"))
                item.ReadFiles();
        }
    }
}
#endif
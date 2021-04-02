#if UNITY_EDITOR

using OpenSkiJumping.Hills;
using UnityEditor;
using UnityEngine;

namespace OpenSkiJumping.Editor
{
    [CustomEditor(typeof(TerrainLayersMixer))]
    public class TerrainLayersMixerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (TerrainLayersMixer) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Mix textures"))
            {
                item.MixTextures();
            }
        } 
        
        [CustomEditor(typeof(ForestGenerator))]
        public class ForestGeneratorEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var item = (ForestGenerator) target;
                DrawDefaultInspector();
                if (GUILayout.Button("Generate"))
                {
                    item.GenerateForests();
                }
            }
        }

    }

    
}
#endif
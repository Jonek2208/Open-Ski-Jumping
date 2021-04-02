#if UNITY_EDITOR

using OpenSkiJumping.Hills;
using UnityEditor;
using UnityEngine;

namespace OpenSkiJumping.Editor
{
    [CustomEditor(typeof(SeatsGenerator))]
    public class SeatsGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (SeatsGenerator) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                item.Generate();
            }
        }
    }
}
#endif
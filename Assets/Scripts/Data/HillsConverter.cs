using System.Linq;
using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HillsConverter")]
    public class HillsConverter : ScriptableObject
    {
        [SerializeField] private HillsRuntimeNew hillsRuntime;
        [SerializeField] private MapXRuntime mapXRuntime;
        
        public void Convert()
        {
            var oldHills = hillsRuntime.GetData();
            var newHills = oldHills.Select(it =>
                new DatabaseObjectFileData<MapX>(HillsMapFactory.ParseHillsMap(it.value), $"{it.fileName.Substring(0, it.fileName.Length-5)}.xml")).ToList();

            mapXRuntime.SetData(newHills);
        }
        
        public void ConvertBack()
        {
            var oldHills = mapXRuntime.GetData();
            var newHills = oldHills.Select(it =>
                new DatabaseObjectFileData<HillsMap>(HillsMapFactory.GetHillsMap(it.value), "")).ToList();

            hillsRuntime.SetData(newHills);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(HillsConverter))]
    public class EditorTerrainScript : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (HillsConverter) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                item.Convert();
            }
            if (GUILayout.Button("To HillsMap"))
            {
                item.Convert();
            }
        }
    }
#endif
}
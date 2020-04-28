#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OpenSkiJumping.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDecimal))]
    public class SerializableDecimalDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = property.serializedObject.targetObject;
            var inst = (SerializableDecimal)fieldInfo.GetValue(obj);
            var fieldRect = EditorGUI.PrefixLabel(position, label);
            string text = GUI.TextField(fieldRect, inst.value.ToString());
            if (GUI.changed)
            {
                decimal val;
                if (decimal.TryParse(text, out val))
                {
                    inst.value = val;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
#endif
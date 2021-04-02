using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CTI {

	public class CTI_URPTransDrawer : MaterialPropertyDrawer {

		public override void OnGUI (Rect position, MaterialProperty prop, string label, MaterialEditor editor) {
			
			//	Needed since Unity 2019
			EditorGUIUtility.labelWidth = 0;

			Vector4 vec4value = prop.vectorValue;

			GUILayout.Space(-18);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginVertical();
				vec4value.x = EditorGUILayout.Slider("Strength", vec4value.x, 0.0f, 10.0f);
				vec4value.y = EditorGUILayout.Slider("Power", vec4value.y, 0.0f, 10.0f);
			EditorGUILayout.EndVertical();
			if (EditorGUI.EndChangeCheck ()) {
				prop.vectorValue = vec4value;
			}
		}
	}

}